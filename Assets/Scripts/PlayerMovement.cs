using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    private Camera _playerCamera;
    private CharacterController _characterController;
    private Vector2 moveInput;

    private void Start()
    {
        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponent<CharacterController>();

        // Subscribe to InputManager events
        InputManager.Instance.OnMoveInput += HandleMoveInput;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= HandleMoveInput;
        }
    }

    private void HandleMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveInput != Vector2.zero)
        {
            // Get camera's forward and right directions (ignore the Y component)
            Vector3 cameraForward = _playerCamera.transform.forward;
            Vector3 cameraRight = _playerCamera.transform.right;

            // Flatten on Y axis for ground movement
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate movement direction relative to camera
            Vector3 moveDirection = (cameraForward * moveInput.y) + (cameraRight * moveInput.x);

            // Apply the movement
            _characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
    }
}
