using GHMisc;
using GHPlatformerControls;
using UnityEngine;

namespace GHAI {
    namespace AIStates {
        public class AIPatrol : AIPathwalker {

            private bool bUsePatrolPath = false;
            private bool bCanUseWaypoints = false;


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
                //Vector3 from = _wpCtrl.patrolPoints[0].transform.position;
                //Vector3 to = _wpCtrl.patrolPoints[1].transform.position;
                //var path = _pathFinder.FindPath(from, to);
                //_wpCtrl.SetPatrolPoints(path);
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

                return true;
            }//Action


            protected override void OnLastPointReached(WaypointControl wpCtrl) {
                base.OnLastPointReached(wpCtrl);
                wpCtrl.FlipIterationIndex();

                if (!bUsePatrolPath) {
                    wpCtrl.ResetPatrolPoints();
                    bUsePatrolPath = true;
                    this.target = wpCtrl.GetNext().transform;
                }
            }//OnLastPointReached


            protected override void OnFirstPointReached(WaypointControl wpCtrl) {
                base.OnFirstPointReached(wpCtrl);
                wpCtrl.FlipIterationIndex();
            }//OnFirstPointReached


            public override bool Interrupt() {
                return base.Interrupt();
            }//Interrupt

        }//class
    }//namespace AIStates
}//namespace