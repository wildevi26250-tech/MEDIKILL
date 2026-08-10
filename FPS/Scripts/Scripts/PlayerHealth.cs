using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int health = 100;

    public AudioClip hitSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            DecreaseHealth(10);
        }
        else if(collision.gameObject.tag == "Magma")
        {
            DecreaseHealth(50);
        }
    }

    public void DecreaseHealth(int decreaseAmount)
    {
        health -= decreaseAmount;
        PlayerLook.Instance.Addshake(0.1f, 0.25f);
        UIManager.Instance.InstantiateHitUI();
        AudioManager.Instance.PlaySFX(hitSFX);
        UIManager.Instance.SetHealthValue(health);
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f;
        UIManager.Instance.EnableDeathUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
