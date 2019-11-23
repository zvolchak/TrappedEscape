using UnityEngine;
using GHTriggers;

public class WeaponReactor : LeverReactor {

    [Tooltip("Either set manually, or leave none and it will pickup the cmp from This GO.")]
    public Weapon WeaponToTrigger;

    public void Start() {
        if(WeaponToTrigger == null)
            WeaponToTrigger = GetComponent<Weapon>();
    }//Start


    public override void Activate() {
        if (WeaponToTrigger == null) {
#if UNITY_EDITOR
            Debug.LogWarning("WeaponToTrigger for " + this.name + " WeaponReactor is not set!");
            return;
#endif
        }//if
        Debug.Log("??");
        WeaponToTrigger.PullTrigger();
    }//Activate


    public override void Deactivate() {
        throw new System.NotImplementedException();
    }//Deactivate
}//class
