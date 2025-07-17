using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Cinemachine.Utility;

public class JoyConCube : MonoBehaviour
{
    private List<Joycon> joycons;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public Vector3 startGyro;// Initial gyro values
    public int jc_ind = 0;
    public Quaternion orientation;
    private Quaternion initialOrientation;

    private Vector3 velocity = Vector3.zero;
    private Vector3 position = Vector3.zero;
    public float accelerationFactor = 5.0f; // 動きの大きさを調整
    public float damping = 0.98f; // 減衰（ドリフト防止）
    private Quaternion joyconToUnity = Quaternion.Euler(-90, 0, 0); // 軸変換補正

    public float gyroSensitivity = 1.0f; // 回転速度の積分係数
    [SerializeField] private float accelToSpeedScale = 200.0f;
    [SerializeField] private float gyroDeadZoneX = 0.1f;
    [SerializeField] private float gyroDeadZoneY = 0.2f;
    [SerializeField] private float gyroDeadZoneZ = 0.1f;
    [SerializeField] private float accelDeadZoneX = 13f;
    [SerializeField] private float accelDeadZoneY = 12f;
    [SerializeField] private float accelDeadZoneZ = 17f; // Z+回転だけ大きめ
    [SerializeField] private float shakeStartThreshold = 1.2f;
    [SerializeField] private float shakeEndThreshold = 0.3f;

    private Vector3 accelFiltered = Vector3.zero;
    [SerializeField] private float highpassFactor = 0.9f; // 0〜1、小さいほど低周波カット強くなる
    private bool isShaking = false;
    private Vector3 shakeDirection = Vector3.zero;


    void Start()
    {
        gyro = Vector3.zero;
        accel = Vector3.zero;
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
            return;
        }
        Joycon j = joycons[jc_ind];
        // Joy-Conを縦持ちしている前提で初期姿勢を取得
        initialOrientation = Quaternion.Inverse(joyconToUnity * j.GetVector());
    }

    void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // Rotation
            gyro = TruncateVector3(j.GetGyro(), 2);
            // Unityの座標系に合わせてジャイロ軸を変換
            gyro = new Vector3(-gyro.y, gyro.z, -gyro.x);
            gyro = ApplySoftDeadZone(gyro, gyroDeadZoneX, gyroDeadZoneY, gyroDeadZoneZ);
            // 回転処理（ジャイロ）
            Vector3 gyroRad = gyro * Mathf.Deg2Rad * gyroSensitivity;
            transform.Rotate(gyroRad * gyroSensitivity * Time.deltaTime, Space.Self);

            // Position
            accel = j.GetAccel();
            Vector3 unityAxisAccel = Vector3.zero;
            unityAxisAccel.x = -accel.y;
            unityAxisAccel.y = accel.z; // Joy-Conの座標系からUnityの座標系への変換
            unityAxisAccel.z = -accel.x;
            // Joy-Conの回転姿勢を取得
            orientation = initialOrientation * joyconToUnity * j.GetVector();

            // 重力方向の推定
            Vector3 estimatedGravity = orientation * Vector3.down;

            // 世界座標系に変換し、重力を除去
            Vector3 worldAccel = orientation * unityAxisAccel;
            Vector3 linearAccel = worldAccel - estimatedGravity;
            // ハイパスフィルタ適用
            accelFiltered = HighPassFilter(linearAccel, accelFiltered, highpassFactor);

            // ----- 振る判定ロジック -----

            if (!isShaking)
            {
                // 開始条件：いずれかの軸でしきい値を超えたら「振る」開始
                if (Mathf.Abs(accelFiltered.x) > shakeStartThreshold ||
                    Mathf.Abs(accelFiltered.y) > shakeStartThreshold ||
                    Mathf.Abs(accelFiltered.z) > shakeStartThreshold)
                {
                    isShaking = true;
                    shakeDirection = accelFiltered.normalized; // 振った瞬間の向きを保持
                    Debug.Log("振る開始");
                }
            }
            else
            {
                // 終了条件：全軸が一定以下なら「振る」終了
                if (Mathf.Abs(accelFiltered.x) < shakeEndThreshold &&
                    Mathf.Abs(accelFiltered.y) < shakeEndThreshold &&
                    Mathf.Abs(accelFiltered.z) < shakeEndThreshold)
                {
                    isShaking = false;
                    velocity = Vector3.zero; // オブジェクトを止める
                    Debug.Log("振る終了");
                }
            }
            Vector3 correctedAccel = Vector3.zero;
            if (isShaking)
            {
                // 重力除去後の加速度を使って移動方向に速度を加える
                correctedAccel = ApplySoftDeadZone(accelFiltered, accelDeadZoneX, accelDeadZoneY, accelDeadZoneZ);
                // 加速度ベクトルを shakeDirection に射影
                Vector3 projectedAccel = Vector3.Project(correctedAccel, shakeDirection);
                velocity = correctedAccel * accelToSpeedScale * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;
            }

            // 緑：加速度方向
            Debug.DrawLine(transform.position, transform.position + correctedAccel.normalized * 2f, Color.green);
            // 黄：ハイパス加速度方向
            Debug.DrawLine(transform.position, transform.position + accelFiltered.normalized * 2f, Color.yellow);
            // 赤：速度方向
            Debug.DrawLine(transform.position, transform.position + velocity.normalized * 2f, Color.red);
            // 青：推定重力方向
            // Debug.DrawLine(transform.position, transform.position + estimatedGravity.normalized * 2f, Color.blue);

            //Debug.Log($"accelFiltered: {accelFiltered}");
            Debug.Log($"JoyconAccel : {j.GetAccel()},JoyconGyro : {j.GetGyro()}");

            // Bボタンでセンター位置のリセット
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN) || Input.GetKeyDown(KeyCode.R))
            {
                Recenter();
                transform.position = Vector3.zero; // Reset position to origin
            }
        }
    }

    // 小数点切り捨て
    Vector3 TruncateVector3(Vector3 v, int decimals)
    {
        float factor = Mathf.Pow(10, decimals);
        return new Vector3(
            Mathf.Floor(v.x * factor) / factor,
            Mathf.Floor(v.y * factor) / factor,
            Mathf.Floor(v.z * factor) / factor
        );
    }

    // 姿勢初期化
    void Recenter()
    {
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];
            j.Recenter();
            transform.rotation = Quaternion.identity; // Reset rotation
            initialOrientation = Quaternion.Inverse(j.GetVector());
        }
        position = Vector3.zero; // Reset position to origin
    }
    Vector3 HighPassFilter(Vector3 current, Vector3 previous, float factor)
    {
        return factor * (previous + current - accel);
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