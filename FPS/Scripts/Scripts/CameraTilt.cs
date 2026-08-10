using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    
    [Header("Tilt Settings")]
    public float tiltAngle = 15f;
    public float tiltSpeed = 10f;

    private float targetTilt;

    private void Update()
    {
        CheckWallTilt();
        ApplyTilt();
    }

    private void CheckWallTilt()
    {
        RaycastHit hit;
        
        bool wallRight = Physics.Raycast(orientation.position, orientation.right, out hit, 1.2f);
        bool wallLeft = Physics.Raycast(orientation.position, -orientation.right, out hit, 1.2f);

        if (wallRight && !Physics.Raycast(orientation.position, Vector3.down, 1.5f))
        {
            targetTilt = tiltAngle;
        }
        else if (wallLeft && !Physics.Raycast(orientation.position, Vector3.down, 1.5f))
        {
            targetTilt = -tiltAngle;
        }
        else
        {
            targetTilt = 0f;
        }
    }

    private void ApplyTilt()
    {
        float currentTilt = Mathf.LerpAngle(transform.localEulerAngles.z, targetTilt, tiltSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTilt);
    }
}
