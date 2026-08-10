using UnityEngine; 

public class SlowMotion2 : MonoBehaviour 
{ 
    public float slowMotionTimescale = 0.5f; 
    public SlowMotion slowMotion; 
    
    private bool hasSlowedTimeAlready2 = false; 
    private bool isDone = false; 

    private float startTimescale; 
    private float startFixedDeltaTime; 

    void Start() 
    { 
        startTimescale = Time.timeScale; 
        startFixedDeltaTime = Time.fixedDeltaTime; 
        if (slowMotion == null)
        {
            Debug.LogError("SlowMotion2: The 'slowMotion' slot is EMPTY ");
        }
    } 

    void Update() 
    { 

        if (isDone) return; 

        if (Input.GetKeyDown(KeyCode.E) && slowMotion.hasSlowedTimeAlready) 
        { 
            if (!hasSlowedTimeAlready2) 
            { 
                StartSlowMotion(); 
            } 
            else 
            { 
                StopSlowMotion(); 
            } 
        } 
    } 

    private void StartSlowMotion() 
    { 
        hasSlowedTimeAlready2 = true; 
        Time.timeScale = slowMotionTimescale; 
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale; 
    } 

    private void StopSlowMotion() 
    { 
        Time.timeScale = startTimescale; 
        Time.fixedDeltaTime = startFixedDeltaTime; 
        
        isDone = true; 
    } 
}
