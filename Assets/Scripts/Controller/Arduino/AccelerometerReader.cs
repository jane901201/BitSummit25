using UnityEngine;
using System.IO.Ports;

public class AccelerometerReader : MonoBehaviour
{
    //[Header("Serial")]
    //public string portName = "COM3";
    //public int baudRate = 115200;

    //[Header("Calibration / Dead-Zone")]
    //public KeyCode calibKey = KeyCode.C; // Cキーでキャリブレーション
    //public bool autoCalibAtStart = true;    // trueなら Start() ですぐ取る
    //[Range(0f, 0.3f)]
    //public float deadZone = 0.03f;          // 0.03g ≒ 0.3 m/s²

    //// ─────────────────────────────
    //private SerialPort serial;
    //private Vector3 gravityRef = Vector3.zero; // 重力ベースライン
    //public Vector3 acceleration { get; private set; } // デッドゾーン＆キャリブ適用後

    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    serial = new SerialPort(portName, baudRate) { ReadTimeout = 100 };
    //    try { serial.Open(); }
    //    catch (System.Exception e) { Debug.LogError("Serial port failed: " + e.Message); }
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //――― ① キー入力で基準を取り直す ―――
    //    if (Input.GetKeyDown(calibKey) && serial.IsOpen)
    //    {
    //        if (TryReadOnce(out Vector3 raw))
    //            gravityRef = raw; // 今の生加速度を基準に
    //    }

    //    //――― ② 毎フレームの読み取り ―――
    //    if (serial != null && serial.IsOpen && TryReadOnce(out Vector3 a))
    //    {
    //        if (autoCalibAtStart && gravityRef == Vector3.zero)   // 初回だけ自動キャリブ
    //            gravityRef = a;

    //        Vector3 motion = a - gravityRef;                      // 重力を除去

    //        //――― ③ デッドゾーン判定 ―――
    //        acceleration = (motion.magnitude < deadZone) ? Vector3.zero : motion;
    //    }
    //}
    ///// <summary>
    ///// シリアル1行を取得して Vector3 に変換（失敗時 false）
    ///// </summary>
    //bool TryReadOnce(out Vector3 vec)
    //{
    //    vec = Vector3.zero;
    //    try
    //    {
    //        string line = serial.ReadLine();          // 例: "0.01,-0.02,1.00"
    //        var sp = line.Split(',');
    //        if (sp.Length != 3) return false;

    //        float x = float.Parse(sp[0]);
    //        float y = float.Parse(sp[1]);
    //        float z = float.Parse(sp[2]);
    //        vec = new Vector3(x, y, z);
    //        return true;
    //    }
    //    catch { return false; }
    //}
    //void OnDestroy()
    //{
    //    if (serial != null && serial.IsOpen) serial.Close();
    //}
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