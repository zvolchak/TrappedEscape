using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GHAbilities {
    [RequireComponent(typeof(DashAbility))]
    public class DashAttack : MonoBehaviour {

        public List<string> TargetTag;
        public float AttackLandedFreezeTime = 0.3f;
        public JumpControls JumpCmp;
        public Player PlayerCmp;

        private DashAbility _dash;
        private SlideAbility _slider;
        private float origTimeScale = -1;


        public void OnTriggerEnter2D(Collider2D collision) {
            if (DashCmp == null)
                return;
            if (!DashCmp.IsDashing) {
                if (!SliderCmp.IsDashing)
                    return;
            }
            if (!SliderCmp.IsDashing) {
                if (!DashCmp.IsDashing)
                    return;
            }

            if (!TargetTag.Contains(collision.tag))
                return;
            if (JumpCmp == null) {
#if UNITY_EDITOR
                string objName = this.transform.parent != null ? this.transform.parent.name : this.name;
                Debug.LogWarning(string.Format("{0} JumpCmp of DashAttack is not set!", objName));
#endif
                return;
            }

            Damageable dmgble = collision.GetComponent<Damageable>();
            if (dmgble == null)
                return;

            dmgble.TakeDamage(PlayerCmp.gameObject, 1000, false);

            StartCoroutine(OnAttackLanded());
        }//OnTriggerEnter2D


        public IEnumerator OnAttackLanded() {
            if (this.origTimeScale > 0)
                yield break;

            this.origTimeScale = Time.timeScale;
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(AttackLandedFreezeTime);

            Time.timeScale = this.origTimeScale;
            this.origTimeScale = -1;
            JumpCmp.AddJumps(1);
        }//OnAttackLanded


        public DashAbility DashCmp {
            get {
                if (_dash == null)
                    _dash = GetComponent<DashAbility>();
                return _dash;
            }
        }

        public SlideAbility SliderCmp {
            get {
                if (_slider == null)
                    _slider = GetComponent<SlideAbility>();
                return _slider;
            }
        }

    }//class
}//namespace