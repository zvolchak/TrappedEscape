using System.Collections;
using UnityEngine;


public class AOEDamage : MonoBehaviour, IDamageDealer {

    [Tooltip("Time is MaxRadius; Value is Damage at that radius distance.")]
    public AnimationCurve DamageGraph;
    public LayerMask TargetLayer;
    public float DelayDamage = 0f;


    private Coroutine currRoutine;


    public void DealDamage(Damageable dmgble) {
        if (DelayDamage <= 0) {
            this.activate();
            return;
        }

        if(this.currRoutine != null)
            return;

        this.currRoutine = StartCoroutine(OnDealDamage(this.DelayDamage));
    }//DealDamage


    public IEnumerator OnDealDamage(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        this.activate();
        this.currRoutine = null;
    }//OnDealDamage


    private void activate() {
        RaycastHit2D[] ray = Physics2D.CircleCastAll(this.transform.position,
                                                this.Radius,
                                                this.transform.right,
                                                this.Radius,
                                                TargetLayer);

        for (int i = 0; i < ray.Length; i++) {
            RaycastHit2D hit = ray[i];

            LayerMask hitLayer = ray[i].collider.gameObject.layer;
            bool isLayer = GameUtils.Utils.CompareLayers(TargetLayer, hitLayer);
            if (!isLayer)
                continue;

            Damageable toDamage = hit.collider.gameObject.GetComponent<Damageable>();
            if (toDamage == null)
                continue;

            float dmgAmount = GetDamage(hit.distance);
            toDamage.TakeDamage(this.gameObject, (int)dmgAmount, false);
        }
    }//activate


    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, this.Radius);
    }//OnDrawGizmos


    public float GetDamage(float distance) {
        return this.DamageGraph.Evaluate(distance);
    }//GetDamage


    public float Radius => this.DamageGraph.keys[this.DamageGraph.keys.Length - 1].time;
}//class
