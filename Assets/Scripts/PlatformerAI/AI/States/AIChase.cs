using UnityEngine;

namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIChase : AIBase {

            protected Pathfinder _pathFinder;

            private RaycastGround _raycastGround;
            private RaycastForwardCollision _raycastFrwd;


            public override bool Action() {
                if (AICtrl.StateMachineAnimator.GetBool("IsDead")) {
                    Interrupt();
                }

                if (_pathFinder == null)
                    _pathFinder = AICtrl.GetComponent<Pathfinder>();
#if UNITY_EDITOR
                if (_pathFinder == null)
                    Debug.LogError("No PathFinder in AIController object!");
#endif

                //GoToTarget(AICntrl.Target.transform);

                //Transform target = NPC.Target.transform;
                //Transform npcTransform = NPC.transform;
                ////float direction = Mathf.Sign((target.position - npcTransform.parent.position).x);
                //float direction = NPC.DirSwitcherCmp.Direction;
                //float distance = (NPC.transform.position - NPC.Target.transform.position).magnitude;
                //if (!NPC.IsLookingAtTarget && distance >= 3.3f) {
                //    NPC.DirSwitcherCmp.OnSwitchDirection();
                //    direction = NPC.DirSwitcherCmp.Direction;
                //}


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
                //NPC.MvmntCmp.SetVelocityX(NPC.MvmntCmp.RunSpeed * direction);
                //callback?.Invoke(this);

                return true;
            }//Action


            public virtual bool GoToTarget(Transform target) {
                //Transform target = NPC.Target.transform;
                //Transform npcTransform = NPC.transform;
                //float direction = Mathf.Sign((target.position - npcTransform.parent.position).x);
                float direction = AICtrl.DirSwitcherCmp.Direction;
                Vector2 distanceVector = (AICtrl.transform.position - target.position);
                float distance = distanceVector.magnitude;
                if (!AICtrl.IsLookingAtTarget(target) && distance >= 3.3f) {
                    AICtrl.DirSwitcherCmp.OnSwitchDirection();
                    direction = AICtrl.DirSwitcherCmp.Direction;
                }
                AICtrl.MvmntCmp.SetVelocityX(AICtrl.MvmntCmp.RunSpeed * direction);
                return Mathf.Abs(distanceVector.x) <= Mathf.Abs(AICtrl.MvmntCmp.Velocity.x) + 0.1f;
            }//GoToTarget


            public override bool Interrupt() {
                if (AICtrl.SprintCmp == null)
                    return false;
                //NPC.SprintCmp.Stop(NPC.MvmntCmp);
                //callback?.Invoke(this);

                return true;
            }//Interrupt

        }//AIChase
    }//namespace AIStates
}//namespace