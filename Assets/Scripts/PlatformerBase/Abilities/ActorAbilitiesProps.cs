using UnityEngine;
using GHAbilities;

[System.Serializable]
public class ActorAbilitiesProps {

    public AActor2D Actor;
    public CollisionDetection CollisionDetection;
    public JumpControls JumpCmp;
    public SlideAbility SlideCmp;
    public DashAbility DashCmp;
    public WallGrab WallGrabCmp;
    public Crouch CrouchCmp;
    public SprintControls SprintCmp;

    private bool bIsStopJump;


    public void HandleJumping(ref Vector2 deltaMovement) {
        if (this.JumpCmp == null)
            return;

        if (this.WallGrabCmp != null && this.WallGrabCmp.IsOnWall) {
            JumpCmp.Reset();
            return;
        }
        if (this.CollisionDetection.IsFallingThrough)
            return;
        if(!this.CollisionDetection.Below)
            return;

        if (Input.GetButtonDown(JumpCmp.InputName)) {
            deltaMovement.y = JumpCmp.Use(deltaMovement).y;
             if(this.WallGrabCmp != null) this.WallGrabCmp.Reset();
            //_wallJump.Reset();
            if(this.DashCmp != null) this.DashCmp.Reset();
            if(this.SlideCmp != null) this.SlideCmp.Reset();

            if(this.Actor.transform.parent != null)
                this.Actor.transform.SetParent(null);
        }

        if (Input.GetButtonUp(JumpCmp.InputName))
            bIsStopJump = true;

        if (!bIsStopJump)
            return;

        if (JumpCmp.IsMinHeightReached) {
            deltaMovement.y = JumpCmp.Stop(deltaMovement).y;
            bIsStopJump = false;
        }
    }//HandleJumping


    public void HandleCrouching(ref Vector2 deltaMovement) {
        if(this.CrouchCmp == null)
            return;
        if(this.Actor == null)
            return;
        if(this.SlideCmp != null && this.SlideCmp.IsDashing)
            return;

        if (!Actor.IsGrounded) {
            this.CrouchCmp.Stop(deltaMovement);
            return;
        }
        if (Input.GetAxis(this.CrouchCmp.InputName) != 0) {
            deltaMovement = this.CrouchCmp.Use(deltaMovement);
        } else {
            this.CrouchCmp.Stop(deltaMovement);
        }
    }//HandleCrouching


    public void HandleDashAbility(ref Vector2 deltaMovement) {
        if (Actor.IsGrounded)
            return;

        if (DashCmp == null)
            return;

        if (WallGrabCmp.IsOnWall) {
            DashCmp.Reset();
            return;
        }

        if (!DashCmp.IsCanDash)
            return;

        deltaMovement = DashCmp.Use(deltaMovement);
    }//HandleDashAbility


    public void HandleSliding(ref Vector2 deltaMovement) {
        if(this.SlideCmp == null)
            return;

        if (CollisionDetection.Left || CollisionDetection.Right) {
            if (!this.SlideCmp.IsDashing)
                return;
        }

        if (!this.Actor.IsGrounded) {
            //if (this.SlideCmp.IsDashing)
            //    deltaMovement = this.SlideCmp.Stop(deltaMovement);
            return;
        }

        if (this.SlideCmp == null)
            return;

        deltaMovement = this.SlideCmp.Use(deltaMovement);
    }//HandleSliding


    public void HandleSprinting(ref Vector2 deltaMovement) {
        if(this.SprintCmp == null)
            return;

        if(this.SprintCmp.IsSprinting || Input.GetAxis(this.SprintCmp.InputName) != 0)
            deltaMovement = this.SprintCmp.Use(deltaMovement);

        if(Input.GetButtonUp(this.SprintCmp.InputName) || Input.GetAxis(this.SprintCmp.InputName) == 0)
            deltaMovement = this.SprintCmp.Stop(deltaMovement);
    }//HandleSprinting

}//ActorAbilitiesProps