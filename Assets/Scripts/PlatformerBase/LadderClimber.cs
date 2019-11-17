using UnityEngine;
using GHTriggers;
using GHPhysics;
using GHPlatformerControls;
using GHGameManager;

namespace GHPlatformerControls {
    public class LadderClimber : MonoBehaviour {

        public float Speed = 1f;
        public string InputName = "Vertical";
        public AActor2D Actor;
        [Tooltip("Snap character position to the ladder X axes on use.")]
        public bool IsSnapToLadder = true;
        [Tooltip("Allow to move horizontally on the ladder.")]
        public bool IsCanMoveOnLadder = false;
        public float GroundDistanceCheck = 0f;

        public bool IsClimbing { get; private set; }
        /// <summary>
        ///  A Ladder cmp with which this actor can interuct with.
        /// </summary>
        public Ladder ActiveLadder { get; private set; }

        private string[] raycastPlatformTags;
        private Vector3 posOfStartClimb;

        //FIXME: dont use input in this CMP!
        private InputManager _input;

        /* ---------------------- EVENTS ---------------------- */

        public delegate void OnLadderUsed(Ladder l);

        /// <summary>
        ///  Called when character starts using ladder.
        /// Signature: <Ladder>
        /// </summary>
        public OnLadderUsed EOnLadderUsed;

        /// <summary>
        ///  Called when character stops using ladder.
        /// Signature: <Ladder>
        /// </summary>
        public OnLadderUsed EOnLadderUnset;

     /* ---------------------- ---------------------- ---------------------- */


        public void Start() {
            if (this.Actor == null)
                this.Actor = GetComponent<AActor2D>();
            _input = this.Actor.GetComponent<InputManager>();
        }//Start


        public void Update() {
            OnClimb();
        }//Update


        public virtual void SetLadder(Ladder newLadder) {
            if (this.Actor == null)
                return;
            if(newLadder == null)
                return;

            this.ActiveLadder = newLadder;
        }//OnTriggerEnter2D


        /// <summary>
        ///  Stop using ladder by setting ActiveLadder to null and reseting
        /// Actor's gravity.
        /// </summary>
        /// <param name="caller"></param>
        public virtual void StopClimbing(Ladder caller) {
            if (caller != null && caller != this.ActiveLadder)
                return;

            if (IsClimbing) {
                if (this.Actor.Gravity != null)
                    this.Actor.Gravity.ResetGravity();
            }

            IsClimbing = false;
            this.enableRaycastIgnoreTags(false);

            this.EOnLadderUnset?.Invoke(this.ActiveLadder);
            
            this.ActiveLadder = null;
        }//OnTriggerExit2D


        /// <summary>
        ///  Called every Update cycle to "move" actor through the ladder.
        /// </summary>
        /// <param name="axisInput"></param>
        /// <param name="isPressed"></param>
        public virtual void OnClimb(float axisInput=float.NegativeInfinity, bool isPressed=false) {
            if (this.Actor == null)
                return;
            if (this.ActiveLadder == null)
                return;

            Gravity gravity = this.Actor.Gravity;
            CollisionDetection cd = this.Actor.CollisionDetector;

            //FIXME: Move Input from global to objects input listener
            float vertInput = 0;
            if (axisInput == float.NegativeInfinity)
                vertInput = _input.GetVerticalAxis();
            else
                vertInput = axisInput;

            vertInput = vertInput != 0 ? Mathf.Sign(vertInput) * 1 : 0;
            if (axisInput == float.NegativeInfinity)
                isPressed = _input.GetVerticalAxis() != 0;
            
            //FIXME: !!! Dont snap to Ladder when Down pressed on Solid ground. !!!

            //This will(should) be called Once per Ladder usage when dropped
            //off the ladder and jumped on it again.
            if (!IsClimbing && isPressed) {
                if (vertInput == 0)
                    return;
                this.enableRaycastIgnoreTags(true);
                this.SnapToLadder(this.ActiveLadder);
                IsClimbing = true;
                this.posOfStartClimb = this.Actor.transform.position;

                this.EOnLadderUsed?.Invoke(this.ActiveLadder);
            }//if not contains

            if (!IsClimbing)
                return;

            //if (cd != null) {
            //    Debug.Log(this.Actor.IsGrounded);
            //    RaycastHit2D ray = cd.VerticalRayMeta[cd.VerticalRayMeta.Length / 2].Ray;
            //    //Vector3 deltaMovement = Vector3.up * axisInput;
            //    //if (!ray) {
            //    //    ray = cd.CastVerticalRay(deltaMovement.y, cd.VerticalRayMeta.Length / 2).Ray;
            //    //}

            //    if (vertInput < 0) {
            //        if (ray) this.enableRaycastIgnoreTags(true);
            //        else this.enableRaycastIgnoreTags(false);
            //    } else {
            //        this.enableRaycastIgnoreTags(false);
            //    }//if
            //}//if cd

            if (gravity != null && vertInput != 0)
                gravity.SetGravity(0);

            OnLadderMvmntController(vertInput);
        }//OnTriggerStay2D


