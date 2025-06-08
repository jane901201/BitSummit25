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

            transform.position = new Vector3(cursorPos.x/5, currentPosition.y, currentPosition.z);
            // �J�[�\�������ւ̃x�N�g�����擾
            Vector3 direction;
            direction.y = cursorPos.y - 10;
            direction.x = cursorPos.x - 0;

            // ��]�p�x�����߂�i2D�p��Z������̉�]�j
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // ��]��K�p�iZ���̂݉�]�j
            transform.rotation = Quaternion.Euler(0, 0, - angle - 90);
        }
    }
}
