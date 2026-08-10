using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject HitUI;

    public GameObject deathUI;

    public TextMeshProUGUI ammoText;

    public Image healthBar;
    public Gradient healthGradient;
    public TextMeshProUGUI E_To_Open;

    private void Awake()
    {
        Time.timeScale = 1f;

        Instance = this;
    }

    public void InstantiateHitUI()
    {
        Instantiate(HitUI, transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
    }

    public void EnableE_To_Open()
    {
        E_To_Open.gameObject.SetActive(true);
    }


    public void SetHealthValue(int health)
    {
        float floatHealth = (float)health / 100;
        healthBar.color = healthGradient.Evaluate(floatHealth);
        healthBar.fillAmount = floatHealth;
    }
}