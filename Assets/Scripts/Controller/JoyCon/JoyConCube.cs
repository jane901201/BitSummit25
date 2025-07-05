using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Remoting;

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
            orientation = initialOrientation * joyconToUnity * j.GetVector();
            gameObject.transform.rotation = orientation;

            // 重力
            //Vector3 gravity = orientation * Vector3.down; // Unityの重力方向を取得

            // Position
            accel = j.GetAccel();
            Vector3 unityAxisAccel = Vector3.zero;
            unityAxisAccel.x = accel.y;
            unityAxisAccel.y = -accel.x; // Joy-Conの座標系からUnityの座標系への変換
            unityAxisAccel.z = -accel.z;
            //unityAxisAccel = unityAxisAccel - gravity; // 重力を除去
            //velocity += unityAxisAccel * accelerationFactor * Time.deltaTime; // 加速度を速度に変換
            //transform.position += velocity * Time.deltaTime; // 速度を位置に変換

            // ① Joy-Con加速度（ローカル）→ Unityワールド座標へ変換
            Vector3 worldAccel = orientation * j.GetAccel();

            // ② 重力ベクトル（Unityの世界で "下" 方向 = Vector3.down）
            Vector3 gravity = Vector3.down;

            // ③ 重力を除去した線形加速度（地面に置いた状態ならほぼゼロになる）
            Vector3 linearAccel = worldAccel - gravity;

            // ④ 移動反映
            velocity += linearAccel * accelerationFactor * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;

            // 
            Debug.Log($"JoyConAccel: {j.GetAccel()} | WorldAccel: {worldAccel}");

            // Bボタンでセンター位置のリセット
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                Recenter();
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
            initialOrientation = Quaternion.Inverse(j.GetVector());
        }
        position = Vector3.zero; // Reset position to origin
    }
}