using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    public float reloadTime = 1f;
    public float fireRate = 0.15f;
    public int magSize = 20;
    public AudioClip ReloadSFX;

    public AudioClip shootingSFX;
    public AudioClip ShellCasingSFX;

    public GameObject bullet;
    public Transform bulletSpawnPoint;

    public float damage = 20f;

    public GameObject weaponFlash;
    public GameObject droppedWeapon;

    public float recoilDistance = 0.1f;
    public float recoilSpeed = 15f;


    public float baseSpread = 1.5f; 
    public float aimSpreadMultiplier = 0.4f; 
    public float moveSpreadMultiplier = 1.6f; 
    public float shotSpreadIncrease = 0.8f;
    public float maxSpread = 8f;
    public float spreadRecoveryRate = 4f;


    public Vector3 rotationOffset = Vector3.zero; 
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 GetPositionOffset => positionOffset;
    public Quaternion GetRotationOffset => Quaternion.Euler(rotationOffset);




    private float currentSpread;
    private Transform playerTransform;
    private Vector3 lastPlayerPos;
    private ADS ads;


    private int currentAmmo;
    public bool isReloading = false;
    private float nextTimeToFire = 0f;

    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 reloadRotationOffset = new Vector3(60, 50, 50);

    private AnimationTrigger animationTrigger;

    public GameObject ShellCasings;
    public Transform ShellEject;
    public float ejectforce = 5f;
    public float rotationalForce = 200f;
    public SlowMotion slowMotion;








    void Start()
    {
        currentAmmo = magSize;

        transform.localPosition += positionOffset;
        transform.localRotation *= Quaternion.Euler(rotationOffset);

        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;

        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        currentSpread = baseSpread;
        ADS found = FindObjectOfType<ADS>();
        if (found != null) ads = found;
        var pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) playerTransform = pm.transform;
        if (playerTransform != null) lastPlayerPos = playerTransform.position;
        slowMotion = GameObject.Find("Player").GetComponent<SlowMotion>();

        animationTrigger = GetComponent<AnimationTrigger>();
        if (animationTrigger == null)
        {
            animationTrigger = GetComponentInParent<AnimationTrigger>();
        }
    }

    public void Shoot()
    {
        if(isReloading) return;
        if(Time.time < nextTimeToFire) return;

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        
         if (animationTrigger != null)
        {
            animationTrigger.TriggerRecoilAnimation();
        }

        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        slowMotion.StopSlowMotion();


        AudioManager.Instance.PlaySFX(shootingSFX, 0.25f);
        AudioManager.Instance.PlaySFX(ShellCasingSFX, 3f);

        Quaternion adjustedRotation;

        if (ads != null && !ads.IsAiming)
        {
        float yaw = Random.Range(-currentSpread, currentSpread);
        float pitch = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRot = Quaternion.Euler(pitch, yaw, 0f);
        adjustedRotation = bulletSpawnPoint.rotation * spreadRot * Quaternion.Euler(-6f, -3f, 0f);
        }
        else
        {
            adjustedRotation = bulletSpawnPoint.rotation;
        }

        Instantiate(bullet, bulletSpawnPoint.position, adjustedRotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        shellcasing();



        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));

        
        currentSpread = Mathf.Min(currentSpread + shotSpreadIncrease, maxSpread);
    }

    private void Update()
    {
        bool isMoving = false;
        if (playerTransform != null)
        {
            Vector3 pos = playerTransform.position;
            float speed = 0f;
            if (Time.deltaTime > 0f)
                speed = (pos - lastPlayerPos).magnitude / Time.deltaTime;
            lastPlayerPos = pos;
            isMoving = speed > 0.1f;
        }

        float target = baseSpread;
        if (ads != null && ads.IsAiming)
            target *= aimSpreadMultiplier;
        if (isMoving)
            target *= moveSpreadMultiplier;

        currentSpread = Mathf.MoveTowards(currentSpread, target, spreadRecoveryRate * Time.deltaTime);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (animationTrigger != null)
        {
            AudioManager.Instance.PlaySFX(ReloadSFX);
            animationTrigger.TriggerReloadAnimation();
        }

        yield return new WaitForSeconds(reloadTime);

    currentAmmo = magSize;
    if (UIManager.Instance != null && UIManager.Instance.ammoText != null)
    {
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
    }
    
    isReloading = false;
}

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Recoil()
    {
        if(ads != null && ads.IsAiming)
            yield break;
            
    }

    public void Drop()
    {
        UIManager.Instance.ammoText.text = "";
        Instantiate(droppedWeapon, transform.position, transform.rotation);
        Destroy(gameObject);
    }

      public void shellcasing()
    {
        if (ShellCasings == null || ShellEject == null) return;

        GameObject newShell = Instantiate(ShellCasings, ShellEject.position, ShellEject.rotation);
        Rigidbody rb = newShell.GetComponent<Rigidbody>();

        if(rb != null)
        {
            rb.AddForce(ShellEject.right * ejectforce, ForceMode.Impulse);
            
            Vector3 randomTorque = new Vector3(
                Random.Range(-rotationalForce, rotationalForce), 
                Random.Range(-rotationalForce, rotationalForce), 
                Random.Range(-rotationalForce, rotationalForce)
            );
            rb.AddTorque(randomTorque);
        }
    }


}