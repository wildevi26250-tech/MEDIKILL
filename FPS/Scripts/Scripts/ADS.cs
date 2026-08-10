using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

public class ADS : MonoBehaviour
{
    public Transform gunTransform;
    public Transform DefaultPos;
    public Transform ADSPos;
    public float ADSspeed = 15f;
    public bool isAiming = false;
    public bool IsAiming => isAiming;

    private PlayerShooting playerShooting;
    public Gun gun;
    public SlowMotion slowMotion;
    public PlayerMovement playerMovement;

    private Coroutine slowMoDurationCoroutine;

    private void Awake()
    {
        playerShooting = FindObjectOfType<PlayerShooting>();
        UpdateGunTransform();
    }

    private void Update()
    {
        UpdateGunTransform();

        if (gunTransform == null || DefaultPos == null || ADSPos == null) return;

        Vector3 gunOffset = Vector3.zero;
        if (playerShooting != null && playerShooting.gun != null)
        {
            gunOffset = playerShooting.gun.GetPositionOffset;
        }

        Vector3 targetLocal;
        if (isAiming)
        {
            targetLocal = ADSPos.localPosition + gunOffset;
        }
        else
        {
            targetLocal = DefaultPos.localPosition + gunOffset;
        }

        gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetLocal, Time.deltaTime * ADSspeed);
    }

    private void UpdateGunTransform()
    {
        if (playerShooting != null && playerShooting.gun != null)
        {
            gunTransform = playerShooting.gun.transform;
        }
    }

    public void OnADS(InputValue value)
    {
        isAiming = value.isPressed;

        if (playerMovement.state == PlayerMovement.MovementState.air || playerMovement.state == PlayerMovement.MovementState.wallrunning)
        {
            if (slowMoDurationCoroutine != null)
            {
                StopCoroutine(slowMoDurationCoroutine);
            }

            slowMotion.StartSlowMotion();
            slowMoDurationCoroutine = StartCoroutine(SlowMoTimer(3f));
        }
    }

    public void OnADSRelease(InputValue value)
    {
        isAiming = false;

        if (slowMoDurationCoroutine != null)
        {
            StopCoroutine(slowMoDurationCoroutine);
            slowMoDurationCoroutine = null;
        }

        slowMotion.StopSlowMotion();
    }

    private IEnumerator SlowMoTimer(float duration)
    {
        duration = 5f;
        yield return new WaitForSecondsRealtime(duration);
        slowMotion.StopSlowMotion();
        slowMoDurationCoroutine = null;
    }
}
