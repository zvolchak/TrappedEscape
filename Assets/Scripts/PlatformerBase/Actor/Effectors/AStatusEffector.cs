using UnityEngine;

namespace GHActor {

    [System.Serializable]
    public abstract class AStatusEffector : IStatusEffector {

        public string StatusName;
        public float Duration = 1f;
        protected float timeOfApply = -1f;


        public virtual bool ApplyEffect(AActor2D actor) {
            this.timeOfApply = Time.timeSinceLevelLoad;
            return true;
        }//ApplyEffect


        public virtual bool RemoveEffect(AActor2D actor) {
            this.timeOfApply = -1f;
            return true;
        }//RemoveEffect


        public virtual float GetCurrentDuration() {
            if (this.timeOfApply < 0)
                return -1f;

            return Time.timeSinceLevelLoad - this.timeOfApply;
        }

        public virtual float GetMaxDuration() {
            return this.Duration;
        }

        public string GetName() {
            if (StatusName == null || StatusName == "")
                return this.GetType().Name;
            return this.StatusName;
        }
    }//class
}//namespace