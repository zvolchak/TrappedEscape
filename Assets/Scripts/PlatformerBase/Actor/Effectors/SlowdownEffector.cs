using System.Collections.Generic;
using UnityEngine;
/*
namespace GHActor {

    ///<summery>
    ///</summery>
    [RequireComponent(typeof(BoxCollider2D))]
    public class SlowdownEffector : MonoBehaviour, AStatusEffect {

        public LayerMask TargetLayer;
        [Tooltip("Ration at which speed is affected, e.g. newSpeed=speed * SlowdownFactor.")]
        public float SlowdownFactor = 0.5f;
        private BoxCollider2D _collider;
        private Dictionary<GameObject, ActorStatusController> affectedActors = new Dictionary<GameObject, ActorStatusController>();


        public virtual void OnTriggerEnter2D(Collider2D collision) {
            bool isTargetLayer = GameUtils.Utils.CompareLayers(TargetLayer, collision.gameObject.layer);
            if (this.IsDisabled)
                return;

            if (!isTargetLayer)
                return;

            ActorStatusController statusCtrl = null;
            if (this.affectedActors.ContainsKey(collision.gameObject))
                statusCtrl = this.affectedActors[collision.gameObject];
            else {
                statusCtrl = collision.GetComponent<ActorStatusController>();
                this.affectedActors.Add(collision.gameObject, statusCtrl);
            }

            if (statusCtrl == null)
                return;

            statusCtrl.ApplyStatusEffect(this);
        }//OnTriggerEnter2D


        public virtual void OnTriggerExit2D(Collider2D collision) {
            if (!this.affectedActors.ContainsKey(collision.gameObject))
                return;

            ActorStatusController statusCtrl = this.affectedActors[collision.gameObject];
            if (statusCtrl != null)
                statusCtrl.RemoveStatusEffect(this);
        }//OnTriggerExit2D


        public bool ApplyEffect(AActor2D actor) {
            if (actor == null)
                return true;

            var mvmnt = actor.MvmntCmp;
            if (mvmnt.MaxRunSpeed == mvmnt.RunSpeed * SlowdownFactor)
                return false;

            mvmnt.SetMaxSpeed(mvmnt.RunSpeed * SlowdownFactor);
            return true;
        }//ApplyEffect


        public bool RemoveEffect(AActor2D actor) {
            if (actor == null)
                return true;

            actor.MvmntCmp.ResetMaxSpeed();
            return true;
        }//RemoveEffect

        public float GetMaxDuration() {
            throw new System.NotImplementedException();
        }

        public virtual bool IsDisabled => false;


        public BoxCollider2D ColliderCmp {
            get {
                if (_collider == null)
                    _collider = GetComponent<BoxCollider2D>();
                return _collider;
            }
        }//ColliderCmp


    }//SlowdawnStatus
}//namespace
*/