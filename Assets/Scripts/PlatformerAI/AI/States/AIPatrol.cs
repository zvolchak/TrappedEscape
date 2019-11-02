using GHMisc;
using GHPlatformerControls;
using UnityEngine;

namespace GHAI {
    namespace AIStates {
        public class AIPatrol : AIBase {

            public float WaitTimeAtPoint = 0f;

            private WaypointControl _wpCtrl;
            private bool bHasInit = false;
            private float timeOfArrival = -1f;


            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateEnter(animator, stateInfo, layerIndex);
                if (AICtrl.StateMachineAnimator.GetBool("IsDead"))
                    Interrupt();

                if (!bHasInit && _wpCtrl == null) {
                    _wpCtrl = AICtrl.GetComponent<WaypointControl>();
                    bHasInit = true;
                }
            }//OnStateEnter


            public override bool Action() {
                //if (AICtrl.StateMachineAnimator.GetBool("IsDead"))
                //    Interrupt();
                //if (!bHasInit && _wpCtrl == null) {
                //    _wpCtrl = AICtrl.GetComponent<WaypointControl>();
                //    bHasInit = true;
                //}
                if (_wpCtrl == null)
                    return false;

                Transform currDest = _wpCtrl.GetActivePoint(AICtrl.ControlledBy.gameObject);
                if (currDest == null) {
                    currDest = _wpCtrl.GetNext(AICtrl.ControlledBy.gameObject);
                    _wpCtrl.SetActivePoint(currDest);
                }

                if (currDest == null)
                    return false;
                Vector3 npcPos = AICtrl.ControlledBy.transform.position;
                Vector3 direction = currDest.position - npcPos;
                //float distance = Vector3.Distance(npcPos, currDest.position);
                float distance = Mathf.Abs((npcPos - currDest.position).x);
                MovementControls mvmt = AICtrl.MvmntCmp;

                if (distance <= mvmt.RunSpeed) {
                    if (this.WaitTimeAtPoint > 0) {
                        if (this.timeOfArrival < 0)
                            this.timeOfArrival = Time.timeSinceLevelLoad;
                        if (Time.timeSinceLevelLoad - this.timeOfArrival < this.WaitTimeAtPoint)
                            return false;
                        else
                            this.timeOfArrival = -1f;
                    }//if waittime

                    currDest = _wpCtrl.GetNext(AICtrl.ControlledBy.gameObject);
                    _wpCtrl.SetActivePoint(currDest);
                }// if distance

                SwitchDirection dirSwitcher = AICtrl.DirSwitcherCmp;
                dirSwitcher.OnSwitchDirection(Mathf.Sign(direction.x));
                mvmt.SetVelocityX((mvmt.RunSpeed) * dirSwitcher.Direction);

                return true;
            }//Action

            public override bool Interrupt() {
                if (_wpCtrl != null)
                    _wpCtrl.SetActivePoint(null);

                return true;
            }//Interrupt
        }//class
    }//namespace AIStates
}//namespace