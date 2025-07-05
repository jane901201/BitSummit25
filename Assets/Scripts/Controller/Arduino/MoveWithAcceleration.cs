using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

public class MoveWithAcceleration : MonoBehaviour
{
    private AccelerometerReader reader;
    private Vector3 smoothedAccel = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Quaternion sensorRotation = Quaternion.identity; // 姿勢追跡用
    [SerializeField] private float gravityCalibration = -1.043f; // 重力補正値

    public float smoothFactor = 0.9f;
    public float threshold = 0.05f;
    public float accelToSpeedScale = 2.0f;
    public float damping = 0.98f;
    public float gyroSensitivity = 1.0f; // 回転速度の積分係数
    public float gyroDeadZone = 0.9f;

    public float accelThreshold = 0.02f;
    public float accelMax = 0.08f;
    public float accelScale = 100.0f;

    // ───────── パラメータ追加（Inspectorで調整）
    [Range(0f, 1f)] public float accelLPF = 0.2f;   // 0.1~0.3 で滑らか
    public float velocityDamp = 0.99f;            // 0.98~0.995 慣性減衰

    // ───── 追加パラメータ
    public float g2ms = 9.80665f;   // g から m/s² へ
    public float accelGain = 40f;   // 1 g → 40 m/s 相当

    // ───────── 変数追加
    private Vector3 filteredAccel;                 // 1段ローパス後の加速度

    [Range(0f, 1f)] public float compGain = 0.12f;  // 加速度で補正する割合
    public float horizProject = 1f;                 // 0=水平面固定 / 1=重力面固定

    static Vector3 hpPrev;                // 前回値を保持 (メンバでもOK)

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

        // ◉ 軸変換（センサ→Unity座標系）
        accel = new Vector3(-accel.x, -accel.z, -accel.y);
        gyro = new Vector3(-gyro.x, -gyro.z, -gyro.y);

        // 1) ジャイロ積分 -------------------------------
        Vector3 gyroRad = gyro * Mathf.Deg2Rad * gyroSensitivity;
        sensorRotation = sensorRotation * Quaternion.Euler(gyroRad * Time.deltaTime);
        sensorRotation.Normalize();

        // 2) 簡易コンプリメンタリーフィルタで姿勢補正 --
        Vector3 downEst = sensorRotation * Vector3.down;    // 推定した“下”
        Vector3 downMeas = accel.normalized;                 // 実測“下”（加速度方向）

        // 回転誤差を少しだけ補正
        Quaternion corr = Quaternion.FromToRotation(downEst, downMeas);
        sensorRotation = Quaternion.Slerp(Quaternion.identity, corr, compGain) * sensorRotation;
        sensorRotation.Normalize();

        // 3) 最新の重力ベクトル -------------------------
        Vector3 gravity = sensorRotation * new Vector3(0, gravityCalibration, 0); ;

        // 4) 重力除去
        Vector3 correctedAccel = accel - gravity;

        // 4.5) ★ センサ座標 → ワールド座標へ変換
        //     (センサの向きに依存しない移動ベクトルになる)
        Vector3 worldAccel = sensorRotation * correctedAccel;   // <-- 追加

        Vector3 motionInput = worldAccel;

        // accelThreshold以下の値は無視
        motionInput = Threshold(motionInput, accelThreshold);

        // 6') ★ 低域ノイズ除去用ローパス（指数移動平均）
        filteredAccel = Vector3.Lerp(filteredAccel, motionInput, 1f - accelLPF);

        // 6)   LPF 後の線形加速度を m/s² に換算
        Vector3 linAcc = filteredAccel * g2ms;

        // 7) 移動処理
        velocity += linAcc * accelGain * Time.deltaTime;   // a → v
        velocity *= velocityDamp;
        transform.position += velocity * Time.deltaTime;   // v → x

        // 8) 回転処理（ジャイロ）
        transform.Rotate(gyro * gyroSensitivity * Time.deltaTime, Space.Self);

        // ◉ 表示
        // キャリブレーション前後の加速度と検出した重力を表示
        Debug.Log($"Corrected accel: {correctedAccel:F3} | velocity: {velocity:F3} | motionInput: {motionInput:F3} | Gravity: {gravity:F3}");

        // Rキーで位置リセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = Vector3.zero;
            velocity = Vector3.zero; // 同時に慣性もリセット
            transform.rotation = Quaternion.identity; // 姿勢もリセット
            Debug.Log("位置リセットされました。");
        }

    }

    Vector3 ProcessedAccel(Vector3 input)
    {
        float mag = input.magnitude;

        if (mag < accelThreshold)
            return Vector3.zero;

        float normalized = Mathf.InverseLerp(accelThreshold, accelMax, mag);
        return input.normalized * normalized * accelScale;
    }

    Vector3 Threshold(Vector3 value, float threshold)
    {
        return new Vector3(
        Mathf.Abs(value.x) < threshold ? 0f : value.x,
        Mathf.Abs(value.y) < threshold ? 0f : value.y,
        Mathf.Abs(value.z) < threshold ? 0f : value.z
    );
    }
}