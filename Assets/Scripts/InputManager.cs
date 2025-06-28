using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Actions")]
    private InputSystem_Actions inputActions;

    // Events that other scripts can subscribe to
    // Movement
    public event Action<Vector2> OnMoveInput;

    // Sprint toggle mode
    public event Action OnSprintInput;

    // Sprint hold mode
    public event Action OnSprintEnded;
    public event Action OnSprintStart;

    // Jump
    public event Action OnJumpInput;

    // Camera
    public event Action<Vector2> OnLookInput;

    // Buttons
    public event Action OnAttackInput;
    public event Action OnInteractInput;
    public event Action OnPreviousInput;
    public event Action OnNextInput;
    public event Action OnDropInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupInputs()
    {
        inputActions = new InputSystem_Actions();

        // Subscribe to input events
        // Movement
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        // Sprinting
        inputActions.Player.Sprint.started += OnSprintStarted;
        inputActions.Player.Sprint.performed += OnSprintPerformed;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;

        // Jumping
        inputActions.Player.Jump.performed += OnJumpPerformed;

        // Looking
        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Look.canceled += OnLookCanceled;

        // Attacking
        inputActions.Player.Attack.performed += OnAttackPerformed;

        // Interacting
        inputActions.Player.Interact.performed += OnInteractPerformed;

        // Scrolling
        inputActions.Player.Previous.performed += OnPreviousPerformed;
        inputActions.Player.Next.performed += OnNextPerformed;

        // Dropping
        inputActions.Player.Drop.performed += OnDropPerformed;
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
    private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(moveInput);
    }

    private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke(Vector2.zero);
    }

    private void OnLookPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        OnLookInput?.Invoke(lookInput);
    }

    private void OnLookCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnLookInput?.Invoke(Vector2.zero);
    }

    private void OnAttackPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnAttackInput?.Invoke();
    }

    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnInteractInput?.Invoke();
    }

    private void OnPreviousPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnPreviousInput?.Invoke();
    }

    private void OnNextPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnNextInput?.Invoke();
    }

    private void OnDropPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnDropInput?.Invoke();
    }

    private void OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnSprintInput?.Invoke();
    }

    private void OnSprintStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnSprintStart?.Invoke();
    }

    private void OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnSprintEnded?.Invoke();
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke();
    }
}
