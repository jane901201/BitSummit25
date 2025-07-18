using System;
using System.Collections;
using Controller.PC;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneUI : MonoBehaviour
{

    [SerializeField] private Button loadSceneButton;
    [SerializeField] private GameObject loadGameSceneButtonSprite;

    [SerializeField] private GameObject loadTitleSceneButtonSprite;

    [SerializeField] private GameObject playerPointer;
    private String gameSceneName = "GameScene ParametaTyousei";
    
    private bool isTriggered = false;

    public bool IsButtonActive { get; set; } = false; // ←追加

    public UIController uiController; // インスペクターで設定

    private void Start()
    {
        if (PhantomSwing.Instance == null)
        {
            Debug.LogError("PhantomSwing.Instance is null");
        }
        else
        {
            PhantomSwing.Instance.PlayerPointer = playerPointer;
            JoystickController joystickController = playerPointer.GetComponent<JoystickController>();
            PCController pcController = playerPointer.GetComponent<PCController>();
            PhantomSwing.Instance.DeviceSetting(joystickController, pcController);
        }
    }

    private void Update()
    {
        if (PhantomSwing.Instance == null) return;
        if (isTriggered) return;

        // ボタンが有効な時だけ判定する
        //if (!IsButtonActive) return;

        if (loadGameSceneButtonSprite == null) return;

        if (PhantomSwing.Instance.CheckVisualOverlaps_Viewport(loadGameSceneButtonSprite))
        {
            isTriggered = true;
            StartCoroutine(LoadSceneTime());
        }
        if (loadTitleSceneButtonSprite == null) return;
        if (PhantomSwing.Instance.CheckVisualOverlaps_Viewport(loadTitleSceneButtonSprite))
        {
            isTriggered = true;
            gameSceneName = "Title";
            StartCoroutine(LoadSceneTime());
        }
    }

    private IEnumerator LoadSceneTime()
    {
        GameObject targetButtonSprite = gameSceneName == "Title" ? loadTitleSceneButtonSprite : loadGameSceneButtonSprite;

        if (uiController != null)
        {
            uiController.ShowUI();
        }

        yield return new WaitForSeconds(0.1f);

        if (targetButtonSprite != null)
        {
            yield return StartCoroutine(ShakeButton(targetButtonSprite, 0.1f, 1));
        }

        if (gameSceneName == "Title")
        {
            if (TutorialStateManager.Instance != null)
                TutorialStateManager.Instance.ResetTutorial();
        }


        PhantomSwing.Instance.LoadGameScene(gameSceneName);
    }


    /// <summary>
    /// ボタンを左右に揺らす
    /// </summary>
    private IEnumerator ShakeButton(GameObject target, float duration, int repeatCount)
    {
        Vector3 originalPosition = target.transform.localPosition;
        float shakeAmount = 0.1f; // 揺れの大きさ（±10pxなど）
        float halfDuration = duration / (repeatCount * 2); // 1往復で2回動くので

        for (int i = 0; i < repeatCount; i++)
        {
            // 左へ
            target.transform.localPosition = originalPosition + Vector3.left * shakeAmount;
            yield return new WaitForSeconds(halfDuration);
            // 右へ
            target.transform.localPosition = originalPosition + Vector3.right * shakeAmount;
            yield return new WaitForSeconds(halfDuration);
        }

        // 元の位置に戻す
        target.transform.localPosition = originalPosition;
    }

}
