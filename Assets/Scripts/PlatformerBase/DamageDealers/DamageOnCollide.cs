using UnityEngine;
using GHTriggers;


/// <summary>
///  Deal damage OnTriggerEnter2D.
/// </summary>
public class DamageOnCollide : TriggerListener{

    public int DamageAmount;

    public override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if(!HasEntered)
            return;
        Damageable dmgbl = collision.GetComponent<Damageable>();
        if(dmgbl == null)
            return;
        dmgbl.TakeDamage(this.gameObject, DamageAmount, false);
    }//OnTriggerEnter2D

}//DamageDealer
