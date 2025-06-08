using UnityEngine;

public class GoheiFollower : MonoBehaviour
{
    public Transform cursorTransform; // �Ǐ]��iJoystickDrawTracker�̃I�u�W�F�N�g�j

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
