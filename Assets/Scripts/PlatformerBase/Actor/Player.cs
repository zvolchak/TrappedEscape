using UnityEngine;
using GHAbilities;
using GHPlatformerControls;
using GHPhysics;

/// <summary>
///  A "master" class for accessing and controlling players' operations.
/// It is a sort of a "manager" class.
/// </summary>
[RequireComponent(typeof(CollisionDetection), typeof(MovementControls))]
public class Player : AActor2D {

    public static Player Instance;

    [Tooltip("Value at which X and Y velocity is set to 0")]
    public float MinVelocityThreshold = 0.2f;
    public Vector2 MaxVelocityThreshold = new Vector2(10, 15);
    public Sfx LandingDustSfx;
    [Tooltip("How many seconds to stay grounded before setting Idle animation.")]
    public float TimeoutBeforeIdle = 0.1f;
    [Tooltip("Allow player to have velocity higher than threshold.")]
    public bool IgnoreVelocityThreshold = false;

    //private Gravity _gravity;
    public ActorAbilitiesProps Abilities;

    private PlatformDropthrough _dropthrough;
    //private MovementControls MvmntCmp;
    //private CollisionDetection _collisionDetection;
    private CharacterAnimator _charAnimator;
    private LadderClimber _ladderClimber;

    private bool bIsCanMove;
    //private bool bIsInAir;
    private float groundedTimeout; //FIXME: should be somewhere else. Resets IDL state when on ground for too long.
    private int origSortingOrder;
    private string origSortingLayerName;
    //private bool bCanToggleLanded;
    public Vector3 DEBUGSOME;


    /* ---------------------- ----------------------- ---------------------- */


    public override void Start () {
        base.Start();
        if(Instance == null) Instance = this;
        else Destroy(this.gameObject);

        //_gravity = GetComponent<Gravity>();
        //_collisionDetection = GetComponent<CollisionDetection>();
        //MvmntCmp = GetComponent<MovementControls>();
        _dropthrough = GetComponent<PlatformDropthrough>();
        _charAnimator = GetComponent<CharacterAnimator>();
        _ladderClimber = GetComponent<LadderClimber>();

        bIsCanMove = true;
        this.groundedTimeout = 0f;

        this.origSortingLayerName = SpriteRendererCmp.sortingLayerName;
        this.origSortingOrder = SpriteRendererCmp.sortingOrder;

        this.Abilities.Actor = this;
        this.EOnLanded += this.Abilities.JumpCmp.Reset;
    }//Start


    /// <summary>
    ///  Cast ground rays to set IsGrounded flag.
    /// </summary>
    public override void FixedUpdate() {
        if (!SpriteRendererCmp.enabled)
            return;
        base.FixedUpdate();
        if (!IsCanControl) {
            return;
        }
    }//FixedUpdate


    public override void Update() {
        base.Update();
    }//Update


    public override void LateUpdate() {
        base.LateUpdate();
        //if (!IsCanControl)
        //    return;
        //bIsInAir = !IsGrounded && !IsOnSlope;
    }//LateUpdate


    public override void HandleVelocity(ref Vector2 deltaMovement) {
        if (!IsCanControl)
            return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        this.Abilities.HandleJumping(ref deltaMovement);

        if (_ladderClimber != null && !_ladderClimber.IsClimbing && IsGrounded)
            this.Abilities.HandleSprinting(ref deltaMovement);

        if (_ladderClimber != null && _ladderClimber.IsClimbing)
            deltaMovement = this.Abilities.SprintCmp.Stop(deltaMovement);
    }//HandleVelocity


    public override void HandleMovement(ref Vector2 deltaMovement) {
        if (Abilities.DashCmp != null) {
            if (Abilities.DashCmp.IsDashing ||
                        (Abilities.SprintCmp != null && Abilities.SprintCmp.IsSprinting) ||
                        (Abilities.WallGrabCmp != null && Abilities.WallGrabCmp.IsWallJumping))
                return;
        }

        if(Abilities.WallGrabCmp != null && Abilities.WallGrabCmp.IsOnWall)
            return;

        base.HandleMovement(ref deltaMovement);
    }//HandleMovement


    public void SwitchDirection() {
        //if(!IsGrounded && !IsOnSlope)
        //    return;
        if(DirectionSwitcherCmp == null)
            return;
        if(Abilities.DashCmp != null && Abilities.DashCmp.IsDashing)
            return;
        if(Abilities.WallGrabCmp != null && 
            (Abilities.WallGrabCmp.IsOnWall || Abilities.WallGrabCmp.IsBackflipJumping))
            return;

        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis == 0)
            return;

        float sign = Mathf.Sign(horizontalAxis);
        if (Mathf.Sign(this.transform.localScale.x) != sign)
            DirectionSwitcherCmp.OnSwitchDirection();
    }//SwitchDirection


    /// <summary>
    ///  Set player animation state to Idle if character
    /// is Grounded.
    /// </summary>
    public void SetIdle() {
        if (IsGrounded) {
            this.groundedTimeout += Time.deltaTime;

            if(this.groundedTimeout < TimeoutBeforeIdle)
                return;
        } else {
            this.groundedTimeout = 0f;
        }
    }//SetIdle


    public void SetDisableMovement(bool state) {
        this.bIsCanMove = state;
    }//SetCanMove


    /************************** GETTERS/SETTERS **************************/

    public bool IsCanControl {
        get {
            if (GameState.Instance == null)
                return true;
            return GameState.Instance.CurrentState == GameState.State.playing;
        }//get
    }//IsCanControl

    //public override bool IsGrounded { get {
    //        return _collisionDetection.Below;
    //    }
    //}

    //public bool IsOnSlope { get { return this._raycastGround.IsOnSlope; } }
    public bool IsOnSlope { get { return CollisionDetector.IsOnSlope; } }

    ///// <summary>
    /////  Player was in the air last frame, but this frame has IsGrounded = true.
    ///// IsLanded will become false in the beginning of the next frame.
    ///// </summary>
    //public bool IsLanded { get; private set; }


    public void Respawn() {
        SpriteRendererCmp.enabled = true;
        SpriteRendererCmp.sortingLayerName = this.origSortingLayerName;
        SpriteRendererCmp.sortingOrder = this.origSortingOrder;
        ColliderCmp.enabled = true;
        DamagableCmp.Reset();
    }//Respawn


    public bool CanMove { get { return bIsCanMove && SpriteRendererCmp.enabled; } }

    public float Direction { get { return Mathf.Sign(this.transform.localScale.x); } }

}//class Player