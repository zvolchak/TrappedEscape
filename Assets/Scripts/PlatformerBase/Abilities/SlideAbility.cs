using UnityEngine;

namespace GHAbilities {
    [RequireComponent(typeof(Crouch))]
    public class SlideAbility : DashAbility {

        private Crouch _crouch;


        public override void Start() {
            base.Start();
            _crouch = GetComponent<Crouch>();
        }


        public override Vector2 Use(Vector2 deltaMovement) {
            if (Input.GetAxis("Horizontal") != 0) {
                if (Input.GetButtonDown(_crouch.InputName))
                    this.PressButton();
            }
            return base.Use(deltaMovement);
        }//Use


        public override Vector2 Stop(Vector2 deltaMovement) {
            deltaMovement = base.Stop(deltaMovement);
            if (Input.GetAxis(CrouchCmp.InputName) != 0)
                return deltaMovement;

            deltaMovement = CrouchCmp.Stop(deltaMovement);
            return deltaMovement;
        }//Stop


        /// <summary>
        ///  Save original BoxCollider bounds and then cut it in half.
        /// Bounds will be restored back on Stop().
        /// </summary>
        public override void PressButton() {
            base.PressButton();
            CrouchCmp.Use(Vector2.zero);
        }


        public Crouch CrouchCmp {
            get {
                if (_crouch == null)
                    _crouch = GetComponent<Crouch>();
                return _crouch;
            }
        }

    }//SlideAbility
}//namespace