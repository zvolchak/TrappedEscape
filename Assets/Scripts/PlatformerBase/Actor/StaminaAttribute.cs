using UnityEngine;
using static GHDelegates.CommonDelegates;

namespace GHActor {
    public class StaminaAttribute: MonoBehaviour {

        [Tooltip("Max amount of stamina.")]
        public float Amount = 10f;
        [Tooltip("How long to wait before stamina start restoring.")]
        public float AutoRestoreTimeout = 0.5f;
        public float TimeoutBeforeRestore = 0.5f;
        public float RestoreAmountPerTick = 0.01f;

        public float Status;
        public bool IsRestoring => timeOfDrained != -1;
        public bool IsAutorestore { get {
                if (this.timeOfLastStaminaUpdate == -1)
                    return false;
                float timeDiff = Time.timeSinceLevelLoad - this.timeOfLastStaminaUpdate;
                return timeDiff >= AutoRestoreTimeout;
            }//get
        }//isAutorestore
        public bool IsFullyDrained { get; private set; }

        private float timeOfDrained = -1f;
        private float timeOfLastStaminaUpdate = -1f;

        /* ---------------------- EVENTS ---------------------- */

        /// <summary>
        /// Signature: (float currentStatus)
        /// </summary>
        public ValueDelegate EOnStaminaUpdated;

        /// <summary>
        /// Event called when the stamina is fully drained and can't be used
        /// until it fully restored.
        /// </summary>
        public SimpleDelegate EOnFullyDrained;


        /// <summary>
        /// Called when the stamina is fully restored and is invoked before Reset().
        /// </summary>
        public SimpleDelegate EOnFullyRestored;
 

        /* ---------------------- ------------------- ---------------------- */

        public void Start() {
            this.Status = Amount;
        }//Start


        public void Update() {
            if (IsRestoring || this.IsAutorestore) {
                this.onStaminaRestore();
            }
        }//Update


        private void onStaminaRestore() {
            if (TimeoutBeforeRestore > 0) {
                if (Time.timeSinceLevelLoad - this.timeOfDrained < TimeoutBeforeRestore)
                    return;
            }//if timeout

            this.UpdateStamina(RestoreAmountPerTick);
        }//onStaminaRestore


        public void UpdateStamina(float byAmount) {
            this.Status += byAmount;
            if(Mathf.Sign(byAmount) < 0 && this.Status > 0)
                ResetFlags();

            if (this.Status > Amount) {
                EOnFullyRestored?.Invoke();
                Reset();
            }
            if (this.Status < 0) {
                this.Status = 0;
                this.IsFullyDrained = true;
                this.timeOfDrained = Time.timeSinceLevelLoad;
                EOnFullyDrained?.Invoke();
            }

            if(Mathf.Sign(byAmount) < 0)
                this.timeOfLastStaminaUpdate = Time.timeSinceLevelLoad;


            EOnStaminaUpdated?.Invoke(this.Status);
        }//UpdateStamina


        public void Reset() {
            this.Status = Amount;
            ResetFlags();
        }//Reset


        public void ResetFlags() {
            this.IsFullyDrained = false;
            this.timeOfDrained = -1f;
            this.timeOfLastStaminaUpdate = -1f;
        }
    }//class
}//namespace
