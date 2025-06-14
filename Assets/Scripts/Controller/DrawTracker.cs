namespace Controller
{
    using UnityEngine;
    using System.Collections.Generic;

    public class DrawTracker
    {
        private List<Vector3> recordedPositions = new();
        private float timer = 0f;
        private readonly float analysisInterval;
        private readonly float fastSwingThreshold;
        private readonly LineRenderer lineRenderer;

        private bool isTracking = false;

        public DrawTracker(LineRenderer renderer, float interval, float threshold)
        {
            lineRenderer = renderer;
            analysisInterval = interval;
            fastSwingThreshold = threshold;
        }

        public void StartTracking()
        {
            isTracking = true;
            timer = 0f;
            recordedPositions.Clear();
            lineRenderer.positionCount = 0;
        }

        public void StopTracking()
        {
            isTracking = false;
            timer = 0f;
            recordedPositions.Clear();
            lineRenderer.positionCount = 0;
        }

        public void UpdateTracking(Vector3 position, float deltaTime)
        {
           // if (!isTracking) return;

            timer += deltaTime;
            recordedPositions.Add(position);
            lineRenderer.positionCount = recordedPositions.Count;
            lineRenderer.SetPositions(recordedPositions.ToArray());

            if (timer >= analysisInterval)
            {
                Debug.Log("2秒経過");
                AnalyzeAndReset();
            }
        }

        private void AnalyzeAndReset()
        {
            if (recordedPositions.Count >= 2)
            {
                Vector3 dir = (recordedPositions[^1] - recordedPositions[0]).normalized;
                float distance = Vector3.Distance(recordedPositions[0], recordedPositions[^1]);
                float speed = distance / analysisInterval;

                var swingDir = JudgeDirection(dir);
                var swingSpeed = JudgeSpeed(speed);

                Debug.Log($"方向: {swingDir}, 速度: {speed:F2} → {swingSpeed}");
                GameManager.Instance.TakeGhostsDamage(swingDir, swingSpeed);
            }

            recordedPositions.Clear();
            lineRenderer.positionCount = 0;
            timer = 0f;
        }

        private SwingDirection JudgeDirection(Vector3 dir)
        {
            float absX = Mathf.Abs(dir.x);
            float absY = Mathf.Abs(dir.y);
            if (Mathf.Abs(absX - absY) < 0.3f) return SwingDirection.Diagonal;
            else if (absX > absY) return SwingDirection.Horizontal;
            else return SwingDirection.Vertical;
        }

        private SwingSpeed JudgeSpeed(float speed)
        {
            return speed >= fastSwingThreshold ? SwingSpeed.Fast : SwingSpeed.Slow;
        }
    }
}