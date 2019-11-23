using UnityEngine;

namespace GHTriggers {

    ///<summery>
    /// A Platform that is toggled by steping into or by the some sort of Lever
    /// outside this object.
    ///</summery>
    public class LiftPlatform : MovingPlatform {

        [Tooltip("Time to wait before start moving. Use -1 for no delay.")]
        public float DelayTime = 0.5f;
        public LayerMask TriggerTarget;
        //public delegate void TriggerEntered(LiftPlatform lift);

        private bool bHasTriggered;
        private bool bAlreadyEnteredOnce;
        private Coroutine currRoutine;


        public override void Start() {
            base.Start();
        }//Start


        public override void Update() {
            if (!bHasTriggered)
                return;

            Vector2 deltaMovement = Vector2.zero;
            MoveTowardsPoint(ref deltaMovement);
        }//Update


        public override bool MoveTowardsPoint(ref Vector2 deltaMovement) {
            bool bHasDelivered = base.MoveTowardsPoint(ref deltaMovement);
            if (bHasDelivered) {
                TriggerMovement(true);
                bHasTriggered = false;
                bAlreadyEnteredOnce = false;
            }

            return bHasDelivered;
        }//MoveTowardsPoint


        /// <summary>
        ///  Initiate movement for the lift.
        /// </summary>
        public void TriggerMovement(bool isImmediate = true) {
            if (isImmediate)
                bHasTriggered = true;
            else {
                if(this.currRoutine != null)
                    return;

                if(this.DelayTime > 0)
                    this.currRoutine = StartCoroutine(DelayedTriggerTogle(DelayTime));
                else
                    bHasTriggered = true;
            }
        }//ToggleTrigger


        public System.Collections.IEnumerator DelayedTriggerTogle(float delayTime) {
            bIsTriggered = true;
            yield return new WaitForSeconds(delayTime);
            bHasTriggered = true;
            this.currRoutine = null;
        }//DelayedTriggerTogle


        public override void Respawn() {
            base.Respawn();
            bHasTriggered = false;
            bAlreadyEnteredOnce = false;
            this.currRoutine = null;
        }//Respawn


        public void OnTriggerEnter2D(Collider2D collision) {
            bool isTarget = GameUtils.Utils.CompareLayers(TriggerTarget, collision.gameObject.layer);
            if (!isTarget)
                return;
            if (bHasTriggered || bAlreadyEnteredOnce)
                return;

            TriggerSubscribers();

            bAlreadyEnteredOnce = true;
            if(this.currRoutine == null)
                this.currRoutine = StartCoroutine(DelayedTriggerTogle(DelayTime));
        }//OnTriggerEnter2D


        public void OnTriggerExit2D(Collider2D collision) {
            bool isTarget = GameUtils.Utils.CompareLayers(TriggerTarget, collision.gameObject.layer);
            if (!isTarget)
                return;
        }//OnTriggerExit2D

    }//LiftPlatform
}//namespace