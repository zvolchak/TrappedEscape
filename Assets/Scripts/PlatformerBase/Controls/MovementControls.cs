using GHGameManager;
using UnityEngine;

namespace GHPlatformerControls {

    public class MovementControls : MonoBehaviour {

        public float RunSpeed = 10f;
        public float SlowdownInertial = 0.1f;
        public string InputName = "Horizontal";
        public Animator AnimationController;

        private float origMaxSpeed = float.MinValue;
        private float prevDirection = 0; //set when input direction changed from last input check.
        private Vector2 pushForce = Vector2.zero;
        private float inertiaTime = 0f;
        private float timeOfPush = -1f;
        private float pushbackCooldown = -1f;
        private bool bIsCanInputHorizontal = true;
        private InputManager _inputMngr;

        public delegate Vector2 OnDirectionChanged(Vector2 deltaMovement, float newDir, float prevDir);
        /// <summary>
        /// OnDirectionChanged(Vector2 deltaMovement, float newDir, float prevDir);
        /// </summary>
        public OnDirectionChanged EOnDirectionChanged;
        public float horizontalAxis { get; private set; }


        public virtual void Start() {
            this.MaxRunSpeed = RunSpeed;
            _inputMngr = GetComponent<InputManager>();
        }//Start


        public virtual void FixedUpdate() {
            if (InputName == "")
                return;
            //FIXME: idk wtf this is. Dont use Input here!
            if (bIsCanInputHorizontal) {
                //horizontalAxis = Input.GetAxis(InputName);
                horizontalAxis = _inputMngr.GetHorizontalAxis();
            }
        }//Update


        public void LateUpdate() {
            if (this.pushbackCooldown >= 0f) {
                this.pushbackCooldown -= Time.deltaTime;
            } else {
                this.pushbackCooldown = -1f;
            }
        }//LateUpdate


        public Vector2 Move(Vector2 deltaMovement) {
            //horizontalAxis = Input.GetAxis(InputName);
            if (Mathf.Sign(horizontalAxis) != prevDirection) {
                float prevDir = prevDirection;
                prevDirection = Mathf.Sign(horizontalAxis);
                EOnDirectionChanged?.Invoke(deltaMovement, prevDirection, prevDir);
            }

            if (Mathf.Abs(deltaMovement.x) > 0 && horizontalAxis == 0) { //INERTIA
                deltaMovement = ApplyInertia(deltaMovement);
            } else {
                deltaMovement.x = MaxRunSpeed * Mathf.Sign(horizontalAxis) * Mathf.Abs(horizontalAxis);
            }

            //deltaMovement.x = RunSpeed * Mathf.Sign(horizontalAxis) * Mathf.Abs(horizontalAxis);
            return deltaMovement;
        }//Move


        public Vector2 ApplyInertia(Vector2 deltaMovement) {
            float sign = Mathf.Sign(deltaMovement.x);
            deltaMovement.x += -sign * this.SlowdownInertial;
            if (sign > 0) {
                if (deltaMovement.x < 0)
                    deltaMovement.x = 0;
            } else if (sign < 0) {
                if (deltaMovement.x > 0)
                    deltaMovement.x = 0;
            }
            return deltaMovement;
        }//ApplyInertia


        public void Update() {
            if (AnimationController != null && AnimationController.enabled) {
                AnimationController.SetInteger("Speed", (int)Velocity.x);
            }
        }//Update


        /************************** GETTERS/SETTERS **************************/

        public void SetMaxSpeed(float val) {
            if (this.origMaxSpeed == float.MinValue)
                this.origMaxSpeed = this.MaxRunSpeed;
            this.MaxRunSpeed = val;
            if(this.Velocity.x > val)
                this.SetVelocityX(val);
        }//SetMaxSpeed


        public void ResetMaxSpeed() {
            if (this.origMaxSpeed == float.MinValue)
                return;
            this.MaxRunSpeed = this.origMaxSpeed;
            this.origMaxSpeed = float.MinValue;
        }


        public float MaxRunSpeed { get; private set; }
        public Vector2 Velocity { get; private set; }


        public void SetVelocity(Vector2 newVelocity) {
            if (this.pushForce != Vector2.zero) {
                if (Mathf.Sign(this.Velocity.x) != Mathf.Sign(this.pushForce.x))
                    newVelocity.x = 0f;
                if (Mathf.Sign(this.Velocity.y) != Mathf.Sign(this.pushForce.y))
                    newVelocity.y = 0f;

                this.timeOfPush += Time.deltaTime;
                newVelocity.x = this.pushForce.x;
                newVelocity = Vector2.Lerp(this.Velocity, this.pushForce, this.inertiaTime * Time.fixedDeltaTime);
                if (this.timeOfPush >= 0.3f) {
                    this.pushForce = Vector2.zero;
                    this.timeOfPush = -1f;
                    this.pushbackCooldown = 0.2f;
                    //if(Mathf.Abs(newVelocity.x) > MaxRunSpeed)
                    //    newVelocity.x = MaxRunSpeed * Mathf.Sign(newVelocity.x);
                }
            }

            this.Velocity = newVelocity;
        }//SetVelocity

        public Vector2 SetVelocityY(float y) {
            SetVelocity(new Vector2(this.Velocity.x, y));
            return this.Velocity;
        }

        public Vector2 SetVelocityX(float x) {
            SetVelocity(new Vector2(x, this.Velocity.y));
            return this.Velocity;
        }//SetVelocityX


        public void AddVelocityX(float amount) {
            SetVelocityX(Velocity.x + amount);
        }//AddVelocityX


        public void AddVelocityY(float amount) {
            SetVelocityY(Velocity.y + amount);
        }//AddVelocityX


        public void AddForce(Vector2 xForce, float inertiaTime = 1f) {
            this.pushForce = xForce;
            this.inertiaTime = inertiaTime;
            if (this.timeOfPush < 0) {
                this.timeOfPush = 0f;
            }
        }//PushForceX


        public void ResetPushForce() {
            this.timeOfPush = -1f;
            this.pushForce = Vector2.zero;
            this.pushbackCooldown = -1f;
        }


        public void EnableHorizontalInput(bool state) {
            this.bIsCanInputHorizontal = state;
        }//EnableHorizontalInput


        public bool IsPushbackCooldown => this.pushbackCooldown != -1f;
    }//class
}//namespace