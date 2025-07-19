using System;
using Controller.PC;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PCController pcController;
    [SerializeField] private GameObject playerPointer;
    [SerializeField] private AccelerometerReader accelerometerReader;
    [SerializeField] private MoveWithAcceleration moveWithAcceleration;
    [SerializeField] private JoyConCube joyConCube;
    [SerializeField] private PlayerInput playerInput;

    private Device device = Device.PC;



    public GameObject PlayerPointer
    {
        get => playerPointer;
        set => playerPointer = value;
    }
    
    public void DeviceSetting(JoystickController joystickController, PCController pcController, AccelerometerReader accelerometerReader
        , MoveWithAcceleration moveWithAcceleration, JoyConCube joyConCube)
    {
        this.joystickController = joystickController;
        this.pcController = pcController;
        //this.accelerometerReader = accelerometerReader;
        //this.moveWithAcceleration = moveWithAcceleration; 
        //this.joyConCube = joyConCube;


        if (device == Device.PC)
        {
            
            joystickController.enabled = false;
            pcController.enabled = true;
            //accelerometerReader.enabled = false;
            //moveWithAcceleration.enabled = false;
            //joyConCube.enabled = false;
        }
        else if (device == Device.JoyStick)
        {
            joystickController.enabled = true;
            pcController.enabled = false;
            //accelerometerReader.enabled = false;
            //moveWithAcceleration.enabled = false;
            //joyConCube.enabled = false;
        }
    }

    private void Update()
    {
        if(joystickController.enabled == true)
        {
            joystickController.enabled = false;
        }
        if (playerPointer == null)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (device == Device.PC)
            {
                joystickController.enabled = true;
                pcController.enabled = false;
                accelerometerReader.enabled false;
                moveWithAcceleration.enabled = false;
                joyConCube.enabled = false;


                device = Device.JoyStick;
                Debug.Log("JoyStick");
            }
            else if(device == Device.JoyStick)
            {
                joystickController.enabled = false;
                pcController.enabled = true;
                accelerometerReader.enabled false;
                moveWithAcceleration.enabled = false;
                joyConCube.enabled = true;

                device = Device.JoyCon;
                Debug.Log("PC");
            }
            else if(device == Device.JoyCon)
            {
                joystickController.enabled = false;
                pcController.enabled = false;
                accelerometerReader.enabled true;
                moveWithAcceleration.enabled = true;
                joyConCube.enabled = false;

                device = Device.Arduino;
                Debug.Log("Arduino");

            }
            else if(device == Device.Arduino)
            {
                joystickController.enabled = false;
                pcController.enabled = true;
                accelerometerReader.enabled false;
                moveWithAcceleration.enabled = false;
                joyConCube.enabled = false;


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
    JoyStick,
    JoyCon,
    Arduino,
}