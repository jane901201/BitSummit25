using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class JoystickDrawTracker : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float baseMoveSpeed = 5f;
    public float moveSpeed = 5f;
    private List<Vector3> recordedPositions = new List<Vector3>();
    private bool isDrawing = false;

    private Vector2 moveInput;
    private Vector3 lastPosition;
    private float recordInterval = 0.05f;
    private float recordTimer = 0f;

    void Update()
    {
        float inputStrength = moveInput.magnitude; // 0〜1
        float adjustedSpeed = baseMoveSpeed * inputStrength;
        
            Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * adjustedSpeed * Time.deltaTime;
            transform.position += movement;
            
        if (isDrawing)
        {

            // 軌跡記録
            recordTimer += Time.deltaTime;
            if (recordTimer >= recordInterval)
            {
                recordedPositions.Add(transform.position);
                lineRenderer.positionCount = recordedPositions.Count;
                lineRenderer.SetPositions(recordedPositions.ToArray());
                recordTimer = 0f;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnDrawButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // 開始時：初期化
            isDrawing = true;
            recordedPositions.Clear();
            recordedPositions.Add(transform.position);
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, transform.position);
        }
        else if (context.canceled)
        {
            // 終了時：方向・速度の解析
            isDrawing = false;
            if (recordedPositions.Count >= 2)
            {
                Vector3 dir = (recordedPositions[^1] - recordedPositions[0]).normalized;
                float distance = Vector3.Distance(recordedPositions[0], recordedPositions[^1]);
                float duration = (recordedPositions.Count - 1) * recordInterval;
                float speed = distance / duration;
                
                SwingDirection swingDir = JudgeDirection(dir);
                SwingSpeed swingSpeed = JudgeSpeed(speed);

                Debug.Log($"方向: {swingDir}, 速度: {speed:F2} → {swingSpeed}");
                GameManager.Instance.TakeGhostsDamage(swingDir, swingSpeed);
                
            }
            
            lineRenderer.positionCount = 0;
            recordedPositions.Clear();
        }
    }
    
    public SwingDirection JudgeDirection(Vector3 dir)
    {
        // 横：X成分が強い、縦：Z成分が強い、斜め：同じくらい
        float absX = Mathf.Abs(dir.x);
        float absZ = Mathf.Abs(dir.z);

        if (Mathf.Abs(absX - absZ) < 0.3f)
        {
            return SwingDirection.Diagonal;
        }
        else if (absX > absZ)
        {
            return SwingDirection.Horizontal;
        }
        else
        {
            return SwingDirection.Vertical;
        }
    }
    
    private SwingSpeed JudgeSpeed(float avgSpeed)
    {
        return avgSpeed >= 2.5f ? SwingSpeed.Fast : SwingSpeed.Slow;
    }
}
