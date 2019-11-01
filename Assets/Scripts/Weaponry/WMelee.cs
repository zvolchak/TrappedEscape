using UnityEngine;

/// <summary>
/// 
/// </summary>
public class WMelee : Weapon{

    public string AnimationAttackName = "Attack";
    //[Tooltip("Time before attack")]
    //public float ChargingTime = 0.3f;
    public LayerMask TargetMask;
    private Animator _animator;


    public override void Start() {
        base.Start();
        _animator = GetComponent<Animator>();
    }


    public override bool Shoot(Quaternion rotDirection) {
        bool isCanShoot = base.Shoot(rotDirection);
        if (!isCanShoot)
            return isCanShoot;

        Vector2 point = this.transform.position;
        Vector2 size = new Vector2(this.Props.Range, this.Props.Range / 2);
        float dir = this.Owner.transform.localScale.x;
        RaycastHit2D[] attackArea = Physics2D.CircleCastAll(point, size.x, this.transform.right * dir, size.x, TargetMask);
        
        Debug.DrawRay(point, size.x * this.transform.right * dir, Color.yellow, 1f);

        foreach (RaycastHit2D ray in attackArea) {
            if (ray) {
                if(ray.collider.gameObject == this.Owner)
                    continue;

                Damageable dmgble = ray.collider.GetComponent<Damageable>();
                if(dmgble == null)
                    continue;

                dmgble.TakeDamage(this.Owner.gameObject, this.Props.Damage, false);
            }
        }

        if (_animator != null) {
            _animator.SetTrigger(AnimationAttackName);
        }
        return true;
    }//Shoot


    public void OnDrawGizmos() {
        
    }


}//class WMelee
