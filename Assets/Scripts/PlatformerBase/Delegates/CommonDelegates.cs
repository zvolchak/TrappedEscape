using UnityEngine;

namespace GHDelegates {
    public class CommonDelegates {
        public delegate void SimpleDelegate();

        public delegate void ValueDelegate(float val);
        public delegate void ValueDelegate2(float val1, float val2);

        public delegate void StateChange(bool prevState, bool newState);

        public delegate void VectorStateChange(Vector3 prevState, Vector3 newState);
    }//class
}//namespace