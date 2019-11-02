using UnityEngine;

namespace GHAI {
    namespace AIDecisions {
        ///<summery>
        ///</summery>
        [CreateAssetMenu(menuName = "AI/Decisions/Vision")]
        public class AIVision : AIDecision {

            public LayerMask VisionBlockingMask;
            public bool IsDebug = false;


            public override bool Decide(AIController controller) {
                bool isInRange = controller.IsInVisibleRange();
                controller.StateMachineAnimator.SetBool(ToggleStateName, isInRange);
                if (!isInRange)
                    return false;

                bool isVisible = !controller.IsVisionBlocked(VisionBlockingMask, IsDebug);

                controller.StateMachineAnimator.SetBool(ToggleStateName, isVisible);

                return false;
            }//Decide

        }//AIVision
    }//namespace AIDecisions
}//namespace
