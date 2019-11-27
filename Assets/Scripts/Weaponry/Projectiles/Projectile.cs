using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    public ProjectileProps Props;
    public ParticleSystem ContactParticles;
    public LayerMask HittableLayer;
    public LayerMask CollisionLayer;

    protected Rigidbody2D _rigidBody;
    protected List<GameObject> damageRecievers;  //objects that recieved damage during the lifespan of this projectile.
    protected ParticleSystem _contactParticles;
    private TrailRenderer _trailRenderer;

    private LayerMask origLayer;
    private LayerMask inactiveLayer;
    private Transform _origTransform; //unknown usecase yet. Maybe I'll find it some sunny day.
    private float origGravity = -1;
    private PoolableObject _poolable;


    /* ------------------------------------------------------------------------------ */


    public void Start() {
        origLayer = this.gameObject.layer;
        inactiveLayer = LayerMask.NameToLayer("Ignore Raycast");
        _trailRenderer = GetComponent<TrailRenderer>();
        if (ContactParticles != null) {
            _contactParticles = Instantiate(ContactParticles);
            _contactParticles.Stop();
        }
        _origTransform = this.transform;
    }//Start

    public void Update() {
        Bounds cameraBounds = GameUtils.Utils.OrthographicBounds(Camera.main);
        bool[] isInView = GameUtils.Utils.IsWithinBounds(cameraBounds, this.transform.position);
        bool isInCamera = isInView[0] && isInView[1];
        if (this.Props.IsOffCameraDestroy && !isInCamera) {
            Destroy();
        }
    }//Update


    public void FixedUpdate() {

    }//FixedUpdate


    public void OnTriggerEnter2D(Collider2D collision) {
        if(_poolable == null)
            _poolable = GetComponent<PoolableObject>();

        if(collision.gameObject == _poolable.Instantiator)
            return;

        bool isCollision = GameUtils.Utils.CompareLayers(CollisionLayer, collision.gameObject.layer);
        if (isCollision) {
            Destroy();
            return;
        }

        bool isHittable = GameUtils.Utils.CompareLayers(HittableLayer, collision.gameObject.layer);
        if(!isHittable)
            return;

        var damagable = collision.GetComponent<Damageable>();
        if (damagable == null) {
            Destroy();
            return;
        }

        if(damagable.IsDead)
            return;

        float randChance = Random.Range(0.0f, 1.0f);
        bool isCrit = randChance < Props.PierceThroughChance;
        damagable.TakeDamage(this.gameObject, Props.Damage, isCrit, Props.VelocityDamageRatio);

        PlayCollisionParticles();

        if (!isCrit)
            Destroy();
        else {
            _rigidBody.velocity = _rigidBody.velocity / 2;
            _rigidBody.gravityScale += 0.5f;

            var rot = this.transform.rotation;
            this.transform.rotation = new Quaternion(rot.x, rot.y, rot.z + 55, rot.w + 3);

        }
    }//OnTriggerEnter2D

  
    public void PlayCollisionParticles() {
        if (_contactParticles == null)
            return;
        Vector2 direction = _rigidBody.velocity.normalized;
        _contactParticles.transform.position = this.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _contactParticles.transform.eulerAngles = new Vector2(angle, 0);

        _contactParticles.Play();
    }//PlayCollisionParticles


    public void Launch(Vector3 direction) {
        /* ************ SWING CONTROL ********************* */
        var currRot = transform.eulerAngles;
        Vector2 swing = new Vector2(currRot.z - 1, currRot.z + 1);
        this.transform.eulerAngles = new Vector3(currRot.x, currRot.y, Random.Range(swing.x, swing.y));

        var shootForce = direction * Props.Force;
        _rigidBody.AddForce(shootForce);
        damageRecievers.Clear();
    }

    /********* Destroy\Disable handler *********/

    public void OnEnable() {
        if (inactiveLayer == this.gameObject.layer)
            this.gameObject.layer = origLayer;

        if (damageRecievers == null)
            damageRecievers = new List<GameObject>();
        if (_rigidBody == null) {
            _rigidBody = GetComponent<Rigidbody2D>();
            if(this.origGravity == -1)
                this.origGravity = this._rigidBody.gravityScale;
        }

        if (_origTransform == null) {
            _origTransform = this.transform;
        }

        if(this.origGravity != -1)
            this._rigidBody.gravityScale = this.origGravity;

        //this.transform.rotation = new Quaternion(0, 0, 0, 0);

        float direction = Mathf.Sign(this.transform.localScale.x);
        Launch(this.transform.right * direction);

        Invoke("Destroy", Props.ProjectileTimeout);  //despawn after X seconds.
    }//OnEnabled


    public void Destroy() {
        CancelInvoke();

        if (_trailRenderer != null)
            _trailRenderer.Clear();

        this.gameObject.SetActive(false);
    }//Destroy


    public void OnDisable() {
        if (_rigidBody == null)
            _rigidBody = GetComponent<Rigidbody2D>();

        this.transform.localScale = _origTransform.localScale;
        CancelInvoke();
    }//OnDisable

}//class


[System.Serializable]
public class ProjectileProps {
    public int Damage = 10;
    [Tooltip("How much percent to take out of the current velocity when damage is taken.")]
    [Range(0, 1)]
    public float VelocityDamageRatio = 0.4f;
    [Tooltip("Time before projectile become inactive again.")]
    public float ProjectileTimeout = 2f;
    [Tooltip("Shooting force to apply to projectile.")]
    public float Force = 500f;
    public bool IsOffCameraDestroy = true;

    [Range(-0.1f, 1f)] public float PierceThroughChance = 0;
}//class