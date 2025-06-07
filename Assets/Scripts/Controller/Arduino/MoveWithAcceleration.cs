using UnityEngine;

public class MoveWithAcceleration : MonoBehaviour
{
    private AccelerometerReader reader;
    private Vector3 smoothedAccel = Vector3.zero;
    public float smoothFactor = 0.9f; // 0.0〜0.99（大きいほど滑らか）
    public float threshold = 0.05f;
    Vector3 processedAccel = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 同じオブジェクトにアタッチされた AccelerometerReader を取得
        reader = GetComponent<AccelerometerReader>();
        if (reader == null)
        {
            Debug.LogError("AccelerometerReader が見つかりません！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (reader != null)
        {
            Vector3 raw = reader.swingAccel;
            smoothedAccel = Vector3.Lerp(smoothedAccel, raw, 1.0f - smoothFactor);
            processedAccel = (smoothedAccel.magnitude < threshold) ? Vector3.zero : smoothedAccel;
            transform.position += processedAccel * Time.deltaTime * 10.0f; // 速度調整のためにスケーリング
        }
    }
}