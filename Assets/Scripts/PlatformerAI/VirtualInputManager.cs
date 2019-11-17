using GHGameManager;

namespace GHAI {
    public class VirtualInputManager : InputManager {


        public override float GetHorizontalAxis() {
            return this.horizontalAxis;
        }//GetHorizontalAxis


        public override float GetVerticalAxis() {
            return this.verticalAxis;
        }//GetVerticalAxis


    }//class
}//namespace
