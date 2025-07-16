using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

public class MoveWithAcceleration : MonoBehaviour
{
    private AccelerometerReader reader;
    private Vector3 velocity = Vector3.zero;
    private Quaternion sensorRotation = Quaternion.identity; // 姿勢追跡用
    private Vector3 estimatedGravity = Vector3.down; // 重力推定用

    public float gyroSensitivity = 1.0f; // 回転速度の積分係数
    [SerializeField] private float gyroDeadZoneX = 13f;
    [SerializeField] private float gyroDeadZoneY = 12f;
    [SerializeField] private float gyroDeadZoneZ = 17f; // Z+回転だけ大きめ
    [SerializeField] private float flatAngleThreshold = 8f; // 平置き判定の角度（度）
    [SerializeField] private float flatDurationThreshold = 1.5f; // 平置き継続時間（秒）
    [SerializeField] private float gravityMagnitude = 1.043f; // 重力の大きさ(g)
    [SerializeField] private float accelDeadZoneX = 13f;
    [SerializeField] private float accelDeadZoneY = 12f;
    [SerializeField] private float accelDeadZoneZ = 17f; // Z+回転だけ大きめ
    [SerializeField] private float accelToSpeedScale = 200.0f;
    [SerializeField] private float shakeStartThreshold = 1.2f;
    [SerializeField] private float shakeEndThreshold = 0.3f;
    private float flatTimer = 0f;
    private bool isCorrecting = false;
    private bool isShaking = false;
    private Vector3 shakeDirection = Vector3.zero;

    void Start()
    {
        reader = GetComponent<AccelerometerReader>();
        if (reader == null)
        {
            Debug.LogError("AccelerometerReader が見つかりません！");
        }

        sensorRotation = Quaternion.identity;
    }

    void Update()
    {
        if (reader == null) return;

        // 生のセンサ値を取得
        Vector3 accel = reader.latestAcceleration;
        Vector3 gyro = reader.latestGyro;
        Vector3 accel_raw = reader.rawLatestAcceleration;

        // ◉ 軸変換（センサ→Unity座標系）
        accel = new Vector3(-accel.x, -accel.z, -accel.y);
        gyro = new Vector3(-gyro.x, -gyro.z, -gyro.y);
        accel_raw = new Vector3(-accel_raw.x, -accel_raw.z, -accel_raw.y);

        // 平置き角度を測定
        float flatAngle = Vector3.Angle(accel_raw.normalized, Vector3.down);

        // ★ ジャイロにデッドゾーン（ノイズ除去）を適用
        // 軸ごとにしきい値適用（Z+は特に大きめに設定）
        gyro = ApplySoftDeadZone(gyro, gyroDeadZoneX, gyroDeadZoneY, gyroDeadZoneZ);

        Vector3 gyroRad = gyro * Mathf.Deg2Rad * gyroSensitivity;

        // 8) 回転処理（ジャイロ）
        transform.Rotate(gyro * gyroSensitivity * Time.deltaTime, Space.Self);

        // 平置き判定
        if (flatAngle < flatAngleThreshold)
        {
            flatTimer += Time.deltaTime;

            if (flatTimer > flatDurationThreshold)
            {
                isCorrecting = true;
            }
        }
        else
        {
            flatTimer = 0f;
            isCorrecting = false;
        }

        // 姿勢補正処理
        if (isCorrecting)
        {
            // 現在のY回転を保持するための回転抽出
            Vector3 euler = transform.rotation.eulerAngles;
            float currentYaw = euler.y;

            // ターゲット：Y軸だけ保持したidentity姿勢
            Quaternion target = Quaternion.Euler(0, currentYaw, 0);

            // 姿勢をゆっくり補正（補間率はお好みで調整）
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 2f);
        }

        // ----- 振る判定ロジック -----

        if (!isShaking)
        {
            // 開始条件：いずれかの軸でしきい値を超えたら「振る」開始
            if (Mathf.Abs(accel.x) > shakeStartThreshold ||
                Mathf.Abs(accel.y) > shakeStartThreshold ||
                Mathf.Abs(accel.z) > shakeStartThreshold)
            {
                isShaking = true;
                shakeDirection = accel.normalized; // 振った瞬間の向きを保持
                Debug.Log("振る開始");
            }
        }
        else
        {
            // 終了条件：全軸が一定以下なら「振る」終了
            if (Mathf.Abs(accel.x) < shakeEndThreshold &&
                Mathf.Abs(accel.y) < shakeEndThreshold &&
                Mathf.Abs(accel.z) < shakeEndThreshold)
            {
                isShaking = false;
                velocity = Vector3.zero; // オブジェクトを止める
                Debug.Log("振る終了");
            }
        }

        if (isShaking)
        {
            // 重力除去後の加速度を使って移動方向に速度を加える
            Vector3 correctedAccel = ApplySoftDeadZone(accel, accelDeadZoneX, accelDeadZoneY, accelDeadZoneZ);
            // 加速度ベクトルを shakeDirection に射影
            Vector3 projectedAccel = Vector3.Project(correctedAccel, shakeDirection);
            velocity = correctedAccel * accelToSpeedScale * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }

        // 緑：加速度方向
        Debug.DrawLine(transform.position, transform.position + accel_raw.normalized * 2f, Color.green);
        // 黄：ハイパス加速度方向
        Debug.DrawLine(transform.position, transform.position + accel.normalized * 2f, Color.yellow);
        // 赤：速度方向
        Debug.DrawLine(transform.position, transform.position + velocity.normalized * 2f, Color.red);
        // 青：推定重力方向
        Debug.DrawLine(transform.position, transform.position + estimatedGravity.normalized * 2f, Color.blue);
        // 紫：補正後の加速度
        //Debug.DrawLine(transform.position, transform.position + correctedAccel.normalized * 2f, Color.magenta);

        // コンソール表示
        Debug.Log($"ハイパス加速度: {accel}");

        // Rキーで位置リセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = Vector3.zero;
            velocity = Vector3.zero; // 同時に慣性もリセット
            transform.rotation = Quaternion.identity; // 姿勢もリセット
            Debug.Log("位置リセットされました。");
        }

    }

    Vector3 Threshold(Vector3 value, float threshold)
    {
        return new Vector3(
        Mathf.Abs(value.x) < threshold ? 0f : value.x,
        Mathf.Abs(value.y) < threshold ? 0f : value.y,
        Mathf.Abs(value.z) < threshold ? 0f : value.z
    );
    }

    Vector3 Threshold(Vector3 input, float thresholdX, float thresholdY, float thresholdZ)
    {
        return new Vector3(
            Mathf.Abs(input.x) < thresholdX ? 0f : input.x,
            Mathf.Abs(input.y) < thresholdY ? 0f : input.y,
            Mathf.Abs(input.z) < thresholdZ ? 0f : input.z
        );
    }

    float ApplySoftDeadZone(float value, float threshold)
    {
        if (Mathf.Abs(value) < threshold)
            return 0f;

        float reduced = Mathf.Abs(value) - threshold;
        float result = Mathf.Sign(value) * reduced;

        // 符号が逆転してしまったらゼロに
        return Mathf.Sign(result) == Mathf.Sign(value) ? result : 0f;
    }
    Vector3 ApplySoftDeadZone(Vector3 input, float thresholdX, float thresholdY, float thresholdZ)
    {
        return new Vector3(
            ApplySoftDeadZone(input.x, thresholdX),
            ApplySoftDeadZone(input.y, thresholdY),
            ApplySoftDeadZone(input.z, thresholdZ)
        );
    }

}