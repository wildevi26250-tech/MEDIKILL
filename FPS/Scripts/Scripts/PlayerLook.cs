using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public static PlayerLook Instance;

    public float mouseSensitivity = 200f;
    public Transform cam;

    [Header("Wallrun Tilt Settings")]
    public WallRunning wallRunScript;
    public float tiltAngle = 15f;
    public float tiltSpeed = 10f;
    private float currentTilt = 0f;

    private float xRotation = 0f;
    private Vector2 lookInput;

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float shakeFadeSpeed = 1.5f;
    private Vector3 initialCamPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (cam == null)
        {
            Camera camComponent = GetComponentInChildren<Camera>();
            if (camComponent != null)
            {
                cam = camComponent.transform;
            }
        }

        if (cam != null)
        {
            initialCamPos = cam.localPosition;
        }
    }

    private void Update()
    {
        if (!IsOwner()) return;

        HandleMouseLook();
        HandleShake();
    }

    private bool IsOwner()
    {
        if (TryGetComponent<Unity.Netcode.NetworkObject>(out var netObj))
        {
            return netObj.IsOwner;
        }
        return false;
    }

    public void OnLook(InputValue value)
    {
        if (!IsOwner()) return;
        lookInput = value.Get<Vector2>();
    }

    private void HandleMouseLook()
    {
        if (cam == null) return;

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        float targetTilt = 0f;
        if (wallRunScript != null && wallRunScript.IsWallRunningActive())
        {
            if (wallRunScript.IsWallOnRight())
                targetTilt = tiltAngle;
            else if (wallRunScript.IsWallOnLeft())
                targetTilt = -tiltAngle;
        }

        currentTilt = Mathf.LerpAngle(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);
        
        
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleShake()
    {
        if (cam == null) return;

        if (shakeDuration > 0)
        {
            cam.localPosition = initialCamPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * shakeFadeSpeed;
        }
        else
        {
            cam.localPosition = initialCamPos;
        }
    }

    public void Addshake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    public float GetHorizontalCameraInput()
    {
        return lookInput.x;
    }

    public float GetVerticalCameraInput()
    {
        return lookInput.y;
    }
}