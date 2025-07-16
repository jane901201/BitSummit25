using UnityEngine;

public class GoheiFollower : MonoBehaviour
{
    public Transform cursorTransform;

    void Update()
    {
        if (cursorTransform != null)
        {
            Vector3 cursorLocalOffset = new Vector3(cursorTransform.localPosition.x / 5, 0, 0);

            // z‚¾‚¯ +10 ‘«‚·
            cursorLocalOffset.z += -5.2f;

            transform.localPosition = cursorLocalOffset;

            Vector3 direction = new Vector3(cursorTransform.localPosition.x, cursorTransform.localPosition.y - 10, 0);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0, 0, -angle - 90);
        }
    }

}

