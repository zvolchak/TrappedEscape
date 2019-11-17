using UnityEngine;

namespace GHAbilities {
    public class WallGrab : MonoBehaviour {

        public WallGrabProps Props;
        public CollisionDetection PlayerCollider;
        public CollisionDetection WallChecker;

        public bool IsOnWall { get; private set; }
        public bool IsWallJumping { get; private set; }
        public bool IsBackflipJumping { get; private set; }
        /// <summary>
        ///  OnWall state on Previous Frame. This get re-stated every
        /// time OnWall is called.
        /// </summary>
        public bool PrevState { get; private set; }

        private float timeOfWallGrab = -1f;
        private float timeOfGrabCooldown = -1f;

        /// <summary>
        ///  Called once on the frame character grabs the wall.
        /// </summary>
        public AbilityDelegate.OnWallAction EOnWallGrab;
        public AbilityDelegate.OnWallAction EOnWallJump;


        public void FixedUpdate() {
        }//FixedUpdate


        public Vector2 OnWall(Vector2 deltaMovement) {
            this.PrevState = IsOnWall;
            if (this.IsGrabTimeout) {
                this.IsOnWall = false;
                return deltaMovement;
            }

            if (this.WallChecker == null || this.PlayerCollider == null) {
#if UNITY_EDITOR
                string msg = "'WallChecker' or 'PlayerCollider' not set in {0}!";
                string goName = PlayerCollider.transform.name;
                Debug.LogWarning(string.Format(msg, goName));
#endif
                return deltaMovement;
            }

            bool isTimeout = false;
            if (this.timeOfWallGrab != -1 && this.Props.MaxTimeOnWall >= 0) {
                isTimeout = Mathf.Abs(Time.timeSinceLevelLoad - this.timeOfWallGrab) >= this.Props.MaxTimeOnWall;
            }

            if (PlayerCollider.Below || isTimeout) {
                Reset();
                if (isTimeout)
                    this.timeOfGrabCooldown = Time.timeSinceLevelLoad;
                else
                    this.timeOfGrabCooldown = -1f;
                return deltaMovement;
            }

            float direction = Mathf.Sign(this.CastWallcheckRays(deltaMovement).x);
            IsOnWall = false;

            IsOnWall = (WallChecker.Left || WallChecker.Right);
            if (WallChecker.Below) //Ground is too close - no need to stick to the wall.
                IsOnWall = false;

            //Check all the rays has a hit.
            if (IsOnWall) {
                for (int i = 0; i < WallChecker.HorizontalRayMeta.Length; i++) {
                    if (!WallChecker.HorizontalRayMeta[i].Ray) {
                        IsOnWall = false;
                        break;
                    }
                    if (WallChecker.HorizontalRayMeta[i].Ray.collider.tag == "NotSticky") {
                        IsOnWall = false;
                        break;
                    }
                }
            }//IsOnWall

            if (IsOnWall) {
                if (Input.GetButtonDown("Vertical"))
                    IsOnWall = false;
            }

            if (this.PrevState != this.IsOnWall) {
                EOnWallGrab?.Invoke(this);
            }

            if (!IsOnWall) { //Not on the wall - dont do anything to the delta
                this.timeOfWallGrab = -1f;
                return deltaMovement;
            }

            deltaMovement.y = 0;

            IsWallJumping = false;
            IsBackflipJumping = false;

            if (this.timeOfWallGrab == -1) {
                this.timeOfWallGrab = Time.timeSinceLevelLoad;
                this.timeOfGrabCooldown = -1f;
            }

            //An actuall Wall Jump
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) {
                Vector2 force = this.Props.PushForce;
                IsWallJumping = true;
                if (Input.GetButtonDown("Fire1")) {
                    IsBackflipJumping = true;
                    force = this.Props.BackflipForce;
                } else {
                    //Switch direction when wall jumping. Feels better that way.
                    //FIXME: no direct ref to player!
                    throw new System.NotImplementedException("HERE! Fixe Calling for Player instance!");
                    //Player.Instance.DirectionSwitcherCmp.OnSwitchDirection(); 
                }

                EOnWallJump?.Invoke(this);
                //return new Vector2(-direction * Props.PushForce.x, Props.PushForce.y);
                return new Vector2(-direction * force.x, force.y);
            }
            return deltaMovement;
        }//OnWall


        /// <summary>
        ///  Cast rays in the deltaMovement direction to check for the collisoin.
        /// Use "WallChecker" to get the raycast meta.
        /// Ray length will be at least the size of PlayerCollider collider + a skin.
        /// This is long enough to detect a wall collision.
        /// </summary>
        /// <param name="deltaMovement"></param>
        /// <returns>Casted vector direction length.</returns>
        public Vector2 CastWallcheckRays(Vector2 deltaMovement) {
            Vector3 rayLength = deltaMovement;
            rayLength = GetMinRayLength();
            rayLength.x *= Mathf.Sign(PlayerCollider.transform.localScale.x);
            if (this.Props.MinHeightToStick >= 0)
                rayLength.y = -(rayLength.y + this.Props.MinHeightToStick);

            float direction = Mathf.Sign(rayLength.x);
            WallChecker.Move(ref rayLength);

            return rayLength;
        }//CastWallcheckRays


        /// <summary>
        ///  Calculates the difference between the player's collider and the wallchecker one to
        /// identify the minimum length for the ray that would cover that difference.
        /// </summary>
        public Vector2 GetMinRayLength() {
            BoxCollider2D playerCollider = PlayerCollider.ColliderCmp;
            BoxCollider2D wallCollider = WallChecker.ColliderCmp;
            float xDiff = playerCollider.bounds.size.x - wallCollider.bounds.size.x;
            float yDiff = playerCollider.bounds.size.y - wallCollider.bounds.size.y;
            if (xDiff < 0)
                xDiff = 0;
            if (yDiff < 0)
                yDiff = 0;
            //half size is needed, since we are looking at One of the sides of the box.
            return new Vector2(xDiff / 2, yDiff / 2);
        }//GetMinRayLength


        public void Reset() {
            IsOnWall = false;
            IsWallJumping = false;
            IsBackflipJumping = false;
            this.timeOfWallGrab = -1f;
            this.timeOfGrabCooldown = -1f;
        }//Reset


        public bool IsNewState => this.PrevState != this.IsOnWall;

        public bool IsGrabTimeout {
            get {
                if (this.Props.WallGrabCooldown < 0)
                    return false;
                if (this.timeOfGrabCooldown == -1)
                    return false;

                return Mathf.Abs(Time.timeSinceLevelLoad - this.timeOfGrabCooldown) < this.Props.WallGrabCooldown;
            }
        }

    }//WallJump


    [System.Serializable]
    public class WallGrabProps {

        [Tooltip("How long can character hold on to the wall. -1 is infinite.")]
        public float MaxTimeOnWall = -1;
        public float WallGrabCooldown = 2f;
        public Vector2 PushForce = new Vector2(0.05f, 0.1f);
        public Vector2 BackflipForce = new Vector2(0.1f, 0.2f);
        public float SlideDownSpeed = 0.03f;
        public float BackflipSlowdown = 0.05f;
        [Tooltip("Height from the ground to the player at which character can stick to the wall.")]
        public float MinHeightToStick = 4f;

    }//WallJumpProps
}//namespace