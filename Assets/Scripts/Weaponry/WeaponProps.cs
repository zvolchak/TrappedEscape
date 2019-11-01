using System.Collections;
using UnityEngine;


///<summery>
///</summery>

[System.Serializable]
public class WeaponProps {

    public PoolableObject ProjectilePrefab;
    [Tooltip("Spawn point of the projectile")]
    public Transform Nozzle;
    public int Damage = 5;
    public float RateOfFire = 2f;
    [Tooltip("Time before attack.")]
    public float ChargingTime = 0f;
    [Tooltip("Range of attack. Use -1 to make it a bullet (e.g. timer based)")]
    public float Range = -1;
    public int ClipSize = 15;
    public int AmmoPerShot = 1;
    public float ReloadTime = 1.5f;
    public bool IsAutoReload = true;

    public AudioClip AttackSound;

    public bool IsReloading { get; private set; }


    /// <summary>
    ///  Called on Start and End of the reload cycle.
    /// </summary>
    /// <param name="status">True - reload started. False - reload finished.</param>
    public delegate void ReloadDelegate(bool status);
    public ReloadDelegate EOnReloadEvent;


    /// <summary>
    ///  Remove bullets from current clip's value.
    /// </summary>
    public void RemoveFromClip() {
        if (this.ClipStatus == 0 || ClipSize < 0)
            return;

        this.ClipStatus -= AmmoPerShot;
        if (this.ClipStatus < 0)
            this.ClipStatus = 0;
    }//UpdateCurrClip


    public void ResetClip() {
        if(ClipSize < 0) //probably a meele weapon
            return;

        this.ClipStatus = ClipSize;

        EOnReloadEvent?.Invoke(false);
    }//ResetClip


    public IEnumerator ReloadCoRoutine() {
        if (IsReloading || ClipSize < 0)
            yield break;

        IsReloading = true;
        EOnReloadEvent?.Invoke(true);

        yield return new WaitForSeconds(ReloadTime);

        ResetClip();
        IsReloading = false;
        EOnReloadEvent?.Invoke(false);
    }//Reload


    public int ClipStatus { get; private set; }


    public bool IsInRange(Vector3 targetToAttack) {
        float distance = (targetToAttack - Nozzle.position).magnitude;
        return distance <= Range;
    }

}//class WeaponProps