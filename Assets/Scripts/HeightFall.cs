using GHPlatformerControls;
using GHTriggers;
using UnityEngine;

namespace GHActor {
    /// <summary>
    ///  Attach to the object with the AActor component in it.
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class HeightFall : MonoBehaviour {

        public float LethalHeight = 2f;
        [Tooltip("Percentage of the LethalHeight at which to apply stagger effect.")]
        [Range(0f, 1f)] public float StaggerHeight = 0.5f;

#if UNITY_EDITOR
        public bool DrawHeightGizmos = false;
#endif
        public StaggerEffect StaggerEffector;

        private Player _actor;
        private LadderClimber _ladderClimber;
        private ActorStatusController _statusEffectCtrl;
        private bool bIsTrackHeight = false;
        private float highestPoint = float.NegativeInfinity;


        public void Start() {
            _actor = GetComponent<Player>();
            _ladderClimber = GetComponent<LadderClimber>();
            _statusEffectCtrl = GetComponentInChildren<ActorStatusController>();
            if(_statusEffectCtrl == null)
                _statusEffectCtrl = GetComponent<ActorStatusController>();

            _actor.EOnGroundStateChange += GroundStateChanged;
            if (_ladderClimber != null) {
                _ladderClimber.EOnLadderUnset += JumpedOffLadder;
                _ladderClimber.EOnLadderUsed += OnLadderUsed;
            }

        }//Start


        public void Update() {
            if (Input.GetKeyDown(KeyCode.B)) {
                _statusEffectCtrl.ApplyStatusEffect(this.StaggerEffector);
            }

            if (this.bIsTrackHeight) {
                this.trackActorHeight();
            }//if track height
        }//Update


        public void LandedFromHeight() {
            float distance = this.highestPoint - this._actor.transform.position.y;
            if (distance >= LethalHeight)
                _actor.DamagableCmp.TakeDamage(this.gameObject, int.MaxValue, false);
            else if (distance >= (LethalHeight * StaggerHeight))
                _statusEffectCtrl.ApplyStatusEffect(this.StaggerEffector);

            Reset();
        }//LandedFromHeight


        private void trackActorHeight() {
            if (_actor == null) {
#if UNITY_EDITOR
                Debug.LogWarning(this.name + " HeightFall's Actor is null to track height!");
#endif
                return;
            }

            if (this.highestPoint == float.NegativeInfinity)
                this.highestPoint = _actor.transform.position.y;
            

            if(this.highestPoint < _actor.transform.position.y)
                this.highestPoint = _actor.transform.position.y;
        }//trackActorHeight


        public void GroundStateChanged(bool prevState, bool newState) {
            if (newState) {
                LandedFromHeight();
                return;
            }
            if(_ladderClimber != null && _ladderClimber.IsOnLadder)
                return;

            this.bIsTrackHeight = true;
        }//GroundStateChanged


        public void JumpedOffLadder(Ladder l) {
            this.bIsTrackHeight = true;         
        }//JumpedOffLadder


        public void OnLadderUsed(Ladder l) {
            Reset();
        }//OnLadderUsed


        public void Reset() {
            this.bIsTrackHeight = false;
            this.highestPoint = float.NegativeInfinity;
        }//Reset


        public void OnDrawGizmos() {
            if(!DrawHeightGizmos || this.LethalHeight < 0)
                return;

            Vector3 dest = this.transform.position;
            dest.y -= this.LethalHeight;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, dest);
        }//OnDrawGizmos

    }//class
}//namespace
