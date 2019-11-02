using UnityEngine;

namespace GHAI {
    namespace AIStates {
        public class AIHunt : AIChase {

            private Transform target = null;
            private PathDebugger _pathDebugger;
            private bool bIsAtTarget = false;


            public override bool Action() {
                base.Action();

                if (target == null)
                    this.target = findTarget();
                if (target == null)
                    return false;
                if (bIsAtTarget)
                    return false;

                bIsAtTarget = this.GoToTarget(this.target);
                if (bIsAtTarget)
                    AICtrl.OnLookAround(this.BackToPatrol);
                return true;
            }//Action


            public void BackToPatrol() {
                AICtrl.StateMachineAnimator.SetTrigger("Patrol");
            }//BackToPatrol


            public override bool Interrupt() {
                return false;
            }//Interrupt


            private Transform findTarget() {
                if (_pathDebugger == null)
                    _pathDebugger = PathDebugger.Instance;
                if (_pathDebugger.TargetPoints.Count == 0)
                    return null;

                return _pathDebugger.TargetPoints[0].transform;
            }//findTarget

        }//class
    }//namespace AIStates
}//namespace