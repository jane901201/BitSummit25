using UnityEngine;
using UnityEngine.UI; // ← 追加
using System.Collections;

public class WaveController : MonoBehaviour
{
    public Image waveImage; // 表示するImage UI
    public Sprite[] waveSprites; // 5つのWave画像 (waveSprites[0]がWave1)

    public int maxWave = 5;
    private int currentWave = 0;
    private bool isDisplaying = false;

    public TutorialMessageManager tutorialManager;

    private void Start()
    {
        // 最初は非表示
        if (waveImage != null)
        {
            Color c = waveImage.color;
            waveImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    public void TriggerWave()
    {
        if (!isDisplaying && currentWave < maxWave)
        {
            currentWave++;

            GameManager.Instance?.SetCurrentWave(currentWave);
            tutorialManager?.ShowNextMessage();

            StartCoroutine(ShowWaveImage());
        }
    }

    private IEnumerator ShowWaveImage()
    {
        isDisplaying = true;

        // 画像をセット
        if (waveImage != null && waveSprites.Length >= currentWave)
        {
            waveImage.sprite = waveSprites[currentWave - 1];
        }

        yield return StartCoroutine(FadeImageAlpha(0f, 1f, 0.5f)); // フェードイン
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeImageAlpha(1f, 0f, 0.5f)); // フェードアウト

        isDisplaying = false;
    }

    private IEnumerator FadeImageAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = waveImage.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            waveImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        waveImage.color = new Color(color.r, color.g, color.b, to);
    }
}
