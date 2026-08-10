using UnityEngine;
public class AnimationTrigger : MonoBehaviour 
{
    [SerializeField] private Animator GunReload;
    [SerializeField] private Animator GunRecoil;
    [SerializeField] private Animator OpenChest;
    [SerializeField] private string animationStateName = "ARReload";
    [SerializeField] private string animationStateNameRecoil = "GlockRecoil";
    [SerializeField] private string animationStateNameOpenChest = "OpenChest";
    private int reloadHash;
    private int recoilHash;
    private int OpenHash;
    

    private Gun gun; 

    private void Awake() 
    {
        reloadHash = Animator.StringToHash(animationStateName);
        recoilHash = Animator.StringToHash(animationStateNameRecoil);
        OpenHash = Animator.StringToHash(animationStateNameOpenChest);
        gun = GetComponent<Gun>();
    }

    public void TriggerReloadAnimation()
    {
        if (GunReload != null) 
        {
            GunReload.Play(reloadHash, -1, 0f);
        } 
        else 
        {
            Debug.LogWarning("GunReload Animator asset reference is missing!");
        }
    }

    public void TriggerRecoilAnimation()
    {
        if (GunRecoil != null) 
        {
            GunRecoil.Play(recoilHash, -1, 0f);
        } 
        else 
        {
            Debug.LogWarning("GunRecoil Animator asset reference is missing!");
        }
    }

    public void TriggerOpenChesAnimation()
    {
        if (OpenChest != null) 
        {
            OpenChest.Play(OpenHash, -1, 0f);
        } 
        else 
        {
            Debug.LogWarning("OpenChest Animator asset reference is missing!");
        }
    }
}