using System.Collections;
using UnityEngine;
using GHPlatformerControls;

namespace GHTriggers {

    /// <summary>
    ///  
    /// </summary>
    public class MovingPlatform : MonoBehaviour {

        public PlatformWaypoint[] Points;
        //[Tooltip("Wait for N seconds at Start and End points before switching direction.")]
        //public float WaitAtEndCycle = 1f;
        public float LerpIndex = 1f;
        [Tooltip("Distance error/margin to a destination point that can be allowed as 'arrived'.")]
        public float ArrivalError = 0.1f;
        public delegate void RespawnEvent(MovingPlatform thePlatform);
        public event RespawnEvent RespawnListeners;
        [Tooltip("Lerp value to be used based of the curve. Use Linear to have no 'slow down' on approach.")]
        public AnimationCurve LerpCurve;
        [Tooltip("Prevents platform from moving, untill called from outside")]
        public bool DontMove = false;
        public bool IsPauseCycle { get; private set; }

        protected int currIndex;
        protected int direction;
        protected bool bIsCanMove;
        protected bool bIsMoving;
        protected bool bIsTriggered;
        protected float currentLerpTime;
        protected CollisionDetection _cd;
        public Vector3 Velocity;


        public virtual void Start() {
            _cd = GetComponent<CollisionDetection>();
            if(this.Points == null || this.Points.Length == 0) {
                if(this.transform.parent != null)
                    this.Points = this.transform.parent.GetComponentsInChildren<PlatformWaypoint>();
            }
            Respawn();
        }//Start


        public virtual void FixedUpdate() {
            bIsMoving = false;
        }//FixedUpdate


        public virtual void Update() {
            if (DontMove)
                return;

            Vector2 deltaMovement = this.Velocity;
            //MovePassangers();
            MoveTowardsPoint(ref deltaMovement);
            this.Velocity = deltaMovement;
            //MovePassangers();
        }//Update


        public virtual void MovePassangers() {
            if (_cd == null)
                return;
            Vector3 deltaMovement = this.Velocity;
            float direction = 1f;
            deltaMovement.y = direction * 0.3f;

            _cd.UpdateRayOrigin();
            _cd.HorizontalCollisions(ref deltaMovement);
            _cd.VerticalCollisions(ref deltaMovement);

            for (int i = 0; i < _cd.VerticalRayMeta.Length; i++) {
                RaycastMeta meta = _cd.VerticalRayMeta[i];
                if(!meta.Ray)
                    continue;

                float moveX = this.Velocity.x;
                float moveY = this.Velocity.y;
                if (Mathf.Sign(this.Velocity.y) > 0)
                    moveY = Velocity.y - (meta.Ray.distance - 0.055f) * direction;

                //var mvmnt = meta.Ray.collider.GetComponent<MovementControls>();
                //if (mvmnt == null)
                //    continue;

                //var player = meta.Ray.collider.GetComponent<Player>();
                //player.IsOnMovingPlatform = true;

                //meta.Ray.collider.transform.Translate(new Vector3(moveX, moveY));
                //mvmnt.SetVelocity(new Vector2(moveX, moveY));
            }//for

        }//MovePassangers


        public virtual bool MoveTowardsPoint(ref Vector2 deltaMovement) {
            //it keep moving even when delta time is 0.
            //Thus - just exit if deltaTime is 0 - problem solved.
            if (Time.deltaTime == 0)
                return false;
            if (!bIsCanMove) {
                this.Velocity = Vector3.zero;
                return false;
            }

            bIsTriggered = true;

            float WaitAtEndCycle = ActivePoint.Props.StayTime;
            if (this.currIndex >= Points.Length - 1 && IsArrived) {
                this.currentLerpTime = 0f;
                StartCoroutine(OnChangeDirection(WaitAtEndCycle));
                return true;
            } else if (this.currIndex <= 0 && IsArrived) {
                this.currentLerpTime = 0f;
                StartCoroutine(OnChangeDirection(WaitAtEndCycle));
                return true;
            } else {
                if (IsArrived) {
                    deltaMovement = Vector3.zero;
                    this.currIndex = this.currIndex + direction;
                }
            }

            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > LerpIndex) {
                currentLerpTime = LerpIndex;
            }


