using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    public Camera playerCamera;

    private void Start()
    {
        if (playerCamera == null)
        {
            GameObject camObj = new GameObject("PlayerCamera");
            playerCamera = camObj.AddComponent<Camera>();
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0, 1.0f, 0);
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        if (!IsOwner)
        {
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
            }
        }
        else
        {
            if (playerCamera != null)
            {
                playerCamera.enabled = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;
    }
}