using UnityEngine;

public class AttachMoveObject : MonoBehaviour
{
    public Transform masterTransform; // �}�X�^�[�ȒǏ]��

    // Update is called once per frame
    void Update()
    {
        if (masterTransform != null)
        {
            // ��]��K�p�iZ���̂݉�]�j
            transform.rotation = masterTransform.rotation;
        }
    }
}
