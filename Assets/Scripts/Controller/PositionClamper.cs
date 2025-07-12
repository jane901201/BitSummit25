namespace Controller
{
    using UnityEngine;

    public class PositionClamper
    {
        private Camera camera;

        public PositionClamper(Camera camera = null)
        {
            this.camera = camera ?? Camera.main;
        }

        public Vector3 GetClampedWorldPosition(Vector3 screenPos, Transform targetTransform)
        {
            float fixedZ = targetTransform.position.z;

            // スクリーン座標からワールド座標に変換
            Vector3 mouseWorldPos = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camera.WorldToScreenPoint(targetTransform.position).z));

            // ビューポート座標に変換してクランプ（0〜1の範囲に制限）
            Vector3 viewportPos = camera.WorldToViewportPoint(mouseWorldPos);
            viewportPos.x = Mathf.Clamp01(viewportPos.x);
            viewportPos.y = Mathf.Clamp01(viewportPos.y);

            // クランプ後のビューポート座標をワールド座標に戻す
            Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);
            clampedWorldPos.z = fixedZ;

            return clampedWorldPos;
        }

        public Vector3 MoveTransformInsideScreen(Vector3 screenPos, Transform targetTransform)
        {
            return GetClampedWorldPosition(screenPos, targetTransform);
        }
        
        public static Vector3 ClampToViewport(Vector3 worldPosition, Camera camera)
        {
            Vector3 viewportPos = camera.WorldToViewportPoint(worldPosition);
            viewportPos.x = Mathf.Clamp01(viewportPos.x);
            viewportPos.y = Mathf.Clamp01(viewportPos.y);
            Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);
            clampedWorldPos.z = worldPosition.z; // Zは維持
            return clampedWorldPos;
        }
    } 
}