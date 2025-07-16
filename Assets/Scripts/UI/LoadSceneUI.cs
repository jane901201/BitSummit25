using System;
using System.Collections;
using Controller.PC;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneUI : MonoBehaviour
{

    [SerializeField] private Button loadSceneButton;
    [SerializeField] private GameObject loadSceneButtonSprite;
    [SerializeField] private GameObject playerPointer;
    [SerializeField] private String gameSceneName = "GameScene ParametaTyousei";
    
    private bool isTriggered = false;
    
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
        if(PhantomSwing.Instance == null)
            return;
        if(isTriggered)
            return;
        if (PhantomSwing.Instance.CheckVisualOverlaps_Viewport(loadSceneButtonSprite))
        {
            isTriggered = true;
            StartCoroutine(LoadSceneTime());
        }
    }

    private IEnumerator LoadSceneTime()
    {
        yield return new WaitForSeconds(0.2f);
        PhantomSwing.Instance.LoadGameScene(gameSceneName);
    }
}
