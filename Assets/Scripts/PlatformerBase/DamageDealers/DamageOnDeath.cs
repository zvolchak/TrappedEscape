using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnDeath : MonoBehaviour, IDamageDealer {

    private IDamageDealer _dmgDealer;
    private Damageable _dmgble;


    public void Start() {
        _dmgDealer = GetComponent<IDamageDealer>();
        _dmgble = GetComponent<Damageable>();

        _dmgble.EOnDeath -= deathInvoked;
        _dmgble.EOnDeath += deathInvoked;
    }


    public void DealDamage(Damageable dmgble) {
        _dmgDealer.DealDamage(dmgble);
    }//DealDamage


    private void deathInvoked(GameObject go) {
        DealDamage(_dmgble);
    }//deathInvoked

}//class
