using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Looking")]
    [SerializeField] private float _cameraSens = 5f;
    [SerializeField] private float _verticalClampMin = -90f;
    [SerializeField] private float _verticalClampMax = 90f;

    // Camera
    private Camera _playerCamera;

    private Vector2 _mouseInput;
    private float _horizontalRotation;
    private float _verticalRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerCamera = GetComponentInChildren<Camera>();

        // Subscribe to InputManager events
        InputManager.Instance.OnLookInput += HandleLookInput;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLookInput -= HandleLookInput;
        }
    }

    private void HandleLookInput(Vector2 input)
    {
        _mouseInput = input;
    }

    private void Update()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        if (_mouseInput != Vector2.zero)
        {
            Vector2 scaledInput = _mouseInput * _cameraSens * Time.deltaTime;
            _horizontalRotation += scaledInput.x;
            _verticalRotation -= scaledInput.y;
            _verticalRotation = Mathf.Clamp(_verticalRotation, _verticalClampMin, _verticalClampMax);

            transform.rotation = Quaternion.Euler(0, _horizontalRotation, 0);
            _playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
            
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _mouseInput = Vector2.zero;
    }
}
