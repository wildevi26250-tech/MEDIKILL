using Unity.Netcode;
using UnityEngine;

public class NetworkCameraFollow : NetworkBehaviour
{
    public Transform targetPlayer;
    public Vector3 offset = new Vector3(0f, 2f, -5f);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Camera cam = GetComponent<Camera>();
            if (cam != null) cam.enabled = false;

            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner || targetPlayer == null) return;

        transform.position = targetPlayer.position + offset;
    }
}
