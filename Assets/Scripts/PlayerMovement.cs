using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeedBonus = 5f;
    [SerializeField] private bool sprintButtonToggle = false;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 8f;
    [SerializeField] private float jumpCooldown = 0.1f;
    [SerializeField] private bool allowAirControl = true;
    [SerializeField] private float airControlMultiplier = 0.5f;

    [Header("Slope Handling")]
    [SerializeField] private float slopeForce = 10f;
    [SerializeField] private float slopeRayLength = 1.5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Camera _playerCamera;
    private CharacterController _characterController;
    private Vector2 moveInput;
    private float verticalVelocity;

    // Sprint variables
    private bool isSprinting = false;
    private float currentSpeed;

    // Jump variables
    private bool canJump = true;
    private float lastJumpTime;

    private void Start()
    {
        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponent<CharacterController>();

        // Initialize current speed
        currentSpeed = movementSpeed;

        // Subscribe to InputManager events
        InputManager.Instance.OnMoveInput += HandleMoveInput;
        InputManager.Instance.OnJumpInput += HandleJumpInput;

        // Subscribe to different sprint events based on mode
        if (sprintButtonToggle)
        {
            InputManager.Instance.OnSprintInput += HandleSprinting;
        } 
        else
        {
            InputManager.Instance.OnSprintStart += HandleSprintStarted;
            InputManager.Instance.OnSprintEnded += HandleSprintEnded;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= HandleMoveInput;
            InputManager.Instance.OnJumpInput -= HandleJumpInput;

            if (sprintButtonToggle)
            {
                InputManager.Instance.OnSprintInput -= HandleSprinting;
            }
            else
            {
                InputManager.Instance.OnSprintStart -= HandleSprintStarted;
                InputManager.Instance.OnSprintEnded -= HandleSprintEnded;
            }
        }
    }

    private void HandleMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleJumpInput()
    {
        Debug.Log($"is grounded: {_characterController.isGrounded}");

        // Check if we can jump (grounded & cooldown passed
        if (_characterController.isGrounded && canJump && Time.time - lastJumpTime > jumpCooldown)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // Calculate jump velocity
        verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravity));
        lastJumpTime = Time.time;
        canJump = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        UpdateJumpCooldown();
    }

    private void UpdateJumpCooldown()
    {
        if (_characterController.isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            canJump = true;
        }
    }

    private void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            // Small downwards force to keep grounded
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Use a slightly larger value than groundCheckDistance
            }

            // Apply additional slope force if moving on a slope
            if (IsOnSlope() && moveInput != Vector2.zero)
            {
                verticalVelocity = -slopeForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity when not grounded
        }
    }

    private bool IsOnSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeRayLength))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > 0.1f && angle < _characterController.slopeLimit;
        }

        return false;
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
            movement = moveDirection * currentSpeed;

            // Apply air control if player is not grounded
            float speedMultiplier = 1f;
            if (!_characterController.isGrounded && allowAirControl)
            {
                speedMultiplier = airControlMultiplier;
            }

            movement = moveDirection * currentSpeed * speedMultiplier;
        }

        // Add vertical movement (gravity/falling)
        movement.y = verticalVelocity;

        // Apply the movement
        _characterController.Move(movement * Time.deltaTime);
    }

    private void HandleSprinting()
    {
        isSprinting = !isSprinting;

        if (isSprinting )
        {
            currentSpeed = movementSpeed + sprintSpeedBonus;
        }
        else
        {
            currentSpeed = movementSpeed;
        }
    }

    private void HandleSprintStarted()
    {
        isSprinting = true;
        currentSpeed = movementSpeed + sprintSpeedBonus;
    }

    private void HandleSprintEnded()
    {
        isSprinting = false;
        currentSpeed = movementSpeed;
    }
}
