using System.Collections.Generic;
using UnityEngine;

namespace GHTriggers {
    ///<summery>
    ///</summery>
    public class LeverColliderReact : LeverReactor {

        public List<SpriteRenderer> SpritesToToggle;

        private Collider2D _colldier;


        public void Start() {
            _colldier = GetComponent<Collider2D>();
            if (DefaultState)
                Activate();
            else
                Deactivate();
        }//Start


        public override void Activate() {
            _colldier.enabled = true;
            this.toggleSprites(true);
            this.SwitchState = true;
        }//Activate


        public override void Deactivate() {
            _colldier.enabled = false;
            this.toggleSprites(false);
            this.SwitchState = false;
        }//Deactivate



        private void toggleSprites(bool state) {
            int size = this.SpritesToToggle.Count;
            for (int i = 0; i < size; i++) {
                this.SpritesToToggle[i].enabled = state;
            }//for
        }//toggleSprites


    }//LeverColliderReact
}//namespace