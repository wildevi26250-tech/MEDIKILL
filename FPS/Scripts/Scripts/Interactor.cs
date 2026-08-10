using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable 
{ 
    void Interact(); 
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractorRange = 5f;

    public UIManager uiManager;

    void Update()
    {
        if (InteractorSource == null) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            int layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractorRange, layerMask))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}
