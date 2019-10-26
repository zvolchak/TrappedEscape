using UnityEngine;

namespace GHAbilities {
    /// <summary>
    ///  Call this script from some other component, presumably The Player one.
    /// Caller should use DeltaMovement after calling UseAbility to get the change in
    /// velocity (e.g. current dashed velocity).
    /// </summary>
    public class DashAbility : AAbility {

        public DashProperties Props;
        public Sfx DashEffects;

        public float TimeSinceLastJump  { get; private set; }
        public bool HasStopped          { get; private set; }

        protected bool bIsNeedReset;
        protected bool bIsParticlesPlayed;
        private float startSpeed = -1f;

        public override void Start() {
            base.Start();
            this.Props.CurrDashTime = -1f;
        }//Start


        public void FixedUpdate() {
            HasStopped = false;
        }//FixedUpdate


        public virtual void Update() {
            if (!IsDashing) {
                if (bIsNeedReset)
                    Reset();
                return;
            }//if not dashing

            this.Props.CurrDashTime += Time.deltaTime;
            if (this.Props.CurrDashTime >= Props.DashTime)
                this.Stop(Vector2.zero);
        }//Update


        public override Vector2 Use(Vector2 deltaMovement) {
            if (!IsCanDash)
                return deltaMovement;

            if (Input.GetButtonDown(InputName))
                PressButton();

            if (!IsDashing)
                return deltaMovement;

            float direction = ActorCmp.transform.localScale.x;
            direction = Mathf.Sign(direction);

            IsDashing = true;
            if (this.Props.CurrDashTime == -1) { //not dashing yet
                Player.Instance.GetComponent<CharacterAnimator>().AnimatorCmp.SetTrigger(this.AnimationState);
                this.Props.CurrDashTime = 0f;
                this.startSpeed = deltaMovement.x;
            }

            deltaMovement.x = startSpeed + this.Props.Force * direction;
            deltaMovement.y = 0f;

            PlaySoundEffect();
            if (!bIsParticlesPlayed) {
                if (DashEffects != null) {
                    DashEffects.PlayParticles(this.transform.position, -ActorCmp.transform.localScale.x);
                    bIsParticlesPlayed = true;
                }
            }//if particles

            bIsNeedReset = true;

            this.EOnUse?.Invoke(this);
            return deltaMovement;
        }//UseAbility


        public override Vector2 Stop(Vector2 deltaMovement) {
            this.TimeSinceLastJump = Time.timeSinceLevelLoad;

            this.Props.CurrDashTime = -1f;

            IsDashing = false;
            bIsNeedReset = false;
            bIsParticlesPlayed = false;
            ReleaseButton();
            HasStopped = true;
            return deltaMovement;
        }//Stop


        public void PlaySoundEffect() {
            if (DashEffects == null)
                return;

            DashEffects.PlaySound();
        }//PlaySoundEffect


        public override void PressButton() {
            base.PressButton();
            this.IsDashing = true;
        }


        public override void Reset(bool isForceReset = false) {
            if (!bIsNeedReset && !isForceReset)
                return;
            this.Stop(Vector2.zero);
        }//Reset

        public bool IsDashing { get; private set; }
        public bool IsCanDash => !this.IsCooldown;
        public bool IsCooldown => (Time.timeSinceLevelLoad - this.TimeSinceLastJump) < this.Props.Timeout;
    }//class AADash


    [System.Serializable]
    public class DashProperties {
        public float Force => this.ForceFunction.Evaluate(this.CurrDashTime);
        public float DashTime => this.ForceFunction[this.ForceFunction.keys.Length - 1].time;
        [Tooltip("Timeout after a dash is complete after which can dash again.")]
        public float Timeout = 0.1f;
        public AnimationCurve ForceFunction;

        public float CurrDashTime { get; set; }

    }//DashProperties
}//namespace