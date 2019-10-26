using UnityEngine;
using GHAbilities;

[RequireComponent(typeof(BoxCollider2D), typeof(CollisionDetection) )]
public class PlatformerControls2d : MonoBehaviour {

    public enum MovementState { standing, normal, walking, running }

    //public float Acceleration = 30f;
    [Tooltip("Normal character speed.")]
    public float NormalSpeed = 10f;
    [Tooltip("Walking speed. (slower than MaxSpeed)")]
    public float WalkSpeed = 5f;
    [Tooltip("Running speed. (faster than MaxSpeed)")]
    public float RunSpeed = 15f;
    public Vector2 Velocity { get { return this.velocity; } }
    public Vector2 velocity;
    public float Gravity = 9.8f;

    public MovementState CurrMovementState { get { return e_MovementState; } }
    public bool IsFacingRight { get { return this.transform.localScale.x == this.origLocalScale.x; } }
    //public Rigidbody2D RigidBodyCmp { get { return _rigidBody; } }
    public float ActiveSpeed { get { return GetActiveMaxSpeed(); } }

    //protected Rigidbody2D           _rigidBody;
    protected BoxCollider2D         _boxCollider;
    protected CollisionDetection    _collisionDetection;
    protected JumpControls          _jumpControls;

    private MovementState e_MovementState;
    private Vector3 origLocalScale;
    private bool bIsCanFlip;
    private bool bIsStopJump;

    /* ************************************************************* */


	void Start () {
        //_rigidBody          = GetComponent<Rigidbody2D>();
        _boxCollider        = GetComponent<BoxCollider2D>();
        _collisionDetection = GetComponent<CollisionDetection>();
        _jumpControls       = GetComponent<JumpControls>();
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -Gravity);

        this.origLocalScale = this.transform.localScale;
        bIsCanFlip = true;
	}//Start


    public void FixedUpdate() {

    }//FixedUpdate


    public void Update () {
        if (Physics2D.gravity.y != -Gravity)
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, -Gravity);

        Vector3 deltaMovement = velocity;

        float horizontalAxis = Input.GetAxis("Horizontal");
        deltaMovement.x = horizontalAxis * Time.deltaTime * ActiveSpeed;

        if (horizontalAxis < 0 && IsFacingRight) {
            Flip();
        }
        if (horizontalAxis > 0 && !IsFacingRight) {
            Flip();
        }

        //if (Input.GetButtonDown(JumpCmp.Props.JumpInputName)) {
        //    deltaMovement.y = JumpCmp.Jump(deltaMovement).y;
        //}

        //if (Input.GetButtonUp(JumpCmp.Props.JumpInputName)) {
        //    bIsStopJump = true;
        //}

        if (bIsStopJump) {
            if (JumpCmp.IsMinHeightReached) {
                deltaMovement.y = JumpCmp.Stop(deltaMovement).y;
                bIsStopJump = false;
            }
        }

        //_jumpControls.OnJump(ref deltaMovement);
        //_jumpControls.JumpCorrection(ref deltaMovement, 1);

        deltaMovement.y += Physics2D.gravity.y * Time.deltaTime;
        _collisionDetection.Move(ref deltaMovement);
        if (Mathf.Abs(deltaMovement.y) <= 0.001)
            deltaMovement.y = 0;

        velocity = deltaMovement;

        transform.Translate(velocity);
    }//Update


    public void SetMovementState(MovementState newState) {
        e_MovementState = newState;
    }//SetMovementState


    public float GetActiveMaxSpeed() {
        switch (e_MovementState) {
            case (MovementState.normal):
                return NormalSpeed;
            case (MovementState.walking):
                return WalkSpeed;
            case (MovementState.running):
                return RunSpeed;
            default:
                return NormalSpeed;
        }//switch
    }//GetActiveMaxSpeed


    public void Flip(bool forceFlip=false) {
        if(!bIsCanFlip && !forceFlip)
            return;
        var localScale = this.transform.localScale;
        localScale.x *= -1;
        this.transform.localScale = localScale;
    }//Flip


    public void SetCanFlip(bool state) {
        bIsCanFlip = state;
    }//SetCanFlip


    public JumpControls JumpCmp {
        get {
            if (_jumpControls == null) {
                _jumpControls = GetComponent<JumpControls>();
            }
            if (_jumpControls == null) {
                _jumpControls = GetComponentInChildren<JumpControls>();
            }
            return _jumpControls;
        }//get
    }//JumpCmp

}//class
