using UnityEngine;
using System.Collections;

public class BobbingMotion : MonoBehaviour
{
    public float moveAmount = 0.5f;
    public float moveDuration = 0.5f;
    public float pauseDuration = 1f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(BobLoop());
    }

    IEnumerator BobLoop()
    {
        while (true)
        {
            // è„Ç÷
            Vector3 targetUp = originalPosition + Vector3.up * moveAmount;
            yield return MoveTo(targetUp, moveDuration);

            // â∫Ç÷
            yield return MoveTo(originalPosition, moveDuration);

            // àÍéûí‚é~
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}
