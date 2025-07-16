using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [Header("スケール変化させるオブジェクト")]
    [SerializeField] private GameObject scaleTarget;      // 最初にスケールアニメをするオブジェクト
    [SerializeField] private float enlargeScale = 3f;     // 最初に大きくする倍率
    [SerializeField] private float shrinkDuration = 1.0f; // 元のサイズに戻るまでの時間

    [Header("一定時間後に表示するオブジェクト")]
    [SerializeField] private GameObject appearTarget;     // 一定時間後に表示するオブジェクト
    [SerializeField] private float appearDelay = 3.0f;    // 表示までの時間（Inspectorから設定）

    [Header("上から下へ動くオブジェクト")]
    [SerializeField] private GameObject moveTarget;       // 動くオブジェクト
    [SerializeField] private Vector3 startPosition;       // 開始位置（上の位置）
    [SerializeField] private Vector3 endPosition;         // 終了位置（下の位置）
    [SerializeField] private float moveDuration = 2.0f;   // 移動時間

    private Vector3 originalScale;

    [SerializeField] private LoadSceneUI loadSceneUI;  // Inspectorでセット

    private void Start()
    {
        if (scaleTarget != null)
        {
            scaleTarget.SetActive(false);
            originalScale = scaleTarget.transform.localScale;
        }

        if (appearTarget != null)
        {
            appearTarget.SetActive(false);
        }

        if (moveTarget != null)
        {
            moveTarget.transform.position = startPosition;
        }

        StartCoroutine(PlayAnimations());
    }

    private IEnumerator PlayAnimations()
    {
        // シーン開始直後から上から下へ移動を同時に開始
        if (moveTarget != null)
        {
            StartCoroutine(MoveFromTopToBottom(moveTarget, startPosition, endPosition, moveDuration));
        }

        // スケールアニメ
        yield return new WaitForSeconds(0.5f);

        if (scaleTarget != null)
        {
            scaleTarget.transform.localScale = originalScale * enlargeScale;
            scaleTarget.SetActive(true);
            yield return StartCoroutine(ShrinkToOriginal(scaleTarget, shrinkDuration));
        }

        float remainingTime = appearDelay - 0.5f - shrinkDuration;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        if (appearTarget != null)
        {
            appearTarget.SetActive(true);

            // LoadSceneUIに「ボタン有効化」を通知
            if (loadSceneUI != null)
            {
                loadSceneUI.IsButtonActive = true;
            }
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

    private IEnumerator MoveFromTopToBottom(GameObject target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            target.transform.position = Vector3.Lerp(from, to, smoothT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.position = to;
    }
}
