using UnityEngine;

public class WGun : Weapon {

    [Tooltip("Either set by hand, or it will be picked from PoolManager.")]
    public ObjectPool ProjectilePool;
    public float PushbackForce = 0f;

    private ProjectileShell _shell;


    public override void Start() {
        base.Start();
        _shell = GetComponent<ProjectileShell>();
    }//Start


    public override bool Shoot(Quaternion rotDirection) {
        bool isCanShoot = base.Shoot(rotDirection);
        if(!isCanShoot)
            return isCanShoot;

        SpawnProjectile(rotDirection);

        return true;
    }//Shoot


    public void SpawnProjectile(Quaternion rotDirection) {
        if(Props.ProjectilePrefab == null)
            return;

        if (ProjectilePool == null) {
            if(PoolManager.Instance != null)
                ProjectilePool = PoolManager.Instance.FindPoolByPrefab(Props.ProjectilePrefab);
        }//if

        float directionSign = Mathf.Sign(Owner.transform.localScale.x);
        if (this.Owner != null)
            directionSign = this.Owner.transform.localScale.x;

        Vector3 spawnPosition = Props.Nozzle == null ? this.transform.position : Props.Nozzle.position;

        PoolableObject projectile = null;
        if (ProjectilePool == null) {   //Spawn instance directly
            projectile = Instantiate(this.Props.ProjectilePrefab);
            projectile.transform.position = spawnPosition;
            projectile.SetActive(true);
        } else {                        //Spawn from pool
            projectile = ProjectilePool.SpawnAt(spawnPosition, true);
        }

        if(Owner != null)
            projectile.SetInstantiator(Owner.gameObject);

        projectile.transform.rotation = this.transform.rotation;

        var projectileScale = projectile.transform.localScale;

        //Make sure bullet will be facing same direction as the gun.
        //Because since bullets are re-used, flipping its scale gets
        //messy and unconsistant.
        if (Mathf.Sign(projectileScale.x) != directionSign)
            projectileScale.x *= -1;

        projectile.transform.rotation = rotDirection;

        projectile.transform.localScale = projectileScale;
        projectile.SetActive(true);

        if(Owner != null && PushbackForce != 0)
            Owner.RBCmp.AddForce(Vector2.right * -Owner.transform.localScale.x * PushbackForce);

        if (_shell != null)
            _shell.Spawn(-this.Owner.transform.localScale.x);
    }//SpawnProjectile

    public PoolManager PoolMngrCmp => PoolManager.Instance;

}//class WGun