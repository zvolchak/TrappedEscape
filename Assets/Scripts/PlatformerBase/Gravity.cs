
using UnityEngine;

namespace GHPhysics {
    public class Gravity : MonoBehaviour {

        public float Force = -9.8f;

        /// <summary>
        ///  Current gravity value that is ussed in ApplyGravity
        /// </summary>
        public float CurrentGravity;

        /// <summary> GravityForce value used at the start of the game. </summary>
        protected float origGravity = float.MinValue;

        /* ---------------------- ----------------------- ---------------------- */

        public void Start() {
            if (this.origGravity == float.MinValue)
                this.origGravity = Force;
            this.CurrentGravity = Force;
        }//Start


        /// <summary>
        ///  Apply gravitational force to the deltaMovement vector.
        /// </summary>
        /// <param name="deltaMovement">Vector to apply force to.</param>
        /// <returns></returns>
        public virtual Vector3 Apply(Vector3 deltaMovement) {
            deltaMovement.y += CurrentGravity * Time.deltaTime;
            return deltaMovement;
        }//ApplyGravity


        /// <summary>
        ///  Apply() called from here and asigned to deltaMovement. It is just a
        /// variation of the same function to enable the use of "ref".
        /// </summary>
        /// <param name="deltaMovement"></param>
        public virtual void Apply(ref Vector3 deltaMovement) {
            deltaMovement = this.Apply(deltaMovement);
        }//Apply


        public virtual void SetGravity(float newVal) {
            if (this.origGravity == float.MinValue) {
                this.origGravity = Force;
            }

            this.CurrentGravity = newVal;
        }//SetGravity


        public virtual void ResetGravity() {
            if (this.origGravity == float.MinValue)
                return;
            this.CurrentGravity = this.origGravity;
        }//ResetGravity

    }//class
}//namespace