using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float baseMoveSpeed = 5f;
    public float fastSwingThreshold = 2.5f;
    public float analysisInterval = 2f;

    private DrawTracker drawTracker;
    private PositionClamper positionClamper;
    private Vector2 moveInput;

    void Start()
    {
        drawTracker = new DrawTracker(lineRenderer, analysisInterval, fastSwingThreshold);
        positionClamper = new PositionClamper(Camera.main);
    }

    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * baseMoveSpeed * moveInput.magnitude * Time.deltaTime;
        //transform.position += movement;
        //transform.position = positionClamper.MoveTransformInsideScreen(position, transform);
        
        Vector3 newPosition = transform.position + movement;

        // 視窗内に制限された座標を取得
        Vector3 clampedPosition = PositionClamper.ClampToViewport(newPosition, Camera.main);

        // 位置を更新
        transform.position = clampedPosition;

        
        drawTracker.UpdateTracking(transform.position, Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnDrawButton(InputAction.CallbackContext context)
    {
        // if (context.started)
        // {
        //     drawTracker.StartTracking();
        // }
        // else if (context.canceled)
        // {
        //     drawTracker.StopTracking();
        // }
    }
}