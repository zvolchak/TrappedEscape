using GHGameManager;
using GHMisc;
using GHPlatformerControls;
using GHTriggers;
using UnityEngine;

namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIPathwalker : AIBase {

            protected Transform target = null;
            protected Pathfinder _pathFinder;
            protected WaypointControl _wpCtrl;
            protected InputManager _input;
            protected bool bHasInit = false;
            protected bool bIsAtTarget = false;
            protected float timeOfArrival = -1f;
            
            private float climbDir = 0f;

            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateEnter(animator, stateInfo, layerIndex);
                if (_pathFinder == null)
                    _pathFinder = AICtrl.GetComponent<Pathfinder>();
                if(_input == null)
                    _input = AICtrl.ControlledBy.GetComponent<InputManager>();

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

                _wpCtrl.EOnPointReached -= this.OnPointReached;
                _wpCtrl.EOnFirstPointReached -= this.OnFirstPointReached;
                _wpCtrl.EOnLastPointReached -= this.OnLastPointReached;

                _wpCtrl.EOnPointReached += this.OnPointReached;
                _wpCtrl.EOnFirstPointReached += this.OnFirstPointReached;
                _wpCtrl.EOnLastPointReached += this.OnLastPointReached;

                AICtrl.LadderClimberCmp.EOnLadderUnset -= OffTheLadder;
                AICtrl.LadderClimberCmp.EOnLadderUnset += OffTheLadder;
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
                BoxCollider2D box = AICtrl.ControlledBy.ColliderCmp as BoxCollider2D;
                Vector2 size = box.size;
                float direction = AICtrl.DirSwitcherCmp.Direction;
                Vector2 distanceVector = (target.position - AICtrl.transform.position);
                float distance = Vector2.Distance(AICtrl.transform.position, target.position);
                //Vector2 deltaMovement = Vector2.right * AICtrl.MvmntCmp.RunSpeed * direction;
                //Vector3 deltaMovement = AICtrl.MvmntCmp.Velocity;
                //if (deltaMovement.x > distance) {
                //    deltaMovement.x = distance;
                //}
                //_input.SetHorizontalAxis(Mathf.Sign(distanceVector.x));

                //if (!AICtrl.IsLookingAtTarget(target) &&
                //        Mathf.Abs(distanceVector.x) >= (size.x / 2) &&
                //        !AICtrl.LadderClimberCmp.IsClimbing) {
                //    //AICtrl.DirSwitcherCmp.OnSwitchDirection();
                //    //direction = AICtrl.DirSwitcherCmp.Direction;
                //    _input.SetHorizontalAxis(_input.GetHorizontalAxis() * -1);
                //}
                Vector2 inputValue = Vector2.zero;
                inputValue.x = Mathf.Sign(distanceVector.x);

                bool isAtX = Mathf.Abs(distanceVector.x) <= size.x / 2;
                bool isAtY = false;
                if (distanceVector.y <= 0 && !AICtrl.LadderClimberCmp.IsClimbing) {
                    isAtY = Mathf.Abs(distanceVector.y) <= size.y / 2;
                } else if (distanceVector.y <= 0 && AICtrl.LadderClimberCmp.IsClimbing) {
                    if (Mathf.Abs(distanceVector.y) <= size.y / 2)
                        isAtY = AICtrl.ControlledBy.IsGrounded;
                }

                //if (!isAtX)
                //    deltaMovement.x = AICtrl.MvmntCmp.RunSpeed * direction;
                //else
                //    deltaMovement.x = 0;

                ////FIXME: this shouldn't be here. Should be called first by Actor,
                ////but it doesn't (or it is too late).
                //AICtrl.ControlledBy.CollisionDetector.Move(ref deltaMovement);

                if (isAtX && !isAtY) {
                    if (climbDir == 0)
                        climbDir = Mathf.Sign(distanceVector.normalized.y);
                    //if (AICtrl.LadderClimberCmp.IsOnLadder) {
                    //    UseLadder(ref deltaMovement, climbDir);
                    //}
                    //_input.SetVerticalAxis(climbDir);
                    inputValue.y = climbDir;
                    inputValue.x = 0f;
                }

                if (isAtY) {
                    //_input.SetVerticalAxis(0);
                    climbDir = 0f;
                    inputValue.y = climbDir;
                }

                if(AICtrl.LadderClimberCmp.IsClimbing)
                    inputValue.x = 0f;

                ////if (AICtrl.LadderClimberCmp.IsClimbing)
                ////    isAtY = false;
                ////else
                ////    climbDir = 0f;

                //AICtrl.MvmntCmp.SetVelocity(deltaMovement);
                bool hasArrived = isAtX && isAtY;
                if (hasArrived) {
                    this.target = _wpCtrl.GetNext().transform;
                    inputValue.x = 0;
                }

                _input.SetHorizontalAxis(inputValue.x);
                _input.SetVerticalAxis(inputValue.y);


                return hasArrived;
            }//GoToTarget


            public virtual void UseLadder(ref Vector3 deltaMovement, float dir) {
                LadderClimber climber = AICtrl.LadderClimberCmp;
                if (climber == null)
                    return;

                if (!climber.IsOnLadder)
                    return;

                //deltaMovement.x = 0f;
                //deltaMovement.y = climber.Speed * dir;

                climber.OnClimb(dir, !climber.IsClimbing);
                deltaMovement = AICtrl.MvmntCmp.Velocity;
                //deltaMovement.y = AICtrl.MvmntCmp.Velocity.y;
            }//UserLadder


            public void OffTheLadder(Ladder l) {
                AICtrl.MvmntCmp.SetVelocityY(0);
            }


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

                AICtrl.LadderClimberCmp.EOnLadderUnset -= OffTheLadder;
                return true;
            }//Interrupt


            /// <summary>
            ///  Is a WaypointControl event subscribtion callback.
            /// Called when actor reaches a waypoint node.
            /// </summary>
            /// <param name="wpCtrl">A WaypointControl cmp that has called this
            ///                 function or the way that controls the waypoint
            ///                 that has been reached.
            /// </param>
            protected virtual void OnPointReached(WaypointControl wpCtrl) {}

            /// <summary>
            ///  Is a WaypointControl event subscribtion callback.
            /// Called when actor reaches a Last in its path waypoint node.
            /// </summary>
            /// <param name="wpCtrl">A WaypointControl cmp that has called this
            ///                 function or the way that controls the waypoint
            ///                 that has been reached.
            /// </param>
            protected virtual void OnLastPointReached(WaypointControl wpCtrl) {}

            /// <summary>
            ///  Is a WaypointControl event subscribtion callback.
            /// Called when actor reaches a First in its path waypoint node.
            /// </summary>
            /// <param name="wpCtrl">A WaypointControl cmp that has called this
            ///                 function or the way that controls the waypoint
            ///                 that has been reached.
            /// </param>
            protected virtual void OnFirstPointReached(WaypointControl wpCtrl) {}

        }//AIChase
    }//namespace AIStates
}//namespace