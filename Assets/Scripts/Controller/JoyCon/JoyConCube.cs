using UnityEngine;
using System.Collections.Generic;

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
        initialOrientation = Quaternion.Inverse(j.GetVector());
    }

    void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // Bボタンでセンター位置のリセット
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                Recenter();
            }

            // Position
            accel = TruncateVector3(j.GetAccel(), 2);
            Vector3 deltaAccel = new Vector3(accel.x, accel.y, accel.z);
            velocity += deltaAccel * Time.deltaTime * accelerationFactor;
            velocity *= damping; // Apply damping to reduce drift
            position += velocity * Time.deltaTime;
            gameObject.transform.position = position;

            // Rotation
            gyro = TruncateVector3(j.GetGyro(), 2);
            orientation = initialOrientation * j.GetVector();
            gameObject.transform.rotation = orientation;
        }
    }

    // 小数点切り捨て
    Vector3 TruncateVector3(Vector3 v,int decimals)
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
    }
}