            float lerpSpeed = LerpIndex;
            if (LerpCurve.length > 1) {
                lerpSpeed = LerpCurve.Evaluate(this.currentLerpTime / LerpIndex);

                deltaMovement = (ActivePoint.transform.position - this.transform.position) * lerpSpeed;
                this.transform.position = Vector3.Lerp(
                                    this.transform.position,
                                    ActivePoint.transform.position,
                                    lerpSpeed);
            } else {
                this.transform.position = Vector3.MoveTowards(this.transform.position,
                    ActivePoint.transform.position,
                    lerpSpeed * Time.deltaTime);
            }

            if (IsArrived)
                deltaMovement = Vector3.zero;

            bIsMoving = true;
            return false;
        }//MoveTowardsPoint


        public void OnDrawGizmos() {
            if (Points == null || Points.Length < 2)
                return;

            for (int i = 1; i < Points.Length; i++) {
                Gizmos.DrawLine(Points[i - 1].transform.position, Points[i].transform.position);
            }//for
        }//OnDrawGizmos


        public virtual IEnumerator OnChangeDirection(float waitTime) {
            bIsCanMove = false;
            this.Velocity = Vector3.zero;
            if (waitTime > 0)
                yield return new WaitForSecondsRealtime(waitTime);

            if (bIsTriggered)
                ChangeDirection();

            yield return null;
        }//ChangeDirection


        public virtual void ChangeDirection() {
            if (IsPauseCycle)
                return;
            currentLerpTime = 0;
            bIsCanMove = true;
            direction *= -1;
            this.currIndex = this.currIndex + direction;
        }//ChangeDirection


        public virtual void Respawn() {
            this.currIndex = 0;
            this.direction = 1;
            bIsCanMove = false;
            bIsMoving = false;
            bIsTriggered = false;
            currentLerpTime = 0f;
            Invoke("SetCanMoveTrue", 1f);

            if (RespawnListeners != null)
                RespawnListeners(this);

            if (Points.Length == 0) {
                bIsCanMove = false;
                return;
            }

            this.transform.position = Points[0].transform.position;
            this.currIndex = 1;
        }//Respawn


        public void TriggerSubscribers() {
            if (RespawnListeners != null)
                RespawnListeners(this);
        }


        public bool CheckAtDestination(int pointIndex) {
            float distDiff_b = (this.transform.localPosition - Points[pointIndex].transform.localPosition).sqrMagnitude;
            return distDiff_b < ArrivalError * ArrivalError;
        }//CheckAtDestination


        public virtual void ActivateEvent() {
            IsPauseCycle = false;
            ChangeDirection();
        }

        public virtual void DeactivateEvent() {
            IsPauseCycle = true;
        }

        /* *************************************** GETTERS / SETTERS *************************************** */

        public void SetCanMove(bool state) { this.bIsCanMove = state; }
        public void SetCanMoveTrue() { this.SetCanMove(true); }
        public void SetDontMove(bool state) { this.DontMove = state; }

        public bool IsArrived {
            get {
                Vector3 pointPos = ActivePoint.transform.position;
                float distanceDiff = (this.transform.position - pointPos).sqrMagnitude;
                return distanceDiff < ArrivalError * ArrivalError;
            }//get
        }//IsArrived

        public float DistanceToDestination {
            get {
                Vector3 pointPos = ActivePoint.transform.position;
                return Vector3.Distance(this.transform.position, pointPos);
            }
        }

        /// <summary>
        ///  Index of the ActivePoint towards which platform will move towards.
        /// </summary>
        public int ActiveIndex { get { return this.currIndex; } }

        /// <summary>
        ///  Point towards which this platform is moving to now.
        /// </summary>
        public PlatformWaypoint ActivePoint { get { return Points[this.currIndex]; } }

        /// <summary>
        ///  True if this platform has moved from initial/spawned position.
        /// </summary>
        public bool IsTriggered { get { return bIsTriggered; } }
        public bool IsMoving { get { return bIsMoving; } }
        public int Direction { get { return this.direction; } }
        public bool IsCanMove { get { return this.bIsCanMove; } }
        public bool IsAtStart { get { return CheckAtDestination(0); } }
        public bool IsAtDestination { get { return CheckAtDestination(Points.Length - 1); } }


    }// class MovingPlatform
}//namespace