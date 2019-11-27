using GHMisc;
using GHPlatformerControls;
using System.Collections;
using UnityEngine;

namespace GHAI {
    namespace AIStates {
        public class AIPatrol : AIPathwalker {

            private bool bUsePatrolPath = false;
            private bool bCanUseWaypoints = false;
            private bool deferPointSwitch = false;
            private WaypointControl wp;


            private void init() {
                if(!_wpCtrl.bHasInit)
                    return;

                _wpCtrl.ResetPatrolPoints();
                bUsePatrolPath = false;
                Vector3 from = this.AICtrl.ControlledBy.transform.position;
                Vector3 to = _wpCtrl.PatrolPath[0].transform.position;
                var path = _wpCtrl.PathfinderCmp.FindPath(from, to);
                _wpCtrl.SetPatrolPath(path);
                this.target = _wpCtrl.PatrolPath[0].transform;
                bCanUseWaypoints = true;
            }//init


            public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateEnter(animator, stateInfo, layerIndex);
                this.init();
            }//OnStateEnter


            public override bool Action() {
                if(!bCanUseWaypoints)
                    this.init();
                if(!bCanUseWaypoints)
                    return false;

                bool isNoAction = !base.Action();
                if(isNoAction)
                    return false;

                //if (deferPointSwitch) {
                //    if (!_wpCtrl.IsWaitingAtWaypoint) {
                //        if (this.wp != null) {
                //            this.AICtrl.MvmntCmp.SetVelocityX(0);
                //        }
                //    }
                //}

                return true;
            }//Action


            protected override void OnLastPointReached(WaypointControl wpCtrl) {
                base.OnLastPointReached(wpCtrl);
                if (wpCtrl.IsWaitingAtWaypoint) {
                    this.deferPointSwitch = true;
                    return;
                }
                wpCtrl.FlipIterationIndex();

                if (!bUsePatrolPath) {
                    var t = wpCtrl.GetActivePoint();
                    wpCtrl.ResetPatrolPoints();
                    bUsePatrolPath = true;
                    this.target = wpCtrl.GetNext(t).transform;
                }
            }//OnLastPointReached


            protected override void OnFirstPointReached(WaypointControl wpCtrl) {
                base.OnFirstPointReached(wpCtrl);
                if (wpCtrl.IsWaitingAtWaypoint) {
                    this.deferPointSwitch = true;
                    return;
                }

                wpCtrl.FlipIterationIndex();
            }//OnFirstPointReached


            public override bool Interrupt() {
                return base.Interrupt();
            }//Interrupt

        }//class
    }//namespace AIStates
}//namespace