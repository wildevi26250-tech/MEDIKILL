using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float wallrunSpeed = 10f;
    private float moveSpeed;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed = 2.5f;
    public float crouchYScale = 0.5f;
    private float startYScale;
    private bool isCrouching = false;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public float groundDistance = 0.4f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Audio")]
    public AudioClip footstepSFX;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    [Header("Input")]
    public Key forwardKey = Key.W;
    public Key backwardKey = Key.S;
    public Key rightKey = Key.D;
    public Key leftKey = Key.A;
    public Key jumpKey = Key.Space;
    public Key sprintKey = Key.LeftShift;
    public Key crouchKey = Key.LeftCtrl;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    public MovementState state;

    public enum MovementState { walking, sprinting, wallrunning, air, crouching }

    [HideInInspector]
    public bool wallrunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
        moveSpeed = walkSpeed;

        if (orientation == null)
        {
            orientation = transform;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        ReadKeyboardDirectly();
        CheckGround();
        StateHandler();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        MovePlayer();
    }

    private void ReadKeyboardDirectly()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float x = 0f;
        float y = 0f;

        if (keyboard[forwardKey].isPressed) y += 1f;
        if (keyboard[backwardKey].isPressed) y -= 1f;
        if (keyboard[rightKey].isPressed) x += 1f;
        if (keyboard[leftKey].isPressed) x -= 1f;

        moveInput = new Vector2(x, y);

        if (keyboard[jumpKey].wasPressedThisFrame && readyToJump)
        {
            if (grounded)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        if (keyboard[crouchKey].wasPressedThisFrame && !isCrouching && !wallrunning)
        {
            isCrouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else if (keyboard[crouchKey].wasReleasedThisFrame && isCrouching)
        {
            isCrouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void CheckGround()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
            return;
        }

        var keyboard = Keyboard.current;
        bool sprintPressed = keyboard != null && keyboard[sprintKey].isPressed;

        if (isCrouching)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && sprintPressed && moveInput.magnitude > 0.1f)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
            moveSpeed = walkSpeed;
        }
    }

    private void MovePlayer()
    {
        if (wallrunning) return;

        Transform referenceTransform = orientation != null ? orientation : transform;
        moveDirection = referenceTransform.forward * moveInput.y + referenceTransform.right * moveInput.x;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        if (OnSlope() && !exitingSlope)
        {
            Vector3 slopeDir = GetSlopeMoveDirection();
            rb.linearVelocity = new Vector3(slopeDir.x * moveSpeed * moveInput.magnitude, rb.linearVelocity.y, slopeDir.z * moveSpeed * moveInput.magnitude);
        }
        else
        {
            rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public float GetHorizontalMovementInput()
    {
        return moveInput.x;
    }

    public float GetVerticalMovementInput()
    {
        return moveInput.y;
    }
}