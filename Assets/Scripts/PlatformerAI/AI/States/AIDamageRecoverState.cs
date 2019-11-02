using UnityEngine;

namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIDamageRecoverState : AIBase {

            public override bool Action() {
                AICtrl.MvmntCmp.SetVelocityX(0);
                return true;
            }

            public override bool Interrupt() {
                return false;
            }

        }//AIDamageRecoverState
    }//namespace AIState
}//namespace