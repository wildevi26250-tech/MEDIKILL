using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        private NetworkManager m_NetworkManager;

        [Header("Spawn Points")]
        public Transform[] spawnPoints;

        [Header("Player Prefab")]
        public GameObject playerPrefab;

        private void Awake()
        {
            m_NetworkManager = GetComponent<NetworkManager>();
        }

        private void Start()
        {
            if (m_NetworkManager == null)
            {
                m_NetworkManager = FindObjectOfType<NetworkManager>();
            }

            if (m_NetworkManager != null)
            {
                m_NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                if (m_NetworkManager.NetworkConfig.PlayerPrefab != null)
                {
                    m_NetworkManager.NetworkConfig.PlayerPrefab = null;
                }
            }
        }

        private void OnDestroy()
        {
            if (m_NetworkManager != null)
            {
                m_NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }
            GUILayout.EndArea();
        }

        private void StartButtons()
        {
            if (GUILayout.Button("Host"))
                m_NetworkManager.StartHost();
            if (GUILayout.Button("Client"))
                m_NetworkManager.StartClient();
            if (GUILayout.Button("Server"))
                m_NetworkManager.StartServer();
        }

        private void StatusLabels()
        {
            var mode = m_NetworkManager.IsHost ? "Host" : m_NetworkManager.IsServer ? "Server" : "Client";
            GUILayout.Label("Transport: " + m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            if (!m_NetworkManager.IsServer) return;

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                spawnPoints = new Transform[] { null };
            }

            int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);
            Vector3 spawnPosition = spawnPoints[spawnIndex] != null ? spawnPoints[spawnIndex].position : Vector3.zero;
            Quaternion spawnRotation = spawnPoints[spawnIndex] != null ? spawnPoints[spawnIndex].rotation : Quaternion.identity;

            if (playerPrefab == null)
            {
                return;
            }

            if (m_NetworkManager.ConnectedClients.ContainsKey(clientId) && 
                m_NetworkManager.ConnectedClients[clientId].PlayerObject != null)
            {
                return;
            }

            GameObject playerObject = Instantiate(playerPrefab, spawnPosition, spawnRotation);
            
            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                networkObject = playerObject.AddComponent<NetworkObject>();
            }

            networkObject.SpawnAsPlayerObject(clientId, true);
        }
    }
}