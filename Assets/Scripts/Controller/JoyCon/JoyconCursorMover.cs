using UnityEngine;
using UnityEngine.InputSystem;
using Controller;
using UnityEngine.Splines;


public class JoyconCursorMover : MonoBehaviour
{
    public float sensitivity = 100f; // 感度調整（スクリーンピクセル単位）
    public float maxSpeed = 1000f;   // 最大スクリーンスピード（ピクセル/秒）
    [SerializeField] private float shakeStartThreshold = 1.2f; //
    public LineRenderer lineRenderer;
    public float baseMoveSpeed = 5f;
    public float fastSwingThreshold = 2.5f;
    public float analysisInterval = 2f;


    private DrawTracker drawTracker;


    private Vector2 screenPosition; // カーソルのスクリーン座標
    private PositionClamper positionClamper;

    //public float 

    void Start()
    {
        positionClamper = new PositionClamper(Camera.main);
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        drawTracker = new DrawTracker(lineRenderer, analysisInterval, fastSwingThreshold);
    }

    void Update()
    {
        if (JoyconManager.Instance.j == null || JoyconManager.Instance.j.Count == 0) return;

        var joycon = JoyconManager.Instance.j[0];
        Vector3 gyro = joycon.GetGyro();// gyro.x: pitch, gyro.y: yaw
        Quaternion quaternion = joycon.GetVector();

        float magnitude = Mathf.Sqrt((gyro.z) * (gyro.z) + (gyro.y) * (gyro.y));

        // ジャイロからスクリーン方向の移動量を計算（スクリーンピクセルベース）
        Vector2 delta = new Vector2(gyro.z, gyro.y) * sensitivity * magnitude * Time.deltaTime;
        
        // スピード制限
        delta = Vector2.ClampMagnitude(delta, maxSpeed * Time.deltaTime);

        Debug.Log(gyro + " " + quaternion);

        // スクリーン座標を更新
        //screenPosition += delta;

        // 画面内に制限
        //screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        //screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);


        Vector3 newPosition = transform.position + new Vector3(delta.x, delta.y, 0);

        // 視窗内に制限された座標を取得
        Vector3 clampedPosition = PositionClamper.ClampToViewport(newPosition, Camera.main);

        // Z座標を維持して位置を更新
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, transform.position.z);



        drawTracker.UpdateTracking(transform.position, Time.deltaTime);


        //// transform をカメラに応じてワールド座標に変換して移動
        //Vector3 newWorldPos = positionClamper.MoveTransformInsideScreen(screenPosition, transform);
        //transform.position = newWorldPos;
    }
}
