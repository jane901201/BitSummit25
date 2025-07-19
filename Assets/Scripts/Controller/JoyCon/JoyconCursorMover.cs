using UnityEngine;
using UnityEngine.InputSystem;
using Controller;


public class JoyconCursorMover : MonoBehaviour
{
    public float sensitivity = 100f; // 感度調整（スクリーンピクセル単位）
    public float maxSpeed = 1000f;   // 最大スクリーンスピード（ピクセル/秒）

    private Vector2 screenPosition; // カーソルのスクリーン座標
    private PositionClamper positionClamper;

    void Start()
    {
        positionClamper = new PositionClamper(Camera.main);
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
    }

    void Update()
    {
        if (JoyconManager.Instance.j == null || JoyconManager.Instance.j.Count == 0) return;

        var joycon = JoyconManager.Instance.j[0];
        Vector3 gyro = joycon.GetGyro(); // gyro.x: pitch, gyro.y: yaw

        // ジャイロからスクリーン方向の移動量を計算（スクリーンピクセルベース）
        Vector2 delta = new Vector2(-gyro.y, -gyro.x) * sensitivity * Time.deltaTime;

        // スピード制限
        delta = Vector2.ClampMagnitude(delta, maxSpeed * Time.deltaTime);

        // スクリーン座標を更新
        screenPosition += delta;

        // 画面内に制限
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

        // transform をカメラに応じてワールド座標に変換して移動
        Vector3 newWorldPos = positionClamper.MoveTransformInsideScreen(screenPosition, transform);
        transform.position = newWorldPos;
    }
}
