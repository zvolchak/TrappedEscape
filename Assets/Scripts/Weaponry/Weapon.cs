using System.Collections;
using UnityEngine;

/// <summary>
///  A Base class for the Weapon mechanics. The Component should be a child of the object which
///  will be using it.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Weapon: MonoBehaviour {

    public WeaponProps Props;
    [Tooltip("Game object that holds the weapon. (typically, a parent of the Weapon object).")]
    public AActor2D Owner;

    public LayerMask Attackable;
    public bool AimAtMouse { get; set; }
    public delegate void OnShootDelegate();
    public OnShootDelegate OnShootListeners;

    protected float lastShotTime = -1;

    private AudioSource _audioSource;
    private SpriteRenderer _spriter;
    private Rigidbody2D _rb;
    private Collider2D _collider;

    protected bool bIsFire;
    protected float cooldown;
    protected bool bIsCooldown;


    // Use this for initialization
    public virtual void Start() {
        bIsFire = false;
        bIsCooldown = false;
        cooldown = 0.0f;
        _audioSource = GetComponent<AudioSource>();
        CrosshairCmp = GetComponentInChildren<Crosshair>();
        _rb = GetComponent<Rigidbody2D>();

        Props.ResetClip();
        if(Owner == null && CrosshairCmp != null)
            CrosshairCmp.gameObject.SetActive(false);
        if(Props.Nozzle == null)
            Props.Nozzle = this.transform;
    }//Start


    public virtual void Update() {
    }//Update


    public virtual void FixedUpdate() {
        if (bIsFire) {
            Shoot(this.transform.rotation);
        }//if fire
    }//FixedUpdate


    /* Pulling trigger sets the variable to indicate the "attacking" state
     * which is handled by the FixedUpdate on each frame, while trigger is
     * pulled. */
    public virtual void PullTrigger() {
        bIsFire = true;
    }//OnShoot


    public virtual void ReleaseTrigger() {
        bIsFire = false;
    }//ReleaseTrigger


    /* True - is shot was made. False - otherwise. */
    public virtual bool Shoot(Quaternion rotDirection) {
        bool isCanShoot = IsCanShoot;
        if (!isCanShoot)
            return false;
        if(_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        if(_audioSource != null)
            _audioSource.PlayOneShot(Props.AttackSound, 1f);

        this.lastShotTime = Time.fixedTime;
        
        this.Props.RemoveFromClip();

        if (this.Props.ClipStatus <= 0)
            StartCoroutine(this.Props.ReloadCoRoutine());

        this.OnShootListeners?.Invoke();
        return isCanShoot;
    }//Shoot


    public virtual void ResetRotation() {
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
    }//ResetRotation


    /// <summary>
    ///  Hide from the scene and flag it in the inventory as none-shootable.
    /// </summary>
    /// <param name="state"></param>
    public virtual void Hide(bool state) {
        SpriteCmp.enabled = !state;
        if(CrosshairCmp != null)
            CrosshairCmp.gameObject.SetActive(!state);
    }//Hide


    public virtual bool IsHidden => SpriteCmp == null || !SpriteCmp.enabled;


    public bool IsCanShoot {
        get {
            if (IsHidden)
                return false;
            if (lastShotTime < 0)
                return true;
            if (Props.ClipSize >= 0 && Props.ClipStatus <= 0)
                return false;
            if(Props.IsReloading)
                return false;
            return (Time.fixedTime - this.lastShotTime) >= Props.RateOfFire;
        }//get
    }//IsCanShoot


    public Crosshair CrosshairCmp { get; private set; }


    public SpriteRenderer SpriteCmp {
        get {
            if (_spriter == null)
                _spriter = GetComponent<SpriteRenderer>();
            return _spriter;
        }//get
    }//SpriteCmp


    public Rigidbody2D RigidBodyCmp {
        get {
            if(_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
    }


    public Collider2D ColliderCmp {
        get {
            foreach (Collider2D cld in GetComponents<Collider2D>()) {
                if (!cld.isTrigger) {
                    _collider = cld;
                    break;
                }
            }
            return _collider;
        }
    }
}//class