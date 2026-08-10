using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour 
{
    public Gun gun;
    public Transform gunHolder;
    private bool isHoldingShoot = false;
    public SlowMotion slowMotion;
    public bool hasCompletedTutorialPick = false; 

    void OnShoot() 
    {
        if (gun == null) return; 

        isHoldingShoot = true;
        
        if (slowMotion != null)
        {
            slowMotion.StopSlowMotion();
        }
    }

    void OnShootRelease() 
    {
        isHoldingShoot = false;
    }

    void OnReload() 
    {
        if(gun != null) {
            gun.TryReload();
        }
    }

    void Update() 
    {
        if(isHoldingShoot && gun != null) {
            gun.Shoot();
        }
    }

    public void OnDrop() 
    {
        if(gun != null) {
            gun.Drop();
            gun = null; 
        }
        isHoldingShoot = false; 
    }
}

