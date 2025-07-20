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

    public bool IsButtonActive { get; set; } = false; // ���ǉ�

    public UIController uiController; // �C���X�y�N�^�[�Őݒ�

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
            AccelerometerReader accelerometerReader = playerPointer.GetComponent<AccelerometerReader>();
            MoveWithAcceleration moveWithAcceleration = playerPointer.GetComponent<MoveWithAcceleration>();
            JoyconManager joyconManager = playerPointer.GetComponent<JoyconManager>();
            JoyconCursorMover joyconCursorMover = playerPointer.GetComponent<JoyconCursorMover>();
            
            PhantomSwing.Instance.DeviceSetting(joystickController, pcController, accelerometerReader, moveWithAcceleration, joyconManager, joyconCursorMover);
        }
    }

    private void Update()
    {
        if (PhantomSwing.Instance == null) return;
        if (isTriggered) return;

        // �{�^�����L���Ȏ��������肷��
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
            TutorialStateManager.Instance?.ResetTutorial();
        }


        PhantomSwing.Instance.LoadGameScene(gameSceneName);
    }


    /// <summary>
    /// �{�^�������E�ɗh�炷
    /// </summary>
    private IEnumerator ShakeButton(GameObject target, float duration, int repeatCount)
    {
        Vector3 originalPosition = target.transform.localPosition;
        float shakeAmount = 0.1f; // �h��̑傫���i�}10px�Ȃǁj
        float halfDuration = duration / (repeatCount * 2); // 1������2�񓮂��̂�

        for (int i = 0; i < repeatCount; i++)
        {
            // ����
            target.transform.localPosition = originalPosition + Vector3.left * shakeAmount;
            yield return new WaitForSeconds(halfDuration);
            // �E��
            target.transform.localPosition = originalPosition + Vector3.right * shakeAmount;
            yield return new WaitForSeconds(halfDuration);
        }

        // ���̈ʒu�ɖ߂�
        target.transform.localPosition = originalPosition;
    }

}
