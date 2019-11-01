using UnityEngine;

namespace GHAbilities {
    public abstract class AAbility : MonoBehaviour {

        public string InputName;
        public string AnimationState;
        [Tooltip("Who owns this ability. Set from UnityEdutir or call RegisterActor() from script.")]
        public AActor2D ActorParent;

        protected AActor2D _actor;

        public AbilityDelegate.OnUseDelegate EOnUse;

        /* ----------------- FUNCTIONS ----------------- */

        /// <summary>
        ///  Use ability - whatever that might be (add Dashing speed to velocity, or Jumping or whatever).
        /// </summary>
        /// <param name="deltaMovement">Current Veocity/DeltaMovement to apply ability to.</param>
        /// <returns></returns>
        public abstract Vector2 Use(Vector2 deltaMovement);

        /// <summary>
        ///  Stop Use ability and prevent further modifications of the Velocity/deltamovement.
        /// </summary>
        /// <param name="deltaMovement">Current Veocity/DeltaMovement to apply ability to.</param>
        /// <returns></returns>
        public abstract Vector2 Stop(Vector2 deltaMovement);

        /// <summary>
        ///  Reset props of the ability.
        /// </summary>
        public abstract void Reset();


        public virtual void Start() {
            this.init();
        }//Start


        public virtual void RegisterActor(AActor2D actor) {
#if UNITY_EDITOR
            if (ActorParent != null) {
                string msg = "Overwriting ActorCmp from RegisterActor while ActorParent is set from Editor in '{0}'!";
                Debug.LogWarning(string.Format(msg, this.name));
            }
#endif
            _actor = actor;
        }//RegisterActor


        /// <summary>
        /// Something that is called on Start to initiate components states and such.
        /// </summary>
        protected virtual void init() {
            if (ActorCmp == null)
                _actor = GetComponentInParent<AActor2D>();
            if (ActorCmp == null)
                _actor = GetComponent<AActor2D>();
        }//init


        public virtual string GetAbilityName() {
            return this.GetType().Name;
        }//GetAbilityName


        public virtual void PressButton() { IsButtonPressed = true; }
        public virtual void ReleaseButton() { IsButtonPressed = false; }

        public bool IsButtonPressed { get; protected set; }

        public AActor2D ActorCmp {
            get {
                return _actor;
            }//get
        }//PlayerCmp

    }//class Ability

}//namespace