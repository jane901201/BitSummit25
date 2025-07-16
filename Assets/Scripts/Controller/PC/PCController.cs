using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.PC
{
    public class PCController : MonoBehaviour
    {
        public LineRenderer lineRenderer;
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

        private void Update()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            Vector3 offset = positionClamper.MoveTransformInsideScreen(mouseScreenPos, transform) - transform.position;
            transform.localPosition += offset;

            drawTracker.UpdateTracking(transform.position, Time.deltaTime);
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            //moveInput = context.ReadValue<Vector2>();
        }
    }
}