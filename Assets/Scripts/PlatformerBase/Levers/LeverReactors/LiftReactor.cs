using UnityEngine;

namespace GHTriggers {

    /// <summary>
    ///  A LiftPlatform component to react(activate/deactivate) to the Lever.
    /// </summary>
    public class LiftReactor : LeverReactor {

        [Tooltip("Either set manually, or leave none and it will pickup the cmp from This GO.")]
        public LiftPlatform LiftToTrigger;


        public void Start() {
            if(LiftToTrigger == null)
                LiftToTrigger = GetComponent<LiftPlatform>();
        }//Start


        public override void Activate() {
            if (LiftToTrigger == null) {
#if UNITY_EDITOR
                Debug.LogWarning("LiftToTrigger for " + this.name + " LiftReactor is not set!");
                return;
#endif
            }
            LiftToTrigger.TriggerMovement();
        }//Activate


        public override void Deactivate() {
            if(LiftToTrigger == null)
                return;

            LiftToTrigger.Respawn();
        }//Deactivate

    }//class

}//namespace
