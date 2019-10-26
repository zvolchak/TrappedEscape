using UnityEngine;
using GHPlatformerControls;
using GHPhysics;

///<summery>
///</summery>
public class AActor2D : MonoBehaviour {

    public Rigidbody2D RBCmp { get; private set; }
    public Collider2D ColliderCmp { get; private set; }
    public MovementControls MvmntCmp { get; private set; }
    public SwitchDirection DirectionSwitcherCmp { get; private set; }
    public ActionAnimator AACmp { get; private set; }
    public Damageable DamagableCmp { get; private set; }
    public SpriteRenderer SpriteRendererCmp { get; private set; }
    public Gravity Gravity { get; private set; }
    public CollisionDetection CollisionDetector;


    public virtual void Start() {
        RBCmp = GetComponent<Rigidbody2D>();
        ColliderCmp = GetComponent<Collider2D>();
        MvmntCmp = GetComponent<MovementControls>();
        DirectionSwitcherCmp = GetComponent<SwitchDirection>();
        AACmp = GetComponent<ActionAnimator>();
        DamagableCmp = GetComponent<Damageable>();
        SpriteRendererCmp = GetComponent<SpriteRenderer>();
        Gravity = GetComponent<Gravity>();
        CollisionDetector = GetComponent<CollisionDetection>();
    }//Start


    public virtual bool IsGrounded => false;

}//AActor
