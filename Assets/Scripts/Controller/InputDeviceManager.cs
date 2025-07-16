using System;
using Controller.PC;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PCController pcController;
    [SerializeField] private GameObject playerPointer;
    [SerializeField] private PlayerInput playerInput;

    private Device device = Device.PC;

    public GameObject PlayerPointer
    {
        get => playerPointer;
        set => playerPointer = value;
    }
    
    public void DeviceSetting(JoystickController joystickController, PCController pcController)
    {
        this.joystickController = joystickController;
        this.pcController = pcController;
        if (device == Device.PC)
        {
            
            joystickController.enabled = false;
            pcController.enabled = true;
        }
        else if (device == Device.JoyStick)
        {
            joystickController.enabled = true;
            pcController.enabled = false;
        }
    }

    private void Update()
    {
        if(playerPointer == null)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (device == Device.PC)
            {
                joystickController.enabled = true;
                pcController.enabled = false;
                device = Device.JoyStick;
                Debug.Log("JoyStick");
            }
            else if(device == Device.JoyStick)
            {
                joystickController.enabled = false;
                pcController.enabled = true;
                device = Device.PC;
                Debug.Log("PC");
            }
        }
    }
    
    private void UpdateControlScheme(string scheme)
    {
        // スキーム名に応じて切り替え
        if (scheme == "Keyboard&Mouse" || scheme == "Keyboard") // キーマウス用
        {
            pcController.enabled = true;
            joystickController.enabled = false;
        }
        else if (scheme == "Gamepad" || scheme == "Joystick") // ゲームパッド用
        {
            pcController.enabled = false;
            joystickController.enabled = true;
        }
        else
        {
            // 不明な場合は両方無効化など適宜処理
            pcController.enabled = false;
            joystickController.enabled = false;
        }
    }
}

public enum Device
{
    PC,
    JoyStick
}