using UnityEngine;
using GHPlatformerControls;
using GHActor;

namespace GHAbilities {
    ///<summery>
    ///</summery>
    public class SprintControls : AAbility {

        public SprintProps Props;
        public bool IsSprinting;

        private MovementControls _mvmnt;
        private StaminaAttribute _stamina;
        private float sprintDirection = 0;


        public override void Start() {
            base.Start();
            if(_stamina == null)
                _stamina = this.ActorCmp.GetComponent<StaminaAttribute>();
        }//Start


        public override Vector2 Use(Vector2 deltaMovement) {
            if (ActorCmp == null) {
                return deltaMovement;
            }

            if (_stamina != null && (_stamina.Status == 0 || _stamina.IsFullyDrained)) {
                return Stop(deltaMovement);
            }

            if (_mvmnt == null) {
                _mvmnt = ActorCmp.MvmntCmp;
                if (_mvmnt == null)
                    return deltaMovement;
            }

            float direction = Mathf.Sign(ActorCmp.transform.localScale.x);
            if (this.sprintDirection != direction && this.sprintDirection != 0) {
                deltaMovement.x += Props.DirectionAcceleration * direction;
                //FIXME this Garbage logic
                if (this.sprintDirection > 0) {
                    if (deltaMovement.x <= 0)
                        this.sprintDirection = direction;
                } else {
                    if (deltaMovement.x >= 0)
                        this.sprintDirection = direction;
                }//if garbage
            } else {
                deltaMovement.x += Props.Acceleration * direction;
                this.sprintDirection = direction;
            }

            if (Mathf.Abs(deltaMovement.x) >= this.SprintingTopSpeed) {
                deltaMovement.x = this.SprintingTopSpeed * direction;
            }

            IsSprinting = true;
            if (_stamina != null) {
                _stamina.UpdateStamina(-Props.StaminaCost);
            }
            return deltaMovement;
        }//UseAbility


        public override Vector2 Stop(Vector2 deltaMovement) {
            if (!IsSprinting)
                return deltaMovement;

            float direction = Mathf.Sign(ActorCmp.transform.localScale.x);

            float prevVal = deltaMovement.x;
            float newVal = deltaMovement.x - Props.Acceleration * direction;
            if (Mathf.Abs(prevVal) < Mathf.Abs(newVal)) {
                Reset();
                return deltaMovement;
            }

            deltaMovement.x = newVal;
            if (Mathf.Abs(deltaMovement.x) <= _mvmnt.MaxRunSpeed) {
                deltaMovement.x = _mvmnt.MaxRunSpeed * direction;
                this.Reset();
            }
            return deltaMovement;
        }//Stop


        public override void Reset(bool isForceReset = false) {
            IsSprinting = false;
            this.sprintDirection = 0;
        }//Reset


        public float SprintingTopSpeed {
            get {
                if (_mvmnt == null)
                    return -1;
                return _mvmnt.MaxRunSpeed + this.Props.AddToSpeed;
            }
        }

    }//SprintControls

    [System.Serializable]
    public class SprintProps {

        [Tooltip("How much value to add to a regular speed when sprinting.")]
        public float AddToSpeed = 5f;
        [Tooltip("How much stamina per second does the Sprinting costs.")]
        public float StaminaCost = 10f;

        [Tooltip("How fast will character get to top spint speed. Higher value - faster top speed is reached.")]
        public float Acceleration = 0.1f;
        public float DirectionAcceleration = 0.2f;

    }//SprintProps
}//namespace