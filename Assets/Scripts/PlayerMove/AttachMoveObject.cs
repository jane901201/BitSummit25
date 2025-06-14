using UnityEngine;

public class AttachMoveObject : MonoBehaviour
{
    public Transform masterTransform; // マスターな追従先

    // Update is called once per frame
    void Update()
    {
        if (masterTransform != null)
        {
            // 回転を適用（Z軸のみ回転）
            transform.rotation = masterTransform.rotation;
        }
    }
}
