using UnityEngine;

public class GoheiFollower : MonoBehaviour
{
    public Transform cursorTransform; // 追従先（JoystickDrawTrackerのオブジェクト）
    public Transform masterTransform; // マスターな追従先

    void Update()
    {
        if (cursorTransform != null)
        {
            Vector3 currentPosition = transform.position;
            Vector3 cursorPos = cursorTransform.position;
            Vector3 masterPos = masterTransform.position;

            transform.position = new Vector3(cursorPos.x/5, currentPosition.y, currentPosition.z);
            // カーソル方向へのベクトルを取得
            Vector3 direction;
            direction.y = cursorPos.y - masterPos.y - 10;
            direction.x = cursorPos.x - masterPos.x;

            // 回転角度を求める（2D用にZ軸周りの回転）
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 回転を適用（Z軸のみ回転）
            transform.rotation = masterTransform.rotation * Quaternion.Euler(0, 0, -angle - 90);
        }
    }
}
