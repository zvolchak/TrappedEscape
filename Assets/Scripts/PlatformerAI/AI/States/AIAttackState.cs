using System.Collections;
using UnityEngine;
using GHWeaponry;

namespace GHAI {
    namespace AIStates {
        ///<summery>
        ///</summery>
        public class AIAttackState : AIBase {

            private bool bIsAttacking = false;


            public override bool Action() {
                AICtrl.MvmntCmp.SetVelocityX(0);
                if (!bIsAttacking) {
                    AICtrl.StartCoroutine(ChargedAttack());
                }
                return false;
            }

            public override bool Interrupt() {
                return false;
            }


            public IEnumerator ChargedAttack() {
                if (!AICtrl.HolsterCmp.ActiveWeapon.IsCanShoot)
                    yield break;

                float distance = (AICtrl.transform.position - AICtrl.Target.transform.position).magnitude;
                Debug.LogError("FIXME: IsLookingAtTarget definition was changed! Refactor needed...");
                //if(!AICntrl.IsLookingAtTarget && distance >= 3.3f)
                //    AICntrl.DirSwitcherCmp.OnSwitchDirection();

                bIsAttacking = true;
                Holster holster = AICtrl.HolsterCmp;
                Weapon weapon = holster.ActiveWeapon;

                if (AICtrl.AnimationCmp != null) {
                    AICtrl.AnimationCmp.SetTrigger("ChargeAttack");
                }

                float chargeTime = weapon.Props.ChargingTime;
                yield return new WaitForSeconds(chargeTime);

                weapon.Shoot(weapon.transform.rotation);

                if (AICtrl.AnimationCmp != null) {
                    AICtrl.AnimationCmp.SetTrigger("Attack");
                }
                bIsAttacking = false;
            }//ChargedAttack

        }//AIAttackState
    }//namespace AIStates
}//namespace