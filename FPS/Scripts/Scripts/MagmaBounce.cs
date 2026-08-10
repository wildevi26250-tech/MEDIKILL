using UnityEngine;

public class MagmaBounce : MonoBehaviour
{
    [SerializeField] private float bounceForce = 12f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Magma"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 currentVelocity = rb.linearVelocity; 
                currentVelocity.y = bounceForce;
                rb.linearVelocity = currentVelocity;
            }
        }
    }
}