using UnityEngine;

namespace GHGameManager {
    public class InputManager : MonoBehaviour {

        private float horizontalAxis = float.NegativeInfinity;


        public float GetHorizontalAxis() {
            return Input.GetAxisRaw("Horizontal");
        }//GetHo


        public void SetHorizontalAxis(float val) {
            val = val > 1 ? 1 : val;
            val = val < -1 ? -1 : val;
            this.horizontalAxis = val;
        }//SetHorizontalAxis


    }//InputManager
}//namespace