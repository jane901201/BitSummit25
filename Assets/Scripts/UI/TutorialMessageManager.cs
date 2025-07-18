using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使う場合

public class TutorialMessageManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialMessage
    {
        public float triggerTime;       // 発火タイミング
        public string message;          // 日本語セリフ
        public string englishMessage;   // 英語セリフ
        public Sprite faceSprite;       // 表情画像

        public float delayBeforeShow = 0f;   // トリガーされてから何秒後に表示するか
        public float displayDuration = 5f;   // 表示を何秒間維持するか
    }


    public List<TutorialMessage> messages; // Inspectorで設定する
    public Image characterImage; // Inspectorで設定するキャラクターImage


    public RectTransform panel;      // キャラとフキダシをまとめたUIのルート
    public TextMeshProUGUI messageText; // セリフを表示するテキスト
    public TextMeshProUGUI englishMessageText; // Inspectorで設定

    public float slideInDuration = 0.5f; // スライドインにかかる時間
    public float slideOutDuration = 0.5f; // スライドアウトにかかる時間
    public float messageDisplayTime = 5f; // 文字送り完了後に何秒間表示するか
    public float typeSpeed = 0.05f;       // 文字送りの速度（秒）

    private bool isDisplaying = false;
    private float startTime;
    private int currentMessageIndex = 0;

    private static bool hasShownTutorial = false;

    void Start()
    {
        if (TutorialStateManager.Instance != null && TutorialStateManager.Instance.HasShownTutorial)
        {
            Debug.Log("[Tutorial] Already shown. Skipping.");
            this.enabled = false;
            return;
        }

        Debug.Log("[Tutorial] Will show tutorial.");
        startTime = Time.time;
        panel.gameObject.SetActive(false);
    }



    IEnumerator ShowMessage(TutorialMessage tutorialMessage)
    {
        isDisplaying = true;

        // トリガーされてから delayBeforeShow 秒待つ
        if (tutorialMessage.delayBeforeShow > 0f)
        {
            yield return new WaitForSeconds(tutorialMessage.delayBeforeShow);
        }

        characterImage.sprite = tutorialMessage.faceSprite;
        panel.gameObject.SetActive(true);

        Vector2 originalPos = panel.anchoredPosition;
        panel.localScale = Vector3.one;
        panel.anchoredPosition = new Vector2(Screen.width, originalPos.y);

        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);
        englishMessageText.color = new Color(englishMessageText.color.r, englishMessageText.color.g, englishMessageText.color.b, 1f);
        characterImage.color = new Color(characterImage.color.r, characterImage.color.g, characterImage.color.b, 1f);

        // テキスト初期化
        messageText.text = "";
        englishMessageText.text = "";

        // スライドイン
        float time = 0;
        while (time < slideInDuration)
        {
            float t = time / slideInDuration;
            panel.anchoredPosition = Vector2.Lerp(new Vector2(Screen.width, originalPos.y), originalPos, t);
            time += Time.deltaTime;
            yield return null;
        }
        panel.anchoredPosition = originalPos;

        // 日本語・英語メッセージを文字送り（/を改行に）
        string processedMessage = tutorialMessage.message.Replace("/", "\n");
        string processedEnglishMessage = tutorialMessage.englishMessage.Replace("/", "\n");

        for (int i = 0; i < Mathf.Max(processedMessage.Length, processedEnglishMessage.Length); i++)
        {
            if (i < processedMessage.Length)
                messageText.text += processedMessage[i];
            if (i < processedEnglishMessage.Length)
                englishMessageText.text += processedEnglishMessage[i];

            yield return new WaitForSeconds(typeSpeed);
        }

        // 表示を messageDisplayTime の代わりに個別設定の displayDuration 維持
        yield return new WaitForSeconds(tutorialMessage.displayDuration);

        // スライドアウト
        time = 0;
        Vector2 targetPos = new Vector2(Screen.width, originalPos.y);
        Vector3 originalScale = panel.localScale;

        while (time < slideOutDuration)
        {
            float t = time / slideOutDuration;
            panel.anchoredPosition = Vector2.Lerp(originalPos, targetPos, t);
            panel.localScale = Vector3.Lerp(originalScale, originalScale * 0.8f, t);
            time += Time.deltaTime;
            yield return null;
        }

        panel.gameObject.SetActive(false);
        panel.localScale = Vector3.one;
        panel.anchoredPosition = originalPos;

        isDisplaying = false;
    }


    private Coroutine currentCoroutine;

    public void ShowNextMessage()
    {
        if (TutorialStateManager.Instance != null && TutorialStateManager.Instance.HasShownTutorial)
        {
            Debug.Log("[Tutorial] Skipped show next message.");
            return;
        }

        if (currentMessageIndex >= messages.Count)
        {
            TutorialStateManager.Instance?.MarkTutorialShown();
            return;
        }

        if (!isDisplaying && currentMessageIndex < messages.Count)
        {
            currentCoroutine = StartCoroutine(ShowMessage(messages[currentMessageIndex]));
            currentMessageIndex++;
        }
    }



    public static void ResetTutorial()
    {
        hasShownTutorial = false;
    }

}
