using UnityEngine;


///<summery>
///</summery>
public class AIDeadState : AIBase {

    public override bool Action() {
        NPC.MvmntCmp.SetVelocityX(0);
        if (NPC.ColliderCmp != null) {
            Destroy(NPC.ColliderCmp.gameObject);
            //NPC.ControlledBy.RBCmp.isKinematic = true;
            //NPC.ControlledBy.RBCmp.velocity = Vector2.zero;
            //NPC.ColliderCmp.gameObject.SetActive(false);
            //NPC.ColliderCmp.enabled = false;
            //NPC.ColliderCmp.isTrigger = true;
        }
        return true;
    }//Action

    public override bool Interrupt() {
        if (NPC.ColliderCmp != null) {
            //NPC.ColliderCmp.gameObject.SetActive(true);
            NPC.ColliderCmp.enabled = true;
            NPC.ColliderCmp.isTrigger = false;
        }
        return true;
    }//Interrupt

}//AIDeadState