        public bool OnLadderMvmntController(float inputAxisValue) {
            if(this.Actor == null)
                return false;

            MovementControls mvmnt = this.Actor.MvmntCmp;
            if (this.Actor.MvmntCmp == null) {
                #if UNITY_EDITOR
                    Debug.LogWarning(this.Actor.name + " has no MovementControls to use the ladder!");
                #endif
                return false;
            }

            //Check ground only when going Down..
            if (!IsCanMoveOnLadder && inputAxisValue <= 0) {
                float direction = Mathf.Abs(inputAxisValue) > 0 ? 1 : 0;
                if (mvmnt.Velocity.x != 0)
                    direction = 1;

                bool isGrounded = GetActorGrounded(-GroundDistanceCheck * direction);
                if (!isGrounded)
                    mvmnt.SetVelocityX(0);
                else {
                    //this.StopClimbing(this.ActiveLadder);
                }
            }

            if (this.Actor.IsGrounded) {
                float climbedDistance = Vector3.Distance(this.posOfStartClimb, this.Actor.transform.position);
                if (climbedDistance >= 1f) {
                    StopClimbing(this.ActiveLadder);
                }
            }

            mvmnt.SetVelocityY(inputAxisValue * Speed);

            return true;
        }//OnLadderMvmntController


        /// <summary>
        ///  Move/Snap Actor to the ladder's X position. Also, if the Actor has
        /// movement component, set its X velovity to 0.
        /// </summary>
        /// <param name="targetLadder"></param>
        public void SnapToLadder(Ladder targetLadder) {
            if (this.Actor == null)
                return;

            Vector3 targetPos = this.Actor.transform.position;
            Vector3 ladderPos = this.ActiveLadder.transform.position;
            //Snap target to the X position of the ladder when climbing
            this.Actor.transform.position = new Vector3(ladderPos.x,
                                                        targetPos.y,
                                                        targetPos.z);
            if (this.Actor.MvmntCmp == null)
                return;

            this.Actor.MvmntCmp.SetVelocityX(0);
        }//SnapToLadder


        public bool GetActorGrounded(float distance=0) {
            if (this.Actor == null)
                return false;
            if (this.Actor.CollisionDetector == null)
                return false;

            CollisionDetection cd = this.Actor.CollisionDetector;
            bool isGrounded = cd.Below || cd.IsOnSlope;
            //Already grounded? No need to cast rays then...
            if(isGrounded)
                return true;

            if(distance != 0) {
                RaycastMeta rayMeta = cd.CastVerticalRay(distance, cd.Props.VerticalRays / 2);
                return rayMeta.Ray;
            }

            return isGrounded;
        }//CheckGround


        /// <summary>
        ///  Saves CollisionDetection platform tags and adds them to the list
        /// of tags to ignore, so that character could "drop through" the platform
        /// and use the ladder below it. 
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="state">True - add tags to CollisionDetection.CollisionIgnoreTags;
        ///                     False - remove tags from that list.
        /// </param>
        protected void enableRaycastIgnoreTags(bool state) {
            if(this.Actor == null)
                return;
            if(this.Actor.CollisionDetector == null)
                return;
            if(state && this.IsRaycastTagsIgnored)
                return;
            if (!state && !this.IsRaycastTagsIgnored)
                return;

            CollisionDetection cd = this.Actor.CollisionDetector;

            if (this.raycastPlatformTags == null)
                this.raycastPlatformTags = cd.Props.PlatformTags.ToArray();

            if (this.raycastPlatformTags == null || this.raycastPlatformTags.Length == 0)
                return;

            for (int i = 0; i < this.raycastPlatformTags.Length; i++) {
                string tagName = this.raycastPlatformTags[i];
                if (state) {
                    if (!cd.Props.CollisionIgnoreTags.Contains(tagName))
                        cd.Props.CollisionIgnoreTags.Add(tagName);
                } else {
                    if (cd.Props.CollisionIgnoreTags.Contains(tagName))
                        cd.Props.CollisionIgnoreTags.Remove(tagName);
                }
            }//for

            if (!state)
                this.raycastPlatformTags = null;
        }//updateRaycastIgnoreTags


        public bool IsOnLadder => this.ActiveLadder != null;

        /// <summary>
        ///  Return True if CollisionDetection IgnoreTags has been altered;
        ///  False - CollisionDetection IgnoreTags are back to origin.
        /// </summary>
        public bool IsRaycastTagsIgnored {
            get {
                return this.raycastPlatformTags != null;
            }//get
        }//IsRaycastTagsIgnored
    }//class
}//namespace