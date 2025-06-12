using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs")]
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"attack triggered - Phase: {context.phase}, Control: {context.control.name}");
    }
}
