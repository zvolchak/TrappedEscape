using System.Collections.Generic;
using UnityEngine;


///<summery>
/// Will subscribe to a Damagable OnDamageTaken and OnDeath event and spawn particles.
///</summery>
[RequireComponent(typeof(Damageable))]
public class DamageEffects : MonoBehaviour {

    public ParticleSystem DmgParticles;
    public List<ParticleSystem> DeathParticles;

    private Damageable _dmg;


    public virtual void Start() {
        _dmg = GetComponent<Damageable>();
        _dmg.EOnDamageTaken += OnDamageTaken;
        _dmg.EOnDeath += OnDeath;
        _dmg.EOnCritHit += OnDamageTaken;
    }//Start


    public virtual void SpawnParticles(ParticleSystem toSpawn, Vector3 position) {
        if (toSpawn == null)
            return;

        var spawned = Instantiate(toSpawn);
        var rot = spawned.transform.rotation;
        var direction = position - this.transform.position;
        var sign = Mathf.Sign(direction.x);
        if (sign > 0)
            spawned.transform.Rotate(-30, 0, 0);
        else
            spawned.transform.Rotate(-180, 0, 0);

        spawned.transform.position = this.transform.position;
        //spawned.transform.position = position;
    }

    public void OnDamageTaken(GameObject instigator, int dmgAmount) {
        this.SpawnParticles(DmgParticles, instigator.transform.position);
    }//SpawnParticles


    public void OnDeath(GameObject instigator) {
        if(DeathParticles == null || DeathParticles.Count == 0)
            return;
        int index = Random.Range(0, DeathParticles.Count);
        this.SpawnParticles(this.DeathParticles[index], instigator.transform.position);
    }//OnDeath


}//DamagEffects
