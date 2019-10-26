using UnityEngine;


[RequireComponent(typeof(Damageable))]
public abstract class AOnDmgEffect : MonoBehaviour{


    private Damageable _dmg;


    public virtual void Start() {
        Dmgbl.EOnDamageTaken -= OnDamageTaken;
        Dmgbl.EOnDamageTaken += OnDamageTaken;
    }//Start


    public abstract void OnDamageTaken(GameObject instigator, int dmgAmount);


    public void OnEnable() {
        this.Start();
    }//OnEnable


    public void OnDisable() {
        _dmg.EOnDamageTaken -= OnDamageTaken;
    }//OnDisable


    public Damageable Dmgbl {
        get {
            if (_dmg == null)
                _dmg = GetComponent<Damageable>();
            return _dmg;
        }
    }

}//class
