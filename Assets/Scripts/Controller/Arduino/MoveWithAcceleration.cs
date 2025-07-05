using UnityEngine;

public class MoveWithAcceleration : MonoBehaviour
{
    private AccelerometerReader reader;
    private Vector3 smoothedAccel = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Quaternion sensorRotation = Quaternion.identity; // 姿勢追跡用

    public float smoothFactor = 0.9f;
    public float threshold = 0.05f;
    public float accelToSpeedScale = 2.0f;
    public float damping = 0.98f;
    public float gyroSensitivity = 1.0f; // 回転速度の積分係数
    public float gyroDeadZone = 0.9f;

    public float accelThreshold = 0.002f;
    public float accelMax = 0.08f;
    public float accelScale = 100.0f;

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
        Vector3 gravity = sensorRotation * Vector3.down;

        // 4) 重力除去
        Vector3 correctedAccel = accel - gravity;

        // 4.5) ★ センサ座標 → ワールド座標へ変換
        //     (センサの向きに依存しない移動ベクトルになる)
        Vector3 worldAccel = sensorRotation * correctedAccel;   // <-- 追加

        // 5) ★ 高域成分のみ抽出（10 Hz 以上）
        float hpAlpha = 0.9f;                 // 0.8〜0.95
        Vector3 highPass = hpAlpha * (hpPrev + correctedAccel - smoothedAccel);
        hpPrev = smoothedAccel;               // 次回用に保存

        //Vector3 motionInput = Vector3.Lerp(highPass, highPass, 1f); // ← そのまま使用

        // 今回は上下移動も生かしたいとのことなので、そのまま使う
        Vector3 motionInput = worldAccel;                       // <-- 変更

        // 6) 平滑化・スケーリング
        //smoothedAccel = Vector3.Lerp(smoothedAccel, motionInput, 1f - smoothFactor);
        //Vector3 drive = ProcessedAccel(motionInput);
        Vector3 drive = motionInput * accelToSpeedScale; // ← 変更

        // 7) 移動処理
        velocity += drive * Time.deltaTime;
        velocity *= damping;
        transform.position += velocity;

        // 8) 回転処理（ジャイロ）
        transform.Rotate(gyro * gyroSensitivity * Time.deltaTime, Space.Self);

        // ◉ 表示（必要なら）
        Debug.Log($"Corrected Accel: {correctedAccel:F3} | Gravity: {gravity:F3}");
        // Drive: {drive:F3} | Velocity: {velocity:F3}");
        //Debug.Log($"Position: {transform.position:F3} | Sensor Rotation: {sensorRotation.eulerAngles:F3}");

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
}