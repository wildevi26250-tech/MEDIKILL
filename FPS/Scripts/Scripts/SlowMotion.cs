using UnityEngine;
using UnityEngine.InputSystem;

public class SlowMotion : MonoBehaviour 
{
    public Tutorial tutorial;
    public float slowMotionTimescale = 0.1f; 

    private float startTimescale = 1f;
    private float startFixedDeltaTime = 0.02f;
    private bool isTimeSlowed = false;
    public InputActionReference dismissTutorialAction;
    public bool hasSlowedTimeAlready = false;
    public PickUp pickUp;

    void Start() 
    {
        if (Time.timeScale > slowMotionTimescale)
        {
            startTimescale = Time.timeScale;
            startFixedDeltaTime = Time.fixedDeltaTime;
        }
    }

    void OnEnable()
    {
        if (dismissTutorialAction != null) dismissTutorialAction.action.Enable();
    }

    void OnDisable()
    {
        if (dismissTutorialAction != null) dismissTutorialAction.action.Disable();
    }

    void Update()
    {
    bool eKeyPressed = dismissTutorialAction != null && dismissTutorialAction.action.WasPressedThisFrame();
    
    if (eKeyPressed && isTimeSlowed) {
        StopSlowMotion();
    }

    if (tutorial != null && tutorial.tutorialText != null) {
        if (tutorial.tutorialText.gameObject.activeSelf && !isTimeSlowed && !hasSlowedTimeAlready) {
            StartSlowMotion();
        }
    }
    }


    public void StartSlowMotion() 
    {
        isTimeSlowed = true;
        hasSlowedTimeAlready = true;
        Time.timeScale = slowMotionTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale;
    }

    public void StopSlowMotion() 
    {
        isTimeSlowed = false;
        Time.timeScale = startTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime;
        
        if (tutorial != null && tutorial.tutorialText != null)
        {
            tutorial.tutorialText.gameObject.SetActive(false); 
        }
    }
    public bool IsTimeSlowed()
    {
        return isTimeSlowed;
    }
}
