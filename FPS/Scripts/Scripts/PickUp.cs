using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Material highlightMaterial;
    private Material[] originalMaterials; 
    private MeshRenderer[] meshRenderers; 

    public GameObject weaponPrefab;
    public float lookRange = 3f;

    private bool isLookedAt = false;
    private PlayerShooting player;
    public SlowMotion slowMotion;
    public Tutorial tutorial;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }

        player = FindObjectOfType<PlayerShooting>();
        if (player != null)
        {
            slowMotion = player.GetComponent<SlowMotion>();
        }
    }

    void Update()
    {
        if (Camera.main == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            if(hit.collider.GetComponentInParent<PickUp>() == this)
            {
                if(!isLookedAt)
                    SetLookedAt(true);

                return;
            }
        }

        if(isLookedAt)
            SetLookedAt(false);
    }

    void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;
        foreach(MeshRenderer mr in meshRenderers)
        {
            if (mr != null) mr.material = lookedAt ? highlightMaterial : originalMaterials[System.Array.IndexOf(meshRenderers, mr)];
        }
    }

    public void OnPickUp()
{
    if(!isLookedAt) return; 

    if (player == null) player = FindObjectOfType<PlayerShooting>();
    if (player == null) return;

    player.OnDrop();

    if(!player.hasCompletedTutorialPick && slowMotion != null)
    {
        slowMotion.StartSlowMotion();
        Debug.Log("slow is start");
    }

    GameObject newWeapon = Instantiate(weaponPrefab, player.gunHolder);
    newWeapon.transform.localPosition = Vector3.zero;
    newWeapon.transform.localRotation = Quaternion.identity;

    Gun weaponGunScript = newWeapon.GetComponent<Gun>();
    if (weaponGunScript == null)
    {
        Debug.LogError("ERROR: broken");
    }

    player.gun = weaponGunScript;
    player.hasCompletedTutorialPick = true;

    if (tutorial == null)
    {
        tutorial = FindObjectOfType<Tutorial>();
    }

    if (tutorial != null)
    {
        if (!tutorial.hasexplainedShoot)
        {
            tutorial.TurnTextOn();
        }
    }
    else
    {
        Debug.LogError("PickUp Error: Could not find Tutorial script in the scene!");
    }

    Destroy(gameObject);
}

}
