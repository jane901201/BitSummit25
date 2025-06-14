using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [Header("Spline Settings")]
    public SplineContainer splineContainer; // �q�G�����L�[���Spline���w��
    public GameObject objectToMove;         // �ړ����������I�u�W�F�N�g
    public float moveSpeed = 1f;            // �ړ����x
    public bool loop = true;                // �I�_�ɒB������ŏ��ɖ߂邩

    private float t = 0f;

    void Update()
    {
        if (splineContainer == null || objectToMove == null || splineContainer.Spline == null)
            return;

        float splineLength = splineContainer.CalculateLength();
        t += (moveSpeed / splineLength) * Time.deltaTime;

        if (t > 1f)
        {
            if (loop)
                t = 0f;
            else
                t = 1f;
        }

        // �X�v���C����̈ʒu�ƌ������擾
        var position = splineContainer.Spline.EvaluatePosition(t);
        var tangent = splineContainer.Spline.EvaluateTangent(t);

        objectToMove.transform.position = position;
        objectToMove.transform.rotation = Quaternion.LookRotation(tangent);
    }
}


