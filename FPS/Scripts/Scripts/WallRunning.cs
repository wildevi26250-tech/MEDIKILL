using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 20f;
    public float wallClimbSpeed = 5f;
    public float maxWallRunTime = 1.5f;
    private float wallRunTimer;
    private bool wallRunExhausted = false;

    [Header("Wall Jump")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 10f;
    public float wallJumpCooldown = 0.4f;
    private bool canWallRun = true;

    [Header("Detection")]
    public float wallCheckDistance = 0.8f;
    public float minJumpHeight = 1f;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Input")]
    public Key orientationKey = Key.W;
    public Key climbUpKey = Key.LeftShift;
    public Key climbDownKey = Key.LeftCtrl;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, (pm.playerHeight * 0.5f) + minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float verticalInput = pm.GetVerticalMovementInput();

        if (!AboveGround())
        {
            wallRunExhausted = false;
        }

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && canWallRun && !wallRunExhausted)
        {
            if (!pm.wallrunning)
                StartWallRun();

            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                wallRunExhausted = true;
                StopWallRun();
            }

            if (keyboard.spaceKey.wasPressedThisFrame)
                WallJump();
        }
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        float verticalVelocity = 0f;
        if (upwardsRunningState())
            verticalVelocity = wallClimbSpeed;
        else if (downwardsRunningState())
            verticalVelocity = -wallClimbSpeed;

        Vector3 targetVelocity = wallForward * pm.wallrunSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, verticalVelocity, targetVelocity.z);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        rb.AddForce(-wallNormal * 50f, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true;
    }

    private void WallJump()
    {
        canWallRun = false;
        StopWallRun();

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        Invoke(nameof(ResetWallRun), wallJumpCooldown);
    }

    private void ResetWallRun()
    {
        canWallRun = true;
    }

    private bool upwardsRunningState()
    {
        return Keyboard.current != null && keyboardIsPressed(climbUpKey);
    }

    private bool downwardsRunningState()
    {
        return Keyboard.current != null && keyboardIsPressed(climbDownKey);
    }

    private bool keyboardIsPressed(Key key)
    {
        return Keyboard.current[key].isPressed;
    }

    public bool IsWallRunningActive()
    {
        return pm != null && pm.wallrunning;
    }

    public bool IsWallOnRight()
    {
        return wallRight;
    }

    public bool IsWallOnLeft()
    {
        return wallLeft;
    }
}
