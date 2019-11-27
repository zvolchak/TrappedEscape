using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class AutoShoot : MonoBehaviour {

    public float Interval = 1f;
    [Tooltip("Turret state on level load: True - shooting; False - idle.")]
    public bool StateOnStart = false;

    private Weapon _weapon;

    public bool IsShooting { get; private set; }

    /* --------------------------------------------------------------------- */

    public void Trigger(bool state) {
        if (!IsShooting && state)
            WeaponCmp.PullTrigger();
        else if (!state) {
            WeaponCmp.ReleaseTrigger();
        }

        IsShooting = state;
    }//Trigger


    public void OnEnable() {
        if(StateOnStart)
            Trigger(true);
    }//OnEnable


    public void OnDisable() {
        Trigger(false);
    }//OnDisable


    public Weapon WeaponCmp {
        get {
            if (_weapon == null)
                _weapon = GetComponent<Weapon>();
            return _weapon;
        }
    }//WeaponCmp

}//class
