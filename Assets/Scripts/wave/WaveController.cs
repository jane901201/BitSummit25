using UnityEngine;
using UnityEngine.UI; // �� �ǉ�
using System.Collections;

public class WaveController : MonoBehaviour
{
    public Image waveImage; // �\������Image UI
    public Sprite[] waveSprites; // 5��Wave�摜 (waveSprites[0]��Wave1)

    public int maxWave = 5;
    private int currentWave = 4;//TODO:Temp
    private bool isDisplaying = false;

    public TutorialMessageManager tutorialManager;

    private void Start()
    {
        // �ŏ��͔�\��
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

        // �摜���Z�b�g
        if (waveImage != null && waveSprites.Length >= currentWave)
        {
            waveImage.sprite = waveSprites[currentWave - 1];
        }

        yield return StartCoroutine(FadeImageAlpha(0f, 1f, 0.5f)); // �t�F�[�h�C��
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeImageAlpha(1f, 0f, 0.5f)); // �t�F�[�h�A�E�g

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
