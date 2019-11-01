using UnityEngine;


///<summery>
///</summery>
public class AIChase : AIBase {

    private RaycastGround _raycastGround;
    private RaycastForwardCollision _raycastFrwd;


    public override bool Action() {
        if (NPC.StateMachineAnimator.GetBool("IsDead")) {
            Interrupt();
        }
        Transform target = NPC.Target.transform;
        Transform npcTransform = NPC.transform;
        //float direction = Mathf.Sign((target.position - npcTransform.parent.position).x);
        float direction = NPC.DirSwitcherCmp.Direction;
        float distance = (NPC.transform.position - NPC.Target.transform.position).magnitude;
        if (!NPC.IsLookingAtTarget && distance >= 3.3f) {
            NPC.DirSwitcherCmp.OnSwitchDirection();
            direction = NPC.DirSwitcherCmp.Direction;
        }
        

        //RaycastGroundCmp.OnRaycast();
        //RaycastFrwdCmp.OnRaycast();

        //int rayIndex = direction < 0 ? RaycastGroundCmp.RayHits.Length - 1 : 0;
        //bool isForward = RaycastFrwdCmp.RayHits[1].Ray;
        //if (isForward) {
        //    isForward = RaycastFrwdCmp.RayHits[1].Ray.collider.gameObject != npcTransform.parent.gameObject;
        //}
        ////Check if has ground in front and no walls. Interrupt this action otherwise.
        //if (!RaycastGroundCmp.RayHits[rayIndex].Ray || isForward) {
        //    Interrupt();
        //    return false;
        //}

        //if (caller.AACmp != null)
        //    caller.AACmp.SetWalking(true);

        //if (NPC.SprintCmp != null) {
        //    NPC.SprintCmp.OnSprint(NPC.MvmntCmp);
        //}
        NPC.MvmntCmp.SetVelocityX(NPC.MvmntCmp.RunSpeed * direction);
        //callback?.Invoke(this);

        return true;
    }//Action


    public override bool Interrupt() {
        if (NPC.SprintCmp == null)
            return false;
        //NPC.SprintCmp.Stop(NPC.MvmntCmp);
        //callback?.Invoke(this);

        return true;
    }//Interrupt


    //public RaycastGround RaycastGroundCmp {
    //    get {
    //        if (_raycastGround == null)
    //            _raycastGround = NPC.GetComponentInParent<RaycastGround>();
    //        return _raycastGround;
    //    }//get
    //}//RaycastGroundCmp


    //public RaycastForwardCollision RaycastFrwdCmp {
    //    get {
    //        if (_raycastFrwd == null)
    //            _raycastFrwd = NPC.GetComponentInParent<RaycastForwardCollision>();
    //        return _raycastFrwd;
    //    }//get
    //}//RaycastFrwdCmp

}//AIChase
