using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [Header("Spline Settings")]
    public SplineContainer splineContainer; // ヒエラルキー上のSplineを指定
    public GameObject objectToMove;         // 移動させたいオブジェクト
    public float moveSpeed = 1f;            // 移動速度
    public bool loop = true;                // 終点に達したら最初に戻るか

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

        // スプライン上の位置と向きを取得
        Vector3 rawPosition = splineContainer.Spline.EvaluatePosition(t);
        Vector3 rawTangent = splineContainer.Spline.EvaluateTangent(t);

        // XとYの符号を反転（Zはそのまま）
        Vector3 position = new Vector3(-rawPosition.x, -rawPosition.y, -rawPosition.z);
        Vector3 tangent = new Vector3(-rawTangent.x, -rawTangent.y, rawTangent.z);

        objectToMove.transform.position = position;
        objectToMove.transform.rotation = Quaternion.LookRotation(tangent);

    }
}


