using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    public Rigidbody playerRigidbody; 

    public void RestartGame()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
