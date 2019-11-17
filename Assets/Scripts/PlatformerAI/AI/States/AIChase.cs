using GHMisc;
using GHPlatformerControls;
using UnityEngine;

namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIChase : AIBase {

            protected Pathfinder _pathFinder;

            private RaycastGround _raycastGround;
            private RaycastForwardCollision _raycastFrwd;


            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateEnter(animator, stateInfo, layerIndex);
                if (_pathFinder == null)
                    _pathFinder = AICtrl.GetComponent<Pathfinder>();
#if UNITY_EDITOR
                if (_pathFinder == null)
                    Debug.LogError("No PathFinder in AIController object!");
#endif
            }//OnStateEnter


            public override bool Action() {
                if (AICtrl.StateMachineAnimator.GetBool("IsDead")) {
                    Interrupt();
                }

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
                float direction = AICtrl.DirSwitcherCmp.Direction;
                Vector2 distanceVector = (target.position - AICtrl.transform.position);
                //float distance = distanceVector.magnitude;
                float distance = Vector2.Distance(AICtrl.transform.position, target.position);
                if (!AICtrl.IsLookingAtTarget(target) && distance >= AICtrl.MvmntCmp.Velocity.x) {
                    AICtrl.DirSwitcherCmp.OnSwitchDirection();
                    direction = AICtrl.DirSwitcherCmp.Direction;
                }
                Vector2 deltaMovement = Vector2.right * AICtrl.MvmntCmp.RunSpeed * direction;

                var box = AICtrl.ControlledBy.ColliderCmp as BoxCollider2D;
                var size = box.size;
                
                Debug.DrawRay(AICtrl.transform.position, 
                    Vector2.down * (Mathf.Abs(deltaMovement.y) + size.y / 2), 
                    Color.green);

                Debug.DrawRay(AICtrl.transform.position,
                    Vector2.right * (Mathf.Abs(deltaMovement.x)), 
                    Color.green);

                bool isAtX = Mathf.Abs(distanceVector.x) <= Mathf.Abs(deltaMovement.x);
                bool isAtY = false;
                if (distanceVector.y <= 0)
                    isAtY = Mathf.Abs(distanceVector.y) <= Mathf.Abs(deltaMovement.y) + size.y / 2;

                if (AICtrl.LadderClimberCmp.IsClimbing)
                    isAtY = false;
                if (isAtX && !isAtY)
                    UseLadder(ref deltaMovement, Mathf.Sign(distanceVector.normalized.y));

                //Debug.Log(distanceVector.y + " : " + target.position.y + " -> " + isAtY + " : " + deltaMovement.y);
                AICtrl.MvmntCmp.SetVelocity(deltaMovement);

                bool hasArrived = isAtX && isAtY;
                return hasArrived;
            }//GoToTarget


            public void UseLadder(ref Vector2 deltaMovement, float dir) {
                LadderClimber climber = AICtrl.LadderClimberCmp;
                if(climber == null)
                    return;

                if(!climber.IsOnLadder)
                    return;

                climber.OnClimb(1, !climber.IsClimbing);
                deltaMovement.x = 0f;
                deltaMovement.y = AICtrl.MvmntCmp.Velocity.y;
            }//UserLadder


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