using UnityEngine;
using GHPlatformerControls;

///<summery>
///</summery>
public class AIWander : AIBase {

    private float timeSinceLastWander;


    public override bool Action() {
        if (NPC.StateMachineAnimator.GetBool("IsDead")) {
            Interrupt();
        }
        MovementControls mvmt = NPC.MvmntCmp;
        SwitchDirection dirSwitcher = NPC.DirSwitcherCmp;

        mvmt.SetVelocityX((mvmt.RunSpeed / 2) * dirSwitcher.Direction);
        //if(this.origAnimatorSpeed == -1)
        //    this.origAnimatorSpeed = AACmp.AnimatorCmp.speed;

        //AACmp.AnimatorCmp.speed = this.wanderAnimatorSpeed;

        float timePassed = Time.timeSinceLevelLoad - this.timeSinceLastWander;
        if (timePassed <= 1)
            return false;

        this.timeSinceLastWander = Time.timeSinceLevelLoad;
        int randDir = Random.Range(0, 2);
        randDir = randDir == 0 ? -1 : 1;
        dirSwitcher.OnSwitchDirection(randDir);
        return true;
    }//Action


    public override bool Interrupt() {
        this.timeSinceLastWander = -1f;
        NPC.MvmntCmp.SetVelocityX(0f);

        return true;
    }//Interrupt
}//AIWander
