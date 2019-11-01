using UnityEngine;
using GHAbilities;

public class CharacterAnimator : MonoBehaviour{

    public CharacterAnimatorProps Props;

    private bool bHasRegistered;
    private Animator _animator;

    public Player PlayerCmp => Player.Instance;
    private Crouch _crouchCmp;


    public virtual void Start() {
    }//start

    public void Update() {
        if (Player.Instance.IsCanControl) {
            AnimatorCmp.StopPlayback();
        }
    }//update

    public void LateUpdate() {
        if (!bHasRegistered) {
            bHasRegistered = InitEvents();
        }//if
    }//LateUpdate


    public virtual bool InitEvents() {
        if (Player.Instance == null)
            return false;

        this.subscribe();

        return true;
    }//InitEvents


    public void SetBool(string name, bool state) {
        if(_animator == null)
            return;
        _animator.SetBool(name, state);
    }//SetBool


    public void SetTrigger(string name) {
        if(_animator == null)
            return;
        if (AnimatorCmp.GetAnimatorTransitionInfo(0).anyState)
            return;
        _animator.SetTrigger(name);
    }//SetTrigger


    public virtual void PlayJumpAnimation() {
        if(AnimatorCmp == null)
            return;
        AnimatorCmp.SetTrigger(Props.Jumped);
    }//PlayJumpAnimation


    public virtual void SetLandedTrigger() {
        if(AnimatorCmp == null)
            return;
        if (AnimatorCmp.GetAnimatorTransitionInfo(0).anyState)
            return;
        AnimatorCmp.SetTrigger(Props.Landed);
    }//PlayJumpAnimation


    public virtual void SetWallGrabTrigger() {
        if (AnimatorCmp == null)
            return;
        if (AnimatorCmp.GetAnimatorTransitionInfo(0).anyState)
            return;
        AnimatorCmp.SetTrigger(Props.OnWallGrabbed);
    }//PlayJumpAnimation


    public virtual void SetDashedTrigger(AAbility ability) {
        if (AnimatorCmp == null)
            return;
        if(AnimatorCmp.GetAnimatorTransitionInfo(0).anyState)
            return;
        AnimatorCmp.SetTrigger(Props.Dashed);
    }//PlayJumpAnimation


    public virtual void SetVelocity(float val) {
        if(AnimatorCmp == null)
            return;
        AnimatorCmp.SetFloat(this.Props.Speed, val);
    }//SetVelocity


    public virtual void SetGrounded(bool state) {
        if (AnimatorCmp == null)
            return;
        AnimatorCmp.SetBool(this.Props.IsGrounded, state);
    }//SetVelocity


    public virtual void SetDashing(bool state) {
        if (AnimatorCmp == null)
            return;
        AnimatorCmp.SetBool(this.Props.Dashing, state);
    }//SetVelocity


    private void groundStateListenerInvoke(bool prevState, bool newState) {
        this.SetGrounded(newState);
    }//groundStateListenerInvoke


    private void velocityUpdateInvoke(Vector2 prevVelocity, Vector2 newVelocity) {
        if (AnimatorCmp == null)
            return;
        AnimatorCmp.SetFloat(this.Props.VelocityY, newVelocity.y);
    }//velocityUpdateInvoke


    private void onWallStateInvoke(WallGrab wallJumpCmp) {
        if (AnimatorCmp == null)
            return;
        AnimatorCmp.SetBool(this.Props.OnWall, wallJumpCmp.IsOnWall);
    }//onWallStateInvoke


    private void onWallJumpInvoke(WallGrab cmp) {
        if (AnimatorCmp == null)
            return;
        if (cmp.IsBackflipJumping)
            AnimatorCmp.SetTrigger("Backflip");
        else
            AnimatorCmp.SetTrigger(this.Props.Jumped);
    }


    private void onCrouchInvoke(AAbility cmp) {
        if (_crouchCmp is null) {
            _crouchCmp = cmp as Crouch;
        }
        this.SetBool(cmp.AnimationState, _crouchCmp.IsCrouching);
        if (_crouchCmp.IsCrouching) {
            this.SetTrigger("Crouched");
        }
    }


    private void onTriggerInvoke(AAbility cmp) {
        this.SetTrigger(cmp.AnimationState);
    }//onTriggerInvoke


    public void OnDisable() {
        this.unsubscribe();

        bHasRegistered = false;
    }//OnDisable


    private void unsubscribe() {
        ActorAbilitiesProps abilities = PlayerCmp.Abilities;

        //abilities.JumpCmp.EOnUse -= this.onTriggerInvoke;
        //PlayerCmp.OnLandedListeners -= SetLandedTrigger;
        //PlayerCmp.EGroundedStateListener -= groundStateListenerInvoke;
        //PlayerCmp.EVelocityUpdateListeners -= velocityUpdateInvoke;
        //abilities.WallGrabCmp.EOnWallGrab -= onWallStateInvoke;
        //abilities.WallGrabCmp.EOnWallJump -= onWallJumpInvoke;
        //abilities.DashCmp.EOnUse -= SetDashedTrigger;
        //PlayerCmp.Abilities.CrouchCmp.EOnUse -= onCrouchInvoke;
    }//unsubscribe


    private void subscribe() {
        this.unsubscribe();

        ActorAbilitiesProps abilities = PlayerCmp.Abilities;

        //abilities.JumpCmp.EOnUse += onTriggerInvoke;
        //PlayerCmp.OnLandedListeners += SetLandedTrigger;
        //PlayerCmp.EGroundedStateListener += groundStateListenerInvoke;
        //PlayerCmp.EVelocityUpdateListeners += velocityUpdateInvoke;
        //abilities.WallGrabCmp.EOnWallGrab += onWallStateInvoke;
        //abilities.WallGrabCmp.EOnWallJump += onWallJumpInvoke;
        //abilities.DashCmp.EOnUse += SetDashedTrigger;
        //PlayerCmp.Abilities.CrouchCmp.EOnUse += onCrouchInvoke;
    }//subscribe

    /********************************** GETTERS/SETTERS **********************************/

    public Animator AnimatorCmp {
        get {
            if(_animator == null)
                _animator = GetComponent<Animator>();

            return _animator;
        }//get
    }//AnimatorCmp

}//Class


[System.Serializable]
public class CharacterAnimatorProps{

    public string Walking = "IsWalking";
    public string Idle = "IsIdle";
    public string IsGrounded = "Grounded";
    public string Jumped = "Jumped";
    public string Landed = "Landed";
    public string VelocityY = "VelocityY";
    public string Speed = "VelocityX";
    public string Dashing = "IsDashing";
    public string Dashed = "Dashed";
    public string OnWall = "IsOnWall";
    public string OnWallGrabbed = "WallGrabbed";
    public string[] DamageReact;
    public string CritHitted = "CritHit";
    public string Died = "Died";
    public string[] DieTrigger = new string[] { "Died_1", "Died_2" };


}//class