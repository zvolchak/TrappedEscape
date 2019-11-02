using UnityEngine;

namespace GHAI {
    namespace AIDecisions {
        ///<summery>
        ///</summery>
        [CreateAssetMenu(menuName = "AI/Decisions/IsDead")]
        public class AIDeadDecision : AIDecision {

            //private Damagable _damagable;

            public override bool Decide(AIController controller) {
                var _damagable = controller.DamagableCmp;
                bool isDead = _damagable.IsDead;
                controller.StateMachineAnimator.SetBool(ToggleStateName, isDead);
                return isDead;
            }//Decide


        }//AIDeadDecision
    }//namespace AIDecisions
}//namespace