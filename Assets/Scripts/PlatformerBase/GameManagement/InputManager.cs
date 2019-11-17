using System.Collections.Generic;
using UnityEngine;

namespace GHGameManager {
    public class InputManager : MonoBehaviour {

        public float HorizontalReverse = 1f;
        public float VerticalReverse = 1f;

        protected float horizontalAxis = 0;
        protected float verticalAxis = 0;

        //private Dictionary<string, bool> pressedThisFrame = new Dictionary<string, bool>();


        public void FixedUpdate() {
            //this.pressedThisFrame = new Dictionary<string, bool>();
        }//FixedUpdate


        /// <summary>
        ///  Return Input.GetAxisRaw("Horizontal") by default. Override it to return
        /// something else (e.g. this.horizontalAxis);
        /// </summary>
        public virtual float GetHorizontalAxis() {
            return Mathf.Sign(HorizontalReverse) * Input.GetAxisRaw("Horizontal");
        }//GetHorizontalAxis


        /// <summary>
        ///  Return Input.GetAxisRaw("Vertical") by default. Override it to return
        /// something else (e.g. this.verticalAxis);
        /// </summary>
        public virtual float GetVerticalAxis() {
            return Mathf.Sign(VerticalReverse) * Input.GetAxisRaw("Vertical");
        }//GetVerticalAxis


        public void SetHorizontalAxis(float val) {
            //string inputName = "Horizontal";
            //this.pressedThisFrame[inputName] = true;

            this.horizontalAxis = Mathf.Clamp(val, -1, 1);
        }//SetHorizontalAxis


        public void SetVerticalAxis(float val) {
            //string inputName = "Vertical";
            //this.pressedThisFrame[inputName] = true;

            this.verticalAxis = Mathf.Clamp(val, -1, 1);
        }//SetVerticalAxis


        public bool IsPressed(string inputName) {
            return false;
        }

    }//InputManager
}//namespace