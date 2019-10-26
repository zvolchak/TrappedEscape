using UnityEngine;
using GHPlatformerControls;

namespace GHAbilities {
    ///<summery>
    ///</summery>
    [RequireComponent(typeof(JumpControls), typeof(Player), typeof(MovementControls))]
    public class Bunnyhopping : MonoBehaviour {

        public float MaxAddedSpeed = 4f;
        public float AddPerHop = 0.4f;
        public float HopInterval = 0.1f;

        private JumpControls _jumpCtrl;
        private Player _player;
        private MovementControls _mvmnt;
        private float timeOfLand;
        private float velocityAtJump = float.MinValue;


        public void Start() {
            _jumpCtrl = GetComponent<JumpControls>();
            _player = GetComponent<Player>();
            _mvmnt = GetComponent<MovementControls>();

            _jumpCtrl.EOnUse -= OnJumpEvent;
            _jumpCtrl.EOnUse += OnJumpEvent;

            _player.OnLandedListeners -= OnLandedEvent;
            _player.OnLandedListeners += OnLandedEvent;
        }//Start


        public void OnJumpEvent(AAbility cmp) {
            float timePassed = Time.timeSinceLevelLoad - this.timeOfLand;
            float velDiff = Mathf.Abs(_mvmnt.Velocity.x) - Mathf.Abs(_mvmnt.RunSpeed);
            if (velDiff >= MaxAddedSpeed)
                return;

            if (timePassed <= HopInterval) {
                if (velocityAtJump == float.MinValue)
                    velocityAtJump = _mvmnt.Velocity.x;

                float dir = Mathf.Sign(this.transform.localScale.x);
                _mvmnt.SetVelocityX(velocityAtJump + (AddPerHop * dir));
                this.velocityAtJump = _mvmnt.Velocity.x;
            } else {
                velocityAtJump = float.MinValue;
            }
        }


        public void OnEnable() {
            
        }//OnEnable


        public void OnDisable() {
            if (_player != null)
                _player.OnLandedListeners -= OnLandedEvent;
            if (_jumpCtrl != null)
                _jumpCtrl.EOnUse -= OnJumpEvent;
        }//OnDisable


        public void OnLandedEvent() {
            this.timeOfLand = Time.timeSinceLevelLoad;
        }//OnLandedEvent


    }//Bunnyhopping
}//namespace