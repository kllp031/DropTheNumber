using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static System.Action<Vector2> OnClick;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction leftClickAction;

    void Awake()
    {
        leftClickAction = inputActions.FindAction("Click");
        leftClickAction.performed += HandleLeftClick;
    }

    void OnEnable() => leftClickAction?.Enable();
    void OnDisable() => leftClickAction?.Disable();

    private void HandleLeftClick(InputAction.CallbackContext context)
    {
        if (Button.IsPaused) return; 

        Vector2 inputPosition;

        // Check for touch input first
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        // Fallback to mouse input
        else if (Mouse.current != null)
        {
            inputPosition = Mouse.current.position.ReadValue();
        }
        else
        {
            // No valid input device found
            return;
        }

        OnClick?.Invoke(inputPosition);
    }
}
