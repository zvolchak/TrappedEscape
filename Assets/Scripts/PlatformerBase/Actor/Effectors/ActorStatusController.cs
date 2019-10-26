using System.Collections.Generic;
using UnityEngine;

namespace GHActor {

    ///<summery>
    /// Add this component to the Actor object or its child. This allows to apply
    /// and control various StatusEffector.
    ///</summery>
    public class ActorStatusController : MonoBehaviour {

        [Tooltip("Who owns this controller.")]
        public AActor2D Owner;
        public List<string> DebugStatusApplied;

        public List<IStatusEffector> Effectors { get; private set; }


        public void Start() {
            if(Owner == null)
                Owner = GetComponentInParent<AActor2D>();
        }//Start


        public void Update() {
            this.checkEffectorTimeout();
        }//Update


        private void checkEffectorTimeout() {
            if(this.Effectors == null || this.Effectors.Count == 0)
                return;

            List<IStatusEffector> origEffectors = this.Effectors;
            for (int i = 0; i < origEffectors.Count; i++) {
                IStatusEffector effect = origEffectors[i];
                float currDuration = effect.GetCurrentDuration();
                if(currDuration < 0)
                    continue;

                if(currDuration >= effect.GetMaxDuration())
                    RemoveStatusEffect(effect);
            }//for
        }//checkEffectorTImeout


        public void ApplyStatusEffect(IStatusEffector effector) {
            if (this.Effectors == null) {
                this.Effectors = new List<IStatusEffector>();
                return;
            }
            if(this.Effectors.Contains(effector))
                return;
            if (effector == null) {
#if UNITY_EDITOR
                Debug.LogWarning("ApplyStatusEffect: Effector is null! [" + this.name + "]");
#endif
                return;
            }

            effector.ApplyEffect(Owner);
            
            this.Effectors.Add(effector);
            this.DebugStatusApplied.Add(effector.GetName());
        }//effector


        public void RemoveStatusEffect(IStatusEffector effector) {
            if(this.Effectors == null)
                return;
            if(!this.Effectors.Contains(effector))
                return;
            effector.RemoveEffect(this.Owner);
            this.Effectors.Remove(effector);
            this.DebugStatusApplied.Remove(effector.GetName());
        }//RemoveStatusEffect

    }//ActorStatusController
}//namespace