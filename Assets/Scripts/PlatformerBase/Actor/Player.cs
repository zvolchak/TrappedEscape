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

    private Gravity _gravity;
    public ActorAbilitiesProps Abilities;

    private PlatformDropthrough _dropthrough;
    private MovementControls _movementCtrls;
    private CollisionDetection _collisionDetection;
    private CharacterAnimator _charAnimator;
    private LadderClimber _ladderClimber;

    private bool bIsCanMove;
    private bool bIsInAir;
    private float groundedTimeout; //FIXME: should be somewhere else. Resets IDL state when on ground for too long.
    private int origSortingOrder;
    private string origSortingLayerName;
    private bool bCanToggleLanded;
    public Vector3 DEBUGSOME;
    private bool prevGroundState;

    /* ---------------------- EVENTS ---------------------- */

    /// <summary>
    ///  Called when character was in the air on the prev frame, but 
    /// is grounded on this frame (e.g. has landed..)
    /// </summary>
    public event PlayerDelegates.OnLandedEvent OnLandedListeners;

    /// <summary>
    ///  Called when IsGrounded state This frame is different from previous frame.
    ///  Signature: (bool prevState, bool newState)
    /// </summary>
    public event PlayerDelegates.OnGroundStateChanged EGroundedStateListener;

    /// <summary>
    /// Called when Velocity of the player is updated.
    /// Signature: (float prevVelocity, float currVelocity)
    /// </summary>
    public PlayerDelegates.OnVelocityUpdate EVelocityUpdateListeners;

    /* ---------------------- ----------------------- ---------------------- */


    public override void Start () {
        base.Start();
        if(Instance == null) Instance = this;
        else Destroy(this.gameObject);

        _gravity = GetComponent<Gravity>();
        _collisionDetection = GetComponent<CollisionDetection>();
        _dropthrough = GetComponent<PlatformDropthrough>();
        _movementCtrls = GetComponent<MovementControls>();
        _charAnimator = GetComponent<CharacterAnimator>();
        _ladderClimber = GetComponent<LadderClimber>();

        bIsCanMove = true;
        this.groundedTimeout = 0f;

        this.origSortingLayerName = SpriteRendererCmp.sortingLayerName;
        this.origSortingOrder = SpriteRendererCmp.sortingOrder;

        this.Abilities.Actor = this;
    }//Start


    /// <summary>
    ///  Cast ground rays to set IsGrounded flag.
    /// </summary>
    public void FixedUpdate() {
        if (!SpriteRendererCmp.enabled)
            return;
        if (!IsCanControl) {
            return;
        }

        if (IsLanded) {
            IsLanded = false;
            bCanToggleLanded = false;
        }//if

        if (IsGrounded && bCanToggleLanded)
            IsLanded = true;

        if (!this.IsGrounded)
            bCanToggleLanded = true;

        Vector3 deltaMovement = _movementCtrls.Velocity;

        //APPLY GRAVITY
        if (this.Abilities != null && Abilities.DashCmp != null && Abilities.WallGrabCmp != null) {
            //FIXME: THIS IS A BUG. Checks for Both DashCmp and WallGrabCmp to 
            //apply gravity... need to check for each individually instead.
            if (!Abilities.DashCmp.IsDashing && !Abilities.WallGrabCmp.IsOnWall) {
                if (_gravity != null) _gravity.Apply(ref deltaMovement);
                //deltaMovement.y += -this.Gravity * Time.deltaTime;
            }
        } else {
            if (_gravity != null) _gravity.Apply(ref deltaMovement);
            //deltaMovement.y += -this.Gravity * Time.deltaTime;
        }

        _collisionDetection.Move(ref deltaMovement);

        _movementCtrls.SetVelocity(deltaMovement);
        transform.Translate(MvmntCmp.Velocity);
    }//FixedUpdate


    public void Update() {
        if (!IsCanControl)
            return;
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        SetIdle();
        SwitchDirection();

        Vector2 deltaMovement = _movementCtrls.Velocity;

        HandleMovingPlatform(ref deltaMovement);

        if (_collisionDetection.Left || _collisionDetection.Right) {
            if (Abilities.SlideCmp != null && Abilities.SlideCmp.IsDashing) {
                deltaMovement = Abilities.SlideCmp.Stop(deltaMovement);
            }
            if (Abilities.DashCmp != null && Abilities.DashCmp.IsDashing)
                deltaMovement = Abilities.DashCmp.Stop(deltaMovement);
        }

        if (Mathf.Abs(horizontalInput) >= 0.01f) {
            if(Abilities.WallGrabCmp != null)
                deltaMovement = Abilities.WallGrabCmp.OnWall(deltaMovement);
        } else if (Abilities.WallGrabCmp != null && Abilities.WallGrabCmp.IsOnWall) {
            deltaMovement = Abilities.WallGrabCmp.OnWall(deltaMovement);
        }


        if (Abilities.WallGrabCmp != null) {
            //This makes a Backflip jump to Slowdown at the peak.
            if (Abilities.WallGrabCmp.IsBackflipJumping && deltaMovement.y > 0) {
                float backflipSlowdown = Abilities.WallGrabCmp.Props.BackflipSlowdown;
                float lerpTime = backflipSlowdown - Mathf.Cos(deltaMovement.y * Time.deltaTime * Mathf.PI * 0.5f);
                deltaMovement = Vector2.Lerp(deltaMovement, Vector3.zero, lerpTime);
            }
        }//if WallGrabCmp

        //Fall Through Platform Logic
        if (this.IsGrounded) {
            if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump")) {
                _collisionDetection.FallThrough();
            }
        }//IsGrounded

        HandleMovement(ref deltaMovement);
        this.Abilities.HandleJumping(ref deltaMovement);
        this.Abilities.HandleSliding(ref deltaMovement);
        this.Abilities.HandleCrouching(ref deltaMovement);
        this.Abilities.HandleDashAbility(ref deltaMovement);
        if (_ladderClimber != null && !_ladderClimber.IsClimbing && IsGrounded)
            this.Abilities.HandleSprinting(ref deltaMovement);

        if (_ladderClimber != null && _ladderClimber.IsClimbing) {
            deltaMovement = this.Abilities.SprintCmp.Stop(deltaMovement);
        }


        if (_charAnimator != null) {
            _charAnimator.SetDashing(Abilities.DashCmp.IsDashing);
            _charAnimator.AnimatorCmp.SetBool("IsSliding", Abilities.SlideCmp.IsDashing);
        }

        if (_collisionDetection.Left || _collisionDetection.Right) {
            _movementCtrls.ResetPushForce();
        }

        if (_charAnimator != null) {
            _charAnimator.SetVelocity(Mathf.Abs(deltaMovement.x));
        }

        EVelocityUpdateListeners?.Invoke(_movementCtrls.Velocity, deltaMovement);
        _movementCtrls.SetVelocity(deltaMovement);

        if (bIsInAir && this.IsGrounded) {
            IsLanded = true;
            this.Abilities.JumpCmp.Reset();
            this.OnLandedListeners?.Invoke();

            if (LandingDustSfx != null)
                LandingDustSfx.PlayParticles(LandingDustSfx.transform.position);
        }

        if (prevGroundState != this.IsGrounded) {
            this.EGroundedStateListener?.Invoke(this.prevGroundState, this.IsGrounded);
            this.prevGroundState = this.IsGrounded;
            if(Abilities.WallGrabCmp != null) Abilities.WallGrabCmp.Reset();
        }
    }//Update


    public void LateUpdate() {
        if (!IsCanControl)
            return;
        bIsInAir = !IsGrounded && !IsOnSlope;
    }//LateUpdate


    public void HandleMovement(ref Vector2 deltaMovement) {
        if (Abilities.DashCmp != null) {
            if (Abilities.DashCmp.IsDashing ||
                        (Abilities.SprintCmp != null && Abilities.SprintCmp.IsSprinting) ||
                        (Abilities.WallGrabCmp != null && Abilities.WallGrabCmp.IsWallJumping))
                return;
        }
        //if(_movementCtrls.IsPushbackCooldown)
        //    return;
        if(Abilities.WallGrabCmp != null && Abilities.WallGrabCmp.IsOnWall)
            return;

        deltaMovement = MvmntCmp.Move(deltaMovement);
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


    public void HandleMovingPlatform(ref Vector2 deltaMovement) {
        foreach (RaycastMeta meta in _collisionDetection.VerticalRayMeta) {
            if(!meta.Ray)
                continue;
            if(!meta.HitTag.Equals("Platform"))
                continue;

            //Vector2 pos = meta.Ray.distance * -meta.Direction;
            //this.transform.Translate(pos);
            this.transform.SetParent(meta.Ray.collider.transform);
            return;
        }//foreach

        if(this.transform.parent != null)
            this.transform.SetParent(null);
    }//HandleMovingPlatform


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

    public override bool IsGrounded { get {
            return _collisionDetection.Below;
        }
    }

    //public bool IsOnSlope { get { return this._raycastGround.IsOnSlope; } }
    public bool IsOnSlope { get { return _collisionDetection.IsOnSlope; } }

    /// <summary>
    ///  Player was in the air last frame, but this frame has IsGrounded = true.
    /// IsLanded will become false in the beginning of the next frame.
    /// </summary>
    public bool IsLanded { get; private set; }


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