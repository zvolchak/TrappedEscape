using UnityEngine;
using GHWeaponry;


///<summery>
/// Decided whether or not actor in a range to attack and whether or not
/// it can attack.
///</summery>
[CreateAssetMenu(menuName = "AI/Decisions/Attack")]
public class AIAttackDecision : AIDecision {

    public LayerMask VisionBlockingMask;


    public override bool Decide(AIController controller) {
        if(controller == null)
            return false;

        Holster holster = controller.HolsterCmp;
        if (holster.ActiveWeapon == null) {
#if UNITY_EDITOR
            Debug.LogWarning(this.name + " has no ActiveWeapon to make Attack Decision!");
#endif
            return false;
        }

        float range = holster.ActiveWeapon.Props.Range;
        bool isInRange = holster.ActiveWeapon.Props.IsInRange(controller.Target.transform.position);
        //if(isInRange)
        //    isInRange = controller.IsVisionBlocked(VisionBlockingMask);

        controller.StateMachineAnimator.SetBool(this.ToggleStateName, isInRange);

        return isInRange;
    }//Decide

}//AIAttackDecision
