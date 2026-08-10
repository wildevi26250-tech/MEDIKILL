using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;
    void Start()
    {
        Destroy(gameObject, time);
    }
}
