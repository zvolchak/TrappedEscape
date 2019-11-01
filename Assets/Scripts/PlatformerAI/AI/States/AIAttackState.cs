using System.Collections;
using UnityEngine;
using GHWeaponry;

///<summery>
///</summery>
public class AIAttackState : AIBase {

    private bool bIsAttacking = false;


    public override bool Action() {
        NPC.MvmntCmp.SetVelocityX(0);
        if (!bIsAttacking) {
            NPC.StartCoroutine(ChargedAttack());
        }
        return false;
    }

    public override bool Interrupt() {
        return false;
    }


    public IEnumerator ChargedAttack() {
        if(!NPC.HolsterCmp.ActiveWeapon.IsCanShoot)
            yield break;

        float distance = (NPC.transform.position - NPC.Target.transform.position).magnitude;
        if(!NPC.IsLookingAtTarget && distance >= 3.3f)
            NPC.DirSwitcherCmp.OnSwitchDirection();

        bIsAttacking = true;
        Holster holster = NPC.HolsterCmp;
        Weapon weapon = holster.ActiveWeapon;

        if (NPC.AnimationCmp != null) {
            NPC.AnimationCmp.SetTrigger("ChargeAttack");
        }

        float chargeTime = weapon.Props.ChargingTime;
        yield return new WaitForSeconds(chargeTime);

        weapon.Shoot(weapon.transform.rotation);

        if (NPC.AnimationCmp != null) {
            NPC.AnimationCmp.SetTrigger("Attack");
        }
        bIsAttacking = false;
    }//ChargedAttack

}//AIAttackState
