using UnityEngine;
using GHPlatformerControls;

namespace GHActor {
    [System.Serializable]
    public class StaggerEffect : AStatusEffector {

        [Tooltip("Speed percentage of the actor's MaxSpeed during stagger.")]
        [Range(0f, 1f)] public float StaggerSpeed = 0.6f;


        public override bool ApplyEffect(AActor2D actor) {
            base.ApplyEffect(actor);
            MovementControls actorMvmnt = actor.MvmntCmp;
            if (actorMvmnt == null)
                return false;

            actorMvmnt.SetMaxSpeed(actorMvmnt.MaxRunSpeed * StaggerSpeed);
            return true;
        }//ApplyEffect


        public override bool RemoveEffect(AActor2D actor) {
            base.RemoveEffect(actor);
            MovementControls actorMvmnt = actor.MvmntCmp;
            if (actorMvmnt == null)
                return false;

            actorMvmnt.ResetMaxSpeed();

            return true;
        }//RemoveEffect

    }//class
}//namespace