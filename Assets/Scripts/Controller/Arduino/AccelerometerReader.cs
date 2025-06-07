using UnityEngine;
using System.IO.Ports;

public class AccelerometerReader : MonoBehaviour
{
    [Header("Serial")]
    public string portName = "COM3";
    public int baudRate = 115200;

    [Header("High-Pass Filter")]
    [Range(0.80f, 0.99f)]
    public float alpha = 0.95f;      // 大きいほど「重力追従が速い＝カット帯域が低い」

    [Range(0f, 0.3f)]
    public float deadZone = 0.03f;   // G単位：ここ未満は完全に0

    public Vector3 swingAccel;       // 剣の“振り”成分（Unity空間へマッピング後）

    // ─────────────────────────
    SerialPort serial;
    Vector3 gravity;                 // ローパスで追従する重力ベクトル

    void Start()
    {
        serial = new SerialPort(portName, baudRate) { ReadTimeout = 100 };
        try { serial.Open(); } catch (System.Exception e) { Debug.LogError(e); }
    }

    void Update()
    {
        if (serial?.IsOpen == true && TryRead(out Vector3 raw))
        {
            // 1) Android流ローパスで重力を推定
            gravity = Vector3.Lerp(gravity, raw, 1f - alpha);

            // 2) ハイパス：動き成分
            Vector3 motion = raw - gravity;

            // 3) デッドゾーン
            swingAccel = (motion.magnitude < deadZone) ? Vector3.zero : motion;

            // ※ 必要なら XYZ → Unity 座標系の軸入替えをここで行う
        }
    }

    bool TryRead(out Vector3 v)
    {
        v = Vector3.zero;
        try
        {
            string[] sp = serial.ReadLine().Split(',');
            if (sp.Length != 3) return false;
            v = new Vector3(float.Parse(sp[0]), float.Parse(sp[1]), float.Parse(sp[2]));
            return true;
        }
        catch { return false; }
    }

    void OnDestroy() { if (serial?.IsOpen == true) serial.Close(); }
}