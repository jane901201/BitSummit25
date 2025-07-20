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
    [SerializeField] private JoyconManager joyconManager;
    [SerializeField] private JoyconCursorMover joyconCursorMover;
    [SerializeField] private PlayerInput playerInput;

    private Device device = Device.PC;



    public GameObject PlayerPointer
    {
        get => playerPointer;
        set => playerPointer = value;
    }
    
    public void DeviceSetting(JoystickController joystickController, PCController pcController, AccelerometerReader accelerometerReader
        , MoveWithAcceleration moveWithAcceleration, JoyconManager joyconManager, JoyconCursorMover joyconCursorMover)
    {
        this.joystickController = joystickController;
        this.pcController = pcController;
        this.accelerometerReader = accelerometerReader;
        this.moveWithAcceleration = moveWithAcceleration;
        this.joyconManager = joyconManager;
        this.joyconCursorMover = joyconCursorMover;

        if (device == Device.PC)
        {
            
            joystickController.enabled = false;
            pcController.enabled = true;
            accelerometerReader.enabled = false;
            moveWithAcceleration.enabled = false;
            joyconManager.enabled = false;
            joyconCursorMover.enabled = false;
        }
        else if (device == Device.JoyStick)
        {
            joystickController.enabled = true;
            pcController.enabled = false;
            accelerometerReader.enabled = false;
            moveWithAcceleration.enabled = false;
            joyconManager.enabled = false;
            joyconCursorMover.enabled = false;
        }
        else if(device == Device.JoyCon)
        {
            joystickController.enabled = false;
            pcController.enabled = false;
            accelerometerReader.enabled = true;
            moveWithAcceleration.enabled = true;
            joyconManager.enabled = false;
            joyconCursorMover.enabled = false;
        }

    }

    private void Update()
    {
        if (playerPointer == null)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (device == Device.PC)
            {
                joystickController.enabled = false;
                pcController.enabled = false;
                accelerometerReader.enabled = false;
                moveWithAcceleration.enabled = false;
                joyconManager.enabled = true;
                joyconCursorMover.enabled = true;

                device = Device.JoyCon;
                Debug.Log("JoyStick");
            }
            //else if(device == Device.JoyStick)
            //{
            //    joystickController.enabled = false;
            //    pcController.enabled = false;
            //    accelerometerReader.enabled = false;
            //    moveWithAcceleration.enabled = false;
            //    joyconManager.enabled = true;
            //    joyconCursorMover.enabled = true;

            //    device = Device.JoyCon;
            //    Debug.Log("JoyCon");
            //}
            else if(device == Device.JoyCon)
            {
                joystickController.enabled = false;
                pcController.enabled = false;
                accelerometerReader.enabled = true;
                moveWithAcceleration.enabled = true;
                joyconManager.enabled = false;
                joyconCursorMover.enabled = false;

                device = Device.Arduino;
                Debug.Log("Arduino");

            }
            else if(device == Device.Arduino)
            {
                joystickController.enabled = false;
                pcController.enabled = true;
                accelerometerReader.enabled = false;
                moveWithAcceleration.enabled = false;
                joyconManager.enabled = false;
                joyconCursorMover.enabled = false;
                
                device = Device.PC;
                Debug.Log("PC");
            }
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