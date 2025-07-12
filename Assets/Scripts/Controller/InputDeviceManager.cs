using Controller.PC;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PCController pcController;
    [SerializeField] private PlayerInput playerInput;

    private void OnEnable()
    {
        playerInput.onControlsChanged += OnControlsChanged;
        UpdateControlScheme(playerInput.currentControlScheme); // 初期化時のSchemeを反映
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        UpdateControlScheme(input.currentControlScheme);
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
