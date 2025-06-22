using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Camera _playerCamera;
    private CharacterController _characterController;
    private Vector2 moveInput;
    private float verticalVelocity;

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
        HandleGravity();
    }

    private void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            verticalVelocity = -groundCheckDistance; // Reset vertical velocity when grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity when not grounded
        }
    }

    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        // Handle horizontal movement
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
            movement = moveDirection * movementSpeed;
        }

        // Add vertical movement (gravity/falling)
        movement.y = verticalVelocity;

        // Apply the movement
        _characterController.Move(movement * Time.deltaTime);
    }
}
