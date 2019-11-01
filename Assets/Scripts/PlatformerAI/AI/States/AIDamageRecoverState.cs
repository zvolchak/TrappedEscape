using UnityEngine;


///<summery>
///</summery>
public class AIDamageRecoverState : AIBase {


    public override bool Action() {
        NPC.MvmntCmp.SetVelocityX(0);
        return true;
    }

    public override bool Interrupt() {
        return false;
    }

}//AIDamageRecoverState
