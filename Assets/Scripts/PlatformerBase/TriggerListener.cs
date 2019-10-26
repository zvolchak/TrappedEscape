using UnityEngine;

namespace GHTriggers {

    public class TriggerListener : MonoBehaviour {

        public string TargetTag;
        public LayerMask TargetLayer;
        public bool HasEntered  { get; private set; }
        public bool HasExited   { get; private set; }
        public bool HasCollided { get; private set; }
        public bool IsStaying   { get; private set; }
        public float StayTime   { get; private set; }

        protected Rigidbody2D _rigidBody;
        protected BoxCollider2D _boxCollider;
        private bool bIsReset;

        public virtual void Start() {
            Reset();
            StayTime = 0f;
        }//Start


        public virtual void LateUpdate() {
            if (bIsReset)
                Reset();
            if (HasEntered && HasExited)
                bIsReset = true;

            HasCollided = false;
        }//LateUpdate


        public virtual void Reset() {
            HasEntered = false;
            HasExited = false;
            bIsReset = false;
        }//Reset


        public virtual void OnTriggerEnter2D(Collider2D collision) {
            bool isValidCollider = ValidateCollider(collision.tag, collision.gameObject.layer);
            if (!isValidCollider)
                return;
            this.StayTime = 0f;
            HasEntered = true;
            HasExited = false;
        }//OnTriggerEnter2D


        public virtual void OnTriggerExit2D(Collider2D collision) {
            bool isValidCollider = ValidateCollider(collision.tag, 
                                                    collision.gameObject.layer);
            if (!isValidCollider)
                return;

            HasExited = true;
            IsStaying = false;
        }//OnTriggerExit2D


        public virtual void OnTriggerStay2D(Collider2D collision) {
            bool isValidCollider = ValidateCollider(collision.tag, 
                                                    collision.gameObject.layer);
            if (!isValidCollider)
                return;
            IsStaying = true;
            this.StayTime += Time.deltaTime;
        }//OnTriggerStay2D


        public virtual void OnCollisionEnter2D(Collision2D collision) {
            bool isValidCollider = ValidateCollider(collision.gameObject.tag, 
                                                    collision.gameObject.layer);
            if (!isValidCollider)
                return;
            HasCollided = true;
        }//OnCollisionEnter2D


        /// <summary>
        ///  Check if other collider matches Target Tag or Target Layer.
        /// If both TargetTag and TargetLayer are set, then check for both to match.
        /// </summary>
        public bool ValidateCollider(string tagName, LayerMask targetLayer) {
            bool isTag = false;
            bool isLayer = GameUtils.Utils.CompareLayers(TargetLayer, targetLayer);
            if (!TargetTag.Equals("")) {
                isTag = tagName.ToLower().Equals(TargetTag.ToLower());
                if (TargetLayer.value != 0) {
                    return isLayer && isTag;
                } else
                    return isTag;
            }//if Tag

            return isLayer;
        }//ValidateCollider


        /* ****************************** GETTERS/SETTERS ****************************** */

        public Rigidbody2D RigidBodyCmp {
            get {
                if (_rigidBody == null)
                    _rigidBody = GetComponent<Rigidbody2D>();
                return _rigidBody;
            }//get
        }//RigidBodyCmp


        public BoxCollider2D BoxColliderCmp {
            get {
                if (_boxCollider == null)
                    _boxCollider = GetComponent<BoxCollider2D>();
                return _boxCollider;
            }//get
        }//BoxColliderCmp

    }//class TriggerListener
}//namespace