using System.Collections.Generic;
using UnityEngine;
using GHMisc;

namespace GHAI {
    namespace AIStates {
        public class AIHunt : AIChase {

            public ParticleSystem SearchAreaNotifier;

            private Transform target = null;
            private PathDebugger _pathDebugger;
            private bool bIsAtTarget = false;
            private List<INode> huntPath = null;
            private WaypointControl _wpCtrl;
            private float enteredTime = -1f;


            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateEnter(animator, stateInfo, layerIndex);

                if (_wpCtrl == null)
                    _wpCtrl = this.AICtrl.GetComponent<WaypointControl>();

                Vector3 distressPoint = Vector3.one * float.NegativeInfinity;

                if (target == null) {
                    distressPoint = findTarget();
                    Vector3 from = this.AICtrl.ControlledBy.transform.position;
                    this.huntPath = _pathFinder.FindPath(from, distressPoint);
                    if (this.huntPath == null) {
                        //this.Interrupt();
                        //return;
                    }
                    _wpCtrl.SetPatrolPath(this.huntPath);
                }

                //FIXME: GARBAGE for debugging
                if (distressPoint.x != float.NegativeInfinity) {
                    if (SearchAreaNotifier != null) {
                        var particle = Instantiate(SearchAreaNotifier);
                        Vector3 pos = distressPoint;
                        pos.y += 1f;
                        particle.transform.position = pos;
                    }
                }

                this.enteredTime = Time.timeSinceLevelLoad;
            }//OnStateEnter


            public override bool Action() {
                base.Action();
 
                if (Time.timeSinceLevelLoad - this.enteredTime < 1f) {
                    AICtrl.MvmntCmp.SetVelocity(Vector2.zero);
                    return false;
                }

                if (bIsAtTarget)
                    return false;
                if (this.target == null) {
                    this.target = _wpCtrl.GetNext().transform;
                }

                bool hasArrived = this.GoToTarget(this.target);

                if (hasArrived) {
                    bIsAtTarget = _wpCtrl.IsAtLastPoint();
                    this.target = _wpCtrl.GetNext().transform;
                }
                if (bIsAtTarget) {
                    _wpCtrl.ResetPatrolPoints();
                    AICtrl.OnLookAround(this.BackToPatrol);
                }

                return true;
            }//Action


            public void BackToPatrol() {
                AICtrl.StateMachineAnimator.SetTrigger("Patrol");
            }//BackToPatrol


            public override bool Interrupt() {
                bIsAtTarget = false;
                target = null;
                if(_wpCtrl != null)
                    _wpCtrl.ResetPatrolPoints();

                return true;
            }//Interrupt


            private Vector3 findTarget() {
                if (_pathDebugger == null)
                    _pathDebugger = PathDebugger.Instance;
                if (_pathDebugger.TargetPoints.Count == 0)
                    return Vector3.one * float.NegativeInfinity;

                return _pathDebugger.TargetPoints[0].GetPosition();
            }//findTarget


            public void OnDrawGizmos() {

            }//OnDrawGizmos

        }//class
    }//namespace AIStates
}//namespace