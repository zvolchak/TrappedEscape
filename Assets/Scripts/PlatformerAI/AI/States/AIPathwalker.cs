using GHMisc;
using GHPlatformerControls;
using UnityEngine;

namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIPathwalker : AIBase {

            protected Transform target = null;
            protected Pathfinder _pathFinder;
            protected WaypointControl _wpCtrl;
            protected bool bHasInit = false;
            protected bool bIsAtTarget = false;
            protected float timeOfArrival = -1f;


            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateEnter(animator, stateInfo, layerIndex);
                if (_pathFinder == null)
                    _pathFinder = AICtrl.GetComponent<Pathfinder>();

                #if UNITY_EDITOR
                    if (_pathFinder == null)
                        Debug.LogError("No PathFinder in AIController object!");
                #endif

                if (AICtrl.StateMachineAnimator.GetBool("IsDead"))
                    Interrupt();

                if (!bHasInit && _wpCtrl == null) {
                    _wpCtrl = AICtrl.GetComponent<WaypointControl>();
                    bHasInit = true;
                }

                //Vector3 from = _wpCtrl.patrolPoints[0].transform.position;
                //Vector3 to = _wpCtrl.patrolPoints[1].transform.position;
                //var path = _pathFinder.FindPath(from, to);
                //_wpCtrl.SetPatrolPoints(path);
                //this.target = _wpCtrl.patrolPoints[0].transform;

                _wpCtrl.EOnPointReached -= this.OnPointReached;
                _wpCtrl.EOnFirstPointReached -= this.OnFirstPointReached;
                _wpCtrl.EOnLastPointReached -= this.OnLastPointReached;

                _wpCtrl.EOnPointReached += this.OnPointReached;
                _wpCtrl.EOnFirstPointReached += this.OnFirstPointReached;
                _wpCtrl.EOnLastPointReached += this.OnLastPointReached;
            }//OnStateEnter


            public override bool Action() {
                if (AICtrl.StateMachineAnimator.GetBool("IsDead")) {
                    Interrupt();
                    return false;
                }

                if (_wpCtrl == null) {
                    #if UNITY_EDITOR
                        Debug.LogError("AIPathwalker has no WaypointControl set!");
                    #endif
                    return false;
                }

                GoToTarget(this.target);

                return true;
            }//Action


            public virtual bool GoToTarget(Transform target) {
                if (this.target == null)
                    return false;

                float direction = AICtrl.DirSwitcherCmp.Direction;
                Vector2 distanceVector = (target.position - AICtrl.transform.position);
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

                AICtrl.MvmntCmp.SetVelocity(deltaMovement);

                bool hasArrived = isAtX && isAtY;
                if (hasArrived) {
                    this.target = _wpCtrl.GetNext().transform;
                }

                return hasArrived;
            }//GoToTarget


            public void UseLadder(ref Vector2 deltaMovement, float dir) {
                LadderClimber climber = AICtrl.LadderClimberCmp;
                if (climber == null)
                    return;

                if (!climber.IsOnLadder)
                    return;

                climber.OnClimb(1, !climber.IsClimbing);
                deltaMovement.x = 0f;
                deltaMovement.y = AICtrl.MvmntCmp.Velocity.y;
            }//UserLadder


            public override bool Interrupt() {
                //if (AICtrl.SprintCmp == null)
                //    return false;
                if (_wpCtrl != null)
                    _wpCtrl.SetActivePoint(null);
                this.timeOfArrival = -1f;
                //NPC.SprintCmp.Stop(NPC.MvmntCmp);
                //callback?.Invoke(this);

                _wpCtrl.EOnPointReached -= this.OnPointReached;
                _wpCtrl.EOnFirstPointReached -= this.OnFirstPointReached;
                _wpCtrl.EOnLastPointReached -= this.OnLastPointReached;

                return true;
            }//Interrupt


            protected virtual void OnPointReached(WaypointControl wpCtrl) {}
            protected virtual void OnLastPointReached(WaypointControl wpCtrl) {}
            protected virtual void OnFirstPointReached(WaypointControl wpCtrl) {}

        }//AIChase
    }//namespace AIStates
}//namespace