using UnityEngine;


public class Damageable : MonoBehaviour {

    public int Health = 100;
#if UNITY_EDITOR
    public int DEBUGHEALTH = -1;
#endif
    public AudioClip[] ImpactSound;

    protected GameObject damageInstigator;
    protected int healthStatus;
    protected AudioSource _audio;
    protected Collider2D _collider; //FIXME: shouldnt be here
    protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;

    /// <summary>
    ///  Called when TakeDamage is called.
    /// </summary>
    /// <param name="instigator">Who is damaging the entiry.</param>
    /// <param name="amount">How much damage is dealt.</param>
    public delegate void OnDamageTakenEvent(GameObject instigator, int amount);
    public delegate void OnDeathEvent(GameObject instigator);
    public delegate void CricicalHitTaken(GameObject instigator, int amount);
    public delegate void OnHealthUpdate(int healthAfterUpdate, int changedAmount);

    /// <summary>
    /// Signature: (GameObject instigator, int dmgAmount);
    /// </summary>
    public OnDamageTakenEvent EOnDamageTaken;
    /// <summary>
    /// Signature: (GameObject instigator);
    /// </summary>
    public OnDeathEvent EOnDeath;
    /// <summary>
    /// Signature: (GameObject instigator, int dmgAmount);
    /// </summary>
    public CricicalHitTaken EOnCritHit;
    /// <summary>
    /// Signature: OnHealthUpdate(int healthAfterUpdate, int changedAmount);
    /// </summary>
    public OnHealthUpdate EOnHealthUpdated;


    public virtual void Start() {
        healthStatus = Health;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audio = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }//Start


    public void FixedUpdate() {
#if UNITY_EDITOR
        DEBUGHEALTH = this.HealthStatus;
#endif
    }//FixedUpdate


    /// <summary>
    ///   Deal damage to this object and call delegate events.
    /// Return True if damage was taken - False if is already dead.
    /// </summary>
    /// <param name="instigator"> object which is dealing the damage </param>
    /// <param name="amount"> amount of damage dealing</param>
    /// <returns></returns>
    public virtual bool TakeDamage(GameObject instigator, int amount, bool isCrit, float velocityDmgIndex=float.NegativeInfinity) {
        if(IsDead)
            return false;
        damageInstigator = instigator;
        this.HealthStatus -= amount;
        if(isCrit) EOnCritHit?.Invoke(instigator, amount);
        else EOnDamageTaken?.Invoke(instigator, amount);

        bool isDead = HealthStatus <= 0;
        if (isDead) {
            EOnDeath?.Invoke(instigator);
            Destroy();
            //this.gameObject.SetActive(false);

        } else {
            PlayImpactSound();
        }

        EOnHealthUpdated?.Invoke(this.HealthStatus, amount);

        return !isDead;
    }//TakeDamage


    /// <summary>
    ///  Restore a portion of the health. Return True if health was restored.
    /// False - if current health is at max value.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public virtual bool HealUp(int amount) {
        if(HealthStatus == Health)
            return false;

        healthStatus += amount;
        if(healthStatus > Health)
            healthStatus = Health;

        EOnHealthUpdated?.Invoke(this.HealthStatus, amount);

        return true;
    }//HealUp


    public void PlayImpactSound() {
        if(_audio == null)
            return;
        if(ImpactSound == null || ImpactSound.Length == 0)
            return;

        int randSound = Random.Range(0, ImpactSound.Length);
        _audio.PlayOneShot(ImpactSound[randSound]);
    }//PlayImpactSound


    /// <summary>
    ///  Reduce health to 0 instantly.
    /// </summary>
    public void InstaKill() {
        healthStatus -= healthStatus;
    }//InstaKill


    public virtual void Destroy() {
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        damageInstigator = null;
    }//Destroy


    public void OnEnable() {
        this.Reset();
    }//OnEnable


    public virtual void Reset() {
        healthStatus = Health;
        if(_collider != null)
            _collider.enabled = true;
        if(_spriteRenderer != null)
            _spriteRenderer.enabled = true;
    }//Reset


    public bool IsDead { get { return !(healthStatus > 0); } }

    public int HealthStatus {
        get { return healthStatus; }
        private set {
            this.healthStatus = value;
        }
    }
}//class