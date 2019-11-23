using System.Collections.Generic;
using UnityEngine;

namespace GHTriggers {
    ///<summery>
    ///</summery>
    public abstract class LeverReactor : MonoBehaviour {

        public bool DefaultState;

        public abstract void Activate();
        public abstract void Deactivate();

        /// <summary>
        ///  Looks at SwitchState and runs Deactivate() if it is True; Activate() if False.
        /// </summary>
        /// <returns>SwitchState status after it has been toggled.</returns>
        public virtual bool Toggle() {
            if (this.SwitchState)
                this.Deactivate();
            else
                this.Activate();
            return this.SwitchState;
        }//Toggle


        public void SetLever(Lever l) {
            if (ConnectedLevers == null)
                ConnectedLevers = new List<Lever>();
            if (this.ConnectedLevers.Contains(l))
                return;
            this.ConnectedLevers.Add(l);
        }//SetLever


        public List<Lever> ConnectedLevers { get; protected set; }
        public bool SwitchState { get; protected set; }

    }//LeverReactor
}//namespace