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
    public CollisionDetection CollisionDetectionCmp;
    public PlatformDropthrough DropthroughCmp { get; private set; }

    protected bool bCanToggleLanded;
    protected bool prevGroundState;

    public virtual bool IsGrounded => CollisionDetectionCmp != null && CollisionDetectionCmp.Below;

    /// <summary>
    ///  Player was in the air last frame, but this frame has IsGrounded = true.
    /// IsLanded will become false in the beginning of the next frame.
    /// </summary>
    public bool IsLanded { get; private set; }

    /* ---------------------- EVENTS ---------------------- */

    /// <summary>
    ///  Called when character was in the air on the prev frame, but 
    /// is grounded on this frame (e.g. has landed..)
    /// </summary>
    public event GHDelegates.CommonDelegates.SimpleDelegate EOnLanded;

    /// <summary>
    ///  Called when IsGrounded state This frame is different from previous frame.
    ///  Signature: (bool prevState, bool newState)
    /// </summary>
    public event GHDelegates.CommonDelegates.StateChange EOnGroundStateChange;

    /// <summary>
    /// Called when Velocity of the player is updated.
    /// Signature: (Vector3 prevVelocity, Vector3 currVelocity)
    /// </summary>
    public GHDelegates.CommonDelegates.VectorStateChange EOnVelocityUpdated;

    /* ---------------------- ----------------------- ---------------------- */


    public virtual void Start() {
        RBCmp = GetComponent<Rigidbody2D>();
        ColliderCmp = GetComponent<Collider2D>();
        MvmntCmp = GetComponent<MovementControls>();
        DirectionSwitcherCmp = GetComponent<SwitchDirection>();
        AACmp = GetComponent<ActionAnimator>();
        DamagableCmp = GetComponent<Damageable>();
        SpriteRendererCmp = GetComponent<SpriteRenderer>();
        Gravity = GetComponent<Gravity>();

        DropthroughCmp = GetComponent<PlatformDropthrough>();

        if(CollisionDetectionCmp == null)
            CollisionDetectionCmp = GetComponent<CollisionDetection>();
    }//Start


    public virtual void FixedUpdate() {
        if (IsLanded) {
            IsLanded = false;
            bCanToggleLanded = false;
        }//if

        if (IsGrounded && bCanToggleLanded && !DropthroughCmp.IsDropping)
            IsLanded = true;

        if (!this.IsGrounded)
            bCanToggleLanded = true;

        Vector3 deltaMovement = MvmntCmp.Velocity;

        //APPLY GRAVITY
        if (this.Gravity != null)
            this.Gravity.Apply(ref deltaMovement);

        if (DropthroughCmp.IsCanDropthrough) {
            if (DropthroughCmp.IsDropping) {
                if (!CollisionDetectionCmp.Props.CollisionIgnoreTags.Contains("Platform"))
                    CollisionDetectionCmp.Props.CollisionIgnoreTags.Add("Platform");
            } else {
                if (CollisionDetectionCmp.Props.CollisionIgnoreTags.Contains("Platform"))
                    CollisionDetectionCmp.Props.CollisionIgnoreTags.Remove("Platform");
            }
        }//if IsCanDropthrough

        CollisionDetectionCmp.Move(ref deltaMovement);

        MvmntCmp.SetVelocity(deltaMovement);
        transform.Translate(MvmntCmp.Velocity);
    }//FixedUpdate


    public virtual void Update() {
        Vector2 deltaMovement = MvmntCmp.Velocity;
        HandleMovingPlatform(ref deltaMovement);
        HandleMovement(ref deltaMovement);
        DropthroughCmp.Check();

        if (CollisionDetectionCmp.Left || CollisionDetectionCmp.Right)
            MvmntCmp.ResetPushForce();

        HandleVelocity(ref deltaMovement);
        MvmntCmp.SetVelocity(deltaMovement);

        if (this.IsGrounded && !prevGroundState && !DropthroughCmp.IsDropping) {
            IsLanded = true;
            this.EOnLanded?.Invoke();
        }//if landed

        if (prevGroundState != this.IsGrounded) {
            this.EOnGroundStateChange?.Invoke(this.prevGroundState, this.IsGrounded);
            this.prevGroundState = this.IsGrounded;
        }
    }//Update


    public virtual void LateUpdate() {
    }//LateUpdate


    /// <summary>
    /// Called before Velocity is set during Update() cycle.
    /// </summary>
    /// <param name="deltaMovement"></param>
    public virtual void HandleVelocity(ref Vector2 deltaMovement) {
    }//HandleVelocity


    public virtual void HandleMovement(ref Vector2 deltaMovement) {
        deltaMovement = MvmntCmp.Move(deltaMovement);
    }//HandleMovement


    public virtual void HandleMovingPlatform(ref Vector2 deltaMovement) {
        foreach (RaycastMeta meta in CollisionDetectionCmp.VerticalRayMeta) {
            if (!meta.Ray)
                continue;
            if (!meta.HitTag.Equals("LiftPlatform"))
                continue;

            this.transform.SetParent(meta.Ray.collider.transform);
            return;
        }//foreach

        if (this.transform.parent != null)
            this.transform.SetParent(null);
    }//HandleMovingPlatform

}//AActor
