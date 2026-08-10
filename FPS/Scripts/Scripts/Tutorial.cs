using UnityEngine; 
using TMPro; 
using UnityEngine.InputSystem; 

public class Tutorial : MonoBehaviour 
{ 
    public TextMeshProUGUI tutorialText; 
    public float interactDistance = 5f; 
    public LayerMask InteractableLayer; 
    public LayerMask FinalTargetLayer; 
    public SlowMotion slowMotion; 
    public bool hasexplainedShoot = false; 
    public TextMeshProUGUI tutorialText2; 
    public TextMeshProUGUI tutorialTextFinal; 
    public float finalDisplayTime = 1f; 
    public LayerMask WallrunLayer; 
    private bool finalTutorialStarted = false; 
    public TextMeshProUGUI tutorialtextwallrun; 
    public bool wallrunExplained; 

    public PlayerMovement playerMovement;

    void Start() 
    { 
        if (tutorialText != null) tutorialText.gameObject.SetActive(false); 
        if (tutorialText2 != null) tutorialText2.gameObject.SetActive(false); 
        if (tutorialtextwallrun != null) tutorialtextwallrun.gameObject.SetActive(false); 
    } 

    void Update() 
    { 
        Ray ray = new Ray(transform.position, transform.forward); 
        RaycastHit hit; 

        if (Physics.Raycast(ray, out hit, interactDistance, FinalTargetLayer)) 
        { 
            FinalTutorial(); 
        } 
        else if (Physics.Raycast(ray, out hit, interactDistance, InteractableLayer)) 
        { 
            if (tutorialText != null && !slowMotion.hasSlowedTimeAlready) 
                tutorialText.gameObject.SetActive(true); 
        } 
        else if (Input.GetKeyDown(KeyCode.E)) 
        { 
            if (tutorialText != null) tutorialText.gameObject.SetActive(false); 
        } 

        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        { 
            if (tutorialText2 != null) tutorialText2.gameObject.SetActive(false); 
        } 
        else if (Physics.Raycast(ray, out hit, interactDistance, WallrunLayer)) 
        { 
            WallrunTutorial(); 
        } 

        if (wallrunExplained && tutorialtextwallrun != null && tutorialtextwallrun.gameObject.activeSelf)
        {
            killText();
        }
    } 

    public void TurnTextOn() 
    { 
        if (tutorialText2 != null && !hasexplainedShoot) 
        { 
            tutorialText2.gameObject.SetActive(true); 
            hasexplainedShoot = true; 
        } 
    } 

    public void FinalTutorial() 
    { 
        if (hasexplainedShoot && !finalTutorialStarted) 
        { 
            finalTutorialStarted = true; 
            slowMotion.StartSlowMotion(); 
            if (tutorialTextFinal != null) 
            { 
                tutorialTextFinal.gameObject.SetActive(true); 
                Invoke("EndTutorialSequence", finalDisplayTime); 
            } 
        } 
    } 

    private void EndTutorialSequence() 
    { 
        if (tutorialTextFinal != null) tutorialTextFinal.gameObject.SetActive(false); 
        if (slowMotion != null) slowMotion.StopSlowMotion(); 
    } 

    public void WallrunTutorial() 
    { 
        if (hasexplainedShoot && finalTutorialStarted && !wallrunExplained) 
        { 
            wallrunExplained = true; 
            slowMotion.StartSlowMotion(); 
            if (tutorialtextwallrun != null) 
            { 
                tutorialtextwallrun.gameObject.SetActive(true); 
            } 
        } 
    } 

    public void killText()
    {
        if (playerMovement != null && playerMovement.state == PlayerMovement.MovementState.wallrunning)
        {
            tutorialtextwallrun.gameObject.SetActive(false);
            if (slowMotion != null) slowMotion.StopSlowMotion(); 
        }
    }
}
