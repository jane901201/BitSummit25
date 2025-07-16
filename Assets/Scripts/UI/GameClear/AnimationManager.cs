using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private GameObject scaleTarget;      // スケール変化させる3Dオブジェクト
    [SerializeField] private GameObject appearTarget;     // 最初非表示→表示させる3Dオブジェクト
    [SerializeField] private float enlargeScale = 3f;     // 最初に大きくする倍率
    [SerializeField] private float shrinkDuration = 1.0f; // 元のサイズに戻るまでの時間

    private Vector3 originalScale;

    private void Start()
    {
        if (scaleTarget != null)
        {
            scaleTarget.SetActive(false); // 最初は非表示
            originalScale = scaleTarget.transform.localScale;
        }

        if (appearTarget != null)
        {
            appearTarget.SetActive(false); // 最初は非表示
        }

        StartCoroutine(PlayAnimations());
    }

    private IEnumerator PlayAnimations()
    {
        // シーン開始後0.5秒待つ
        yield return new WaitForSeconds(0.5f);

        if (scaleTarget != null)
        {
            // めっちゃ大きく
            scaleTarget.transform.localScale = originalScale * enlargeScale;
            scaleTarget.SetActive(true); //表示
            // ゆっくり縮小
            yield return StartCoroutine(ShrinkToOriginal(scaleTarget, shrinkDuration));
        }

        // シーン開始後3秒待つ
        float remainingTime = 3f - 0.5f - shrinkDuration;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        if (appearTarget != null)
        {
            appearTarget.SetActive(true);
        }
    }

    private IEnumerator ShrinkToOriginal(GameObject target, float duration)
    {
        Vector3 startScale = target.transform.localScale;
        Vector3 endScale = originalScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            target.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = endScale;
    }
}
