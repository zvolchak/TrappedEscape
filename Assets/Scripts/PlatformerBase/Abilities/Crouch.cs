using UnityEngine;
using GHPlatformerControls;

namespace GHAbilities {
    public class Crouch : AAbility {

        public BoxCollider2D Collider;
        public MovementControls MvmntCmp;
        public CollisionDetection CollisionDetector;
        public CrouchProps Props;

        public bool IsCrouching { get; private set; }

        private Bounds origBounds;
        private int standUpFlagThisFrame = 0;


        public void FixedUpdate() {
            this.standUpFlagThisFrame = 0;
        }//FixedUpdate


        public override void Start() {
            base.Start();
            Collider = ActorParent.ColliderCmp as BoxCollider2D;
            this.origBounds.size = Vector3.one * -1;
        }//Start


        public override void Reset(bool isForceReset = false) {
        }//Reset


        /// <summary>
        ///  Reset original BoxCollider bounds and reset saved bounds
        /// variable to nothing, so the next time it assumes new BoxCollider
        /// bounds to save.
        /// </summary>
        /// <param name="deltaMovement"></param>
        /// <returns></returns>
        public override Vector2 Stop(Vector2 deltaMovement) {
            if (!IsCrouching)
                return deltaMovement;
            bool canStandUp = this.CheckCanStandUp();
            if (!canStandUp)
                return deltaMovement;

            if (this.origBounds.size != Vector3.one * -1) {
                Collider.size = this.origBounds.size;
                Collider.offset = this.origBounds.center;
                this.origBounds.size = Vector3.one * -1;
            }

            if (IsCrouching) {
                IsCrouching = false;
                this.EOnUse?.Invoke(this);
            }

            return deltaMovement;
        }//Stop


        /// <summary>
        ///  Shoot couple rays upwards to check if there is a collision. Be careful
        /// not to call this function too many times, cause Raycasts...
        /// Note: True returned if collision detector not set.
        /// </summary>
        /// <param name="isForceCheck">[optional] Force check the state and shoot
        ///                         rays again, even though already has been called
        ///                         this frame.
        ///                         </param>
        /// <returns></returns>
        public bool CheckCanStandUp(bool isForceCheck = false) {
            CollisionDetection cd = this.CollisionDetector;
            if (CollisionDetector == null)
                return true;
            //This function already has been called this frame. Don't waste them
            //CPU power. Return the value that has been generated.
            if (this.standUpFlagThisFrame != 0 && !isForceCheck) {
                return this.standUpFlagThisFrame >= 1;
            }

            float rayLength = this.origBounds.size.y / 2;
            for (int i = 0; i < cd.Props.HorizontalRays; i++) {
                var ray = cd.VerticalRay(rayLength, i);
                if (ray.Ray) {
                    this.standUpFlagThisFrame = -1;
                    break;
                }
            }
            return this.standUpFlagThisFrame >= 0;
        }//CheckCanStandUp


        public override Vector2 Use(Vector2 deltaMovement) {
            if (Collider == null)
                Collider = ActorParent.ColliderCmp as BoxCollider2D;
            if (this.origBounds.size == Vector3.one * -1) {
                this.origBounds.size = Collider.size;
                this.origBounds.center = Collider.offset;
            }

            var origSize = this.origBounds.size;
            var origOffset = this.origBounds.center;
            var reducedSize = origSize.y / this.Props.ReduceColliderBy;
            float yDiff = origSize.y - reducedSize;

            Collider.size = new Vector2(origSize.x, reducedSize);
            Collider.offset = new Vector2(origOffset.x, origOffset.y - yDiff / 2);

            if (!IsCrouching) {
                IsCrouching = true;
                this.EOnUse?.Invoke(this);
            }

            float maxSpeed = this.MvmntCmp.MaxRunSpeed / this.Props.ReduceSpeedBy;
            if (Mathf.Abs(deltaMovement.x) > maxSpeed)
                deltaMovement.x = maxSpeed * Mathf.Sign(deltaMovement.x);

            return deltaMovement;
        }//Use

    }//MovementAbility


    [System.Serializable]
    public class CrouchProps {

        public float ReduceColliderBy = 2f;
        public float ReduceSpeedBy = 2f;

    }//CrouchProps
}//namespace