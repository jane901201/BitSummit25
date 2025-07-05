using UnityEngine;
using TMPro;
using System.Collections;

public class WaveController : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    public int maxWave = 5;
    private int currentWave = 0;
    private bool isDisplaying = false;

    private void Start()
    {
        // 最初は非表示（Alpha = 0）
        if (waveText != null)
        {
            Color c = waveText.color;
            waveText.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    public void TriggerWave()
    {
        if (!isDisplaying && currentWave < maxWave)
        {
            currentWave++;
            StartCoroutine(ShowWaveText());
        }
    }

    private IEnumerator ShowWaveText()
    {
        isDisplaying = true;

        waveText.text = $"Wave {currentWave}";
        yield return StartCoroutine(FadeTextAlpha(0f, 1f, 0.5f)); // フェードイン
        yield return new WaitForSeconds(1.5f);                    // 通常表示
        yield return StartCoroutine(FadeTextAlpha(1f, 0f, 0.5f)); // フェードアウト

        isDisplaying = false;
    }

    private IEnumerator FadeTextAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = waveText.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            waveText.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        waveText.color = new Color(color.r, color.g, color.b, to);
    }
}
