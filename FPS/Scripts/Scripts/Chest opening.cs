using UnityEngine;

public class Chestopening : MonoBehaviour, IInteractable 
{ 
    private AnimationTrigger animationTrigger; 
    private bool hasOpened = false;

    private void Awake()
    {
        animationTrigger = GetComponent<AnimationTrigger>();
    }

    public void Interact() 
    {
        if (hasOpened) return;
        if (animationTrigger != null)
        {
            animationTrigger.TriggerOpenChesAnimation();
            hasOpened = true; 
        }
    } 
}
