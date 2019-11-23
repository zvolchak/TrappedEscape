using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GHTriggers {
    ///<summery>
    /// Something that activataes a LeverReactor object.
    ///</summery>
    [RequireComponent(typeof(Collider2D))]
    public class Lever : TriggerListener {

        public LayerMask WhoCanActivateByLayer;
        [Tooltip("List of tags names, separated by space, who can activate this lever.")]
        public string ActivateByTag;
        [Tooltip("Something that is affected by this Lever.")]
        public List<LeverReactor> Reactors = new List<LeverReactor>();
        [Tooltip("Time before Toggling is allowed on next state switch.")]
        public float ToggleTimeout = 0.2f;
        [Tooltip("Toggle the lever back to the original state if this value >= 0.")]
        public float SwitchBackTimeout = -1f;
        public Color DebugColor = new Color(0, 1, 0, 0.2f);
        [Tooltip("Set animator object by hand. If not set - will try to get one from this gameobject.")]
        public Animator AnimatorCmp;
        [Tooltip("Set to false if want to toggle on collider Exit instead of Enter event.")]
        public bool IsToggleOnEnter = true;
        /// <summary>
        ///  When InputName is used, IsToggleOnEnter will be ignored and instead,
        ///  will check for Input button down to toggle the lever.
        /// </summary>
        public string InputName;
        public bool DefaultState = true;
        public bool IsDrawGizmos = true;

        public float timeOfToggle = -1;
        public bool CurrentState { get; protected set; }

        public delegate void OnLeverTriggered(Lever whatWasTriggered);
        /// <summary>
        ///  Signature: void OnLeverTriggered(Lever whatWasTriggered)
        /// </summary>
        public OnLeverTriggered EOnLeverTriggered;


        public override void Start() {
            base.Start();
            if (AnimatorCmp == null)
                AnimatorCmp = GetComponent<Animator>();
            if (Reactors.Count > 0) {
                foreach (LeverReactor r in Reactors) {
                    r.SetLever(this);
                }//foreach
            }
            this.CurrentState = this.DefaultState;
            this.ToggleAnimation(this.CurrentState);
        }//Start


        public void Update() {
            if (IsStaying && !IsToggleOnEnter) {
                if(Input.GetButtonDown(InputName))
                    Toggle();
            }
        }//Update


        /// <summary>
        ///  Activate or Deactivate LeverReactors.
        /// </summary>
        /// <param name="state"> True - Activate levers. False - deactivate.</param>
        public virtual void Toggle() {
            this.timeOfToggle = Time.timeSinceLevelLoad;
            //if (Reactors == null || Reactors.Count == 0)
            //    return;

            int size = this.Reactors.Count;
            for (int i = 0; i < size; i++) {
                var reactor = Reactors[i];
                reactor.Toggle();
            }//for

            this.CurrentState = !this.CurrentState;
            this.ToggleAnimation(this.CurrentState);

            this.EOnLeverTriggered?.Invoke(this);

            if (this.SwitchBackTimeout >= 0) {
                if (this.CurrentState == this.DefaultState)
                    return;
                CancelInvoke("Toggle");
                Invoke("Toggle", this.SwitchBackTimeout);
            }
        }//Toggle


        public override void OnTriggerEnter2D(Collider2D collision) {
            base.OnTriggerEnter2D(collision);
            if(!HasEntered)
                return;
            if (InputName != "")
                return;
            if (!IsToggleOnEnter)
                return;
            OnTrigger(collision.gameObject);
        }//OnTriggerEnter2D


        public override void OnTriggerExit2D(Collider2D collision) {
            base.OnTriggerExit2D(collision);
            if(!HasExited)
                return;
            if (InputName != "")
                return;
            if (IsToggleOnEnter)
                return;
            OnTrigger(collision.gameObject);
        }//OnTriggerExit2D


        public void OnTrigger(GameObject whoIsTriggering) {
            if (!IsCanTrigger(whoIsTriggering))
                return;
            this.Toggle();
        }//OnTrigger


        public virtual void ToggleAnimation(bool state) {
            if (AnimatorCmp == null)
                return;
            if(state)
                AnimatorCmp.SetTrigger("SwitchOn");
            else
                AnimatorCmp.SetTrigger("SwitchOff");
        }//ToggleAnimation


        /// <summary>
        ///  Check if a GameObject that has entered this collider can trigger this lever.
        /// It is called by OnTriggerEnter2D function.
        /// </summary>
        public virtual bool IsCanTrigger(GameObject theOneToTrigger) {
            if (this.timeOfToggle >= 0 && (Time.timeSinceLevelLoad - this.timeOfToggle) < ToggleTimeout)
                return false;
            if (this.timeOfToggle >= 0 && this.ToggleTimeout < 0) //trigger once - cant trigger again.
                return false;

            if (!IsTargetLayer(theOneToTrigger))
                return false;
            if (ActivateByTag != "" && !IsTargetTag(theOneToTrigger.tag))
                return false;

            return true;
        }//IsCanTrigger


        public bool IsTargetLayer(GameObject go) {
            return GameUtils.Utils.CompareLayers(WhoCanActivateByLayer, go.layer);
        }


        /// <summary>
        ///  Return True if WhoCanActivateByTag is not set or if tagToCheckAgainst is in its list.
        /// </summary>
        public bool IsTargetTag(string tagToCheckAgainst) {
            if (WhoCanActivateByTag == null || WhoCanActivateByTag.Length == 0)
                return true;
            return this.WhoCanActivateByTag.Contains(tagToCheckAgainst);
        }//IsTargetTag


        public void OnDrawGizmos() {
            if(!IsDrawGizmos)
                return;
            if (Reactors == null || Reactors.Count == 0)
                return;

            Gizmos.color = DebugColor; //transparent green
            foreach (LeverReactor r in this.Reactors) {
                Gizmos.DrawLine(this.transform.position, r.transform.position);
            }
        }//OnDrawGizmos


        public override void Reset() {
            base.Reset();
            //this.timeOfToggle = -1;
            //this.CurrentState = this.DefaultState;
            //this.ToggleAnimation(this.CurrentState);
        }//Respawn


        public string[] WhoCanActivateByTag => this.ActivateByTag.Split(' ');

    }//Lever
}//namespace