using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Weaponbobsway : MonoBehaviour
{
    [SerializeField] PlayerMovement keyboardInputs;
    [SerializeField] PlayerLook mouseInput;
    [SerializeField] Rigidbody rb;

    [Header("Settings")]
    public bool sway = true;
    public bool swayRotation = true;
    public bool bobOffset = true;
    public bool bobSway = true;

    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;

    public ADS ads;
   
    void Start()
    {
        originLocalPosition = transform.localPosition;
        originLocalRotation = transform.localRotation;
    }

    void Update()
    {
        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput()
    {
        if (keyboardInputs != null)
        {
            walkInput.x = keyboardInputs.GetHorizontalMovementInput();
            walkInput.y = keyboardInputs.GetVerticalMovementInput();
            walkInput = walkInput.normalized;
        }

        if (mouseInput != null)
        {
            lookInput.x = mouseInput.GetHorizontalCameraInput();
            lookInput.y = mouseInput.GetVerticalCameraInput();
        }
    }

    [Header("Sway")]
    public float step = 1.5f;
    public float maxStepDistance = 0.05f;
    Vector3 swayPos;

    void Sway()
    {
        if (sway == false ) { swayPos = Vector3.zero; return; }
        
        Vector3 invertLook = lookInput * -step * 0.01f;
        
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    [Header("Sway Rotation")]
    public float rotationStep = 2.5f;
    public float maxStepRotation = 4f;
    Vector3 swayEulerRot;

    void SwayRotation()
    {
        if (swayRotation == false ) { swayEulerRot = Vector3.zero; return; }

        Vector2 invertLook = lookInput * -rotationStep * 0.01f;
        
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepRotation, maxStepRotation);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepRotation, maxStepRotation);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, -invertLook.x * 0.5f);
    }

    float smooth = 12f;
    float smoothRot = 10f;

    void CompositePositionRotation()
    {
        Vector3 targetPosition = originLocalPosition + swayPos + bobPosition;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * smooth);

        Quaternion targetRotation = originLocalRotation * Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation);
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * smoothRot
        );
    }

    [Header("Bobbing System")]
    public float idleBobSpeed = 1.5f;
    public Vector3 idleBobLimit = new Vector3(0.005f, 0.005f, 0f);
    public float walkBobSpeed = 12f;
    public Vector3 walkBobLimit = new Vector3(0.025f, 0.02f, 0.01f);
    [Range(0f, 1f)] public float adsBobMultiplier = 0.15f;

    private float bobTimer;
    Vector3 bobPosition;

    void BobOffset()
    {
        if (bobOffset == false) { bobPosition = Vector3.zero; return; }

        bool isMoving = walkInput.magnitude > 0.1f;
        float speedFactor = isMoving ? walkBobSpeed : idleBobSpeed;
        
        if (isMoving && rb != null)
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVelocity.magnitude > 0.1f)
            {
                speedFactor *= (flatVelocity.magnitude / 5f);
            }
        }

        bobTimer += Time.deltaTime * speedFactor;

        Vector3 currentLimit = isMoving ? walkBobLimit : idleBobLimit;
        if (ads != null && ads.IsAiming)
        {
            currentLimit *= adsBobMultiplier;
        }

        float bobX = Mathf.Cos(bobTimer * 0.5f) * currentLimit.x;
        float bobY = Mathf.Sin(bobTimer) * currentLimit.y;
        float bobZ = isMoving ? (-walkInput.y * currentLimit.z) : 0f;

        bobPosition = new Vector3(bobX, bobY, bobZ);
    }

    [Header("Bob Rotation")]
    public Vector3 multiplier = new Vector3(1.5f, 1.5f, 2f);
    Vector3 bobEulerRotation;

    void BobRotation()
    {
        bool isMoving = walkInput.magnitude > 0.1f;
        if (bobSway == false || !isMoving) { bobEulerRotation = Vector3.zero; return; }

        Vector3 currentMultiplier = multiplier;
        if (ads != null && ads.IsAiming)
        {
            currentMultiplier *= adsBobMultiplier;
        }

        bobEulerRotation.x = Mathf.Sin(bobTimer) * currentMultiplier.x;
        bobEulerRotation.y = Mathf.Cos(bobTimer * 0.5f) * currentMultiplier.y * walkInput.x;
        bobEulerRotation.z = Mathf.Cos(bobTimer * 0.5f) * currentMultiplier.z * -walkInput.x;
    }
}
