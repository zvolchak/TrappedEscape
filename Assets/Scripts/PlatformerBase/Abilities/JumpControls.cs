using System.Collections.Generic;
using UnityEngine;

namespace GHAbilities {
    /// <summary>
    ///  Attach this component to a parent or children of the object that will be jumping.
    /// 
    /// To trigger a Jump routine, you need to either call a "TriggerJump" function, that
    /// will check an Input and call a Jump() based of that - or you can call Jump() directly.
    /// 
    /// </summary>
    public class JumpControls : AAbility {

        public JumpProperties Props;

        public List<Sfx> JumpSfx;
        [Tooltip("Use this if want to add SFX from external object.")]
        public GameObject GoWithSfx;

        private bool bIsJumpPressed;
        private bool bIsCanJump = true;
        private float timeOfJump = 0; //when was the jump initiated.

        /* ************************************************************** */

        public override void Start() {
            base.Start();
            IsJumpThisFrame = false;
            if (JumpSfx == null)
                JumpSfx = new List<Sfx>();

            if (GoWithSfx != null) {
                JumpSfx.AddRange(GoWithSfx.GetComponents<Sfx>());
            }
        }//Start


        public void FixedUpdate() {
            //isJump was set last frame. Whoever needed it - picked it up last frame.
            //On this new frame - jump is technically done. Therefore - false (unless,
            //Update will indicate it is True this time again).
            IsJumpThisFrame = false;
        }//FixedUpdate


        public override Vector2 Use(Vector2 deltaMovement) {
            //Need to reset jumps from outside this cmp
            if (this.Props.MaxJumps > 0 && CurrentJumps >= this.Props.MaxJumps)
                return deltaMovement;

            IsJumpThisFrame = true;
            IsJumping = true;
            deltaMovement.y = +this.Props.JumpVelocity;
            this.timeOfJump = Time.timeSinceLevelLoad;
            this.CurrentJumps++;

            //this.OnJumpListeners?.Invoke();
            this.EOnUse?.Invoke(this);
            return deltaMovement;
        }//Use


        public override Vector2 Stop(Vector2 deltaMovement) {
            IsJumping = false;
            if (deltaMovement.y > 0)
                deltaMovement.y = 0;
            return deltaMovement;
        }//Stop


        public override void Reset(bool isForceReset = false) {
            this.CurrentJumps = 0;
        }//Reset


        /********************************** GETTERS/SETTERS **********************************/

        public bool IsMinHeightReached => Time.timeSinceLevelLoad - this.timeOfJump >= this.Props.MinHeight;

        public bool IsJumping { get; private set; }

        /// <summary>
        ///  How many jumps has been made before landing (so far).
        /// </summary>
        public int CurrentJumps { get; private set; }

        public void AddJumps(int amount) {
            this.CurrentJumps -= amount;
            if (this.CurrentJumps < 0)
                this.CurrentJumps = 0;
        }//AddJumps

        /// <summary>
        ///  True if Jump was initiated this frame. False no jump yet (or its been done last frame).
        /// </summary>
        public bool IsJumpThisFrame { get; private set; }

        public void SetCanJump(bool state) { this.bIsCanJump = state; }

        private AudioSource _audioSource {
            get {
                if (SoundManager.Instance == null)
                    return null;
                return SoundManager.Instance.SfxAudioSource;
            }
        }//_audioSource
    }//class


    /// <summary>
    ///  Properties used for the JumpControls
    /// </summary>
    [System.Serializable]
    public class JumpProperties {

        [Tooltip("How high will actor jump from current location.")]
        public float JumpHeight = 1f;
        [Tooltip("How fast will actor reach the max height.")]
        public float JumpSpeed = 0.4f;
        [Tooltip("How many jumps before landed. Setting it to 0 will imply unlimited jumps.")]
        public int MaxJumps = 1;
        public float MinHeight = 0.2f;
        public AudioClip JumpSound;

        //ref: https://github.com/SebLague/2DPlatformer-Tutorial/blob/master/Platformer%20E03/Assets/Scripts/Player.cs
        public float Gravity => -(2 * this.JumpHeight) / Mathf.Pow(this.JumpSpeed, 2);
        public float JumpVelocity => (Mathf.Abs(this.Gravity) * this.JumpSpeed);

        public float CurrHoldTime { get; private set; }


        public void UpdateHoldTime(float deltaTime) {
            this.CurrHoldTime += deltaTime;
        }//UpdateHoldTime


        public void ResetHoldTime() {
            this.CurrHoldTime = 0f;
        }//ResetHoldTime

    }//JumpProperties

}//namespace