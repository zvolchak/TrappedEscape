using UnityEngine;


namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIDeadState : AIBase {

            public override bool Action() {
                AICtrl.MvmntCmp.SetVelocityX(0);
                if (AICtrl.ColliderCmp != null) {
                    Destroy(AICtrl.ColliderCmp.gameObject);
                    //NPC.ControlledBy.RBCmp.isKinematic = true;
                    //NPC.ControlledBy.RBCmp.velocity = Vector2.zero;
                    //NPC.ColliderCmp.gameObject.SetActive(false);
                    //NPC.ColliderCmp.enabled = false;
                    //NPC.ColliderCmp.isTrigger = true;
                }
                return true;
            }//Action

            public override bool Interrupt() {
                if (AICtrl.ColliderCmp != null) {
                    //NPC.ColliderCmp.gameObject.SetActive(true);
                    AICtrl.ColliderCmp.enabled = true;
                    AICtrl.ColliderCmp.isTrigger = false;
                }
                return true;
            }//Interrupt

        }//AIDeadState
    }//namespace AIStates
}//namespace
