using UnityEngine;

public class GoheiFollower : MonoBehaviour
{
    public Transform cursorTransform; // 追従先（JoystickDrawTrackerのオブジェクト）

    void Update()
    {
        if (cursorTransform != null)
        {
            Vector3 currentPosition = transform.position;
            Vector3 cursorPos = cursorTransform.position;

            transform.position = new Vector3(cursorPos.x, cursorPos.y, currentPosition.z);
        }
    }
}
