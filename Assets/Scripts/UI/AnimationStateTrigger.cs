using UnityEngine;
using GHTriggers;


public class AnimationStateTrigger : TriggerListener{

    public string OnEnterState = "Entered";
    public string OnExitState = "Exited";
    public string OnStayState = "Staying";

    public Animator AnimatorCmp;

    public override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if(!HasEntered)
            return;
        TriggerState(OnEnterState);
    }


    public override void OnTriggerExit2D(Collider2D collision) {
        base.OnTriggerExit2D(collision);
        if(!HasExited)
            return;

        TriggerState(OnExitState);
    }//OnTriggerExit2D


    public void TriggerState(string stateName) {
        if(AnimatorCmp == null)
            return;
        this.AnimatorCmp.SetTrigger(stateName);
    }//TriggerState

}
