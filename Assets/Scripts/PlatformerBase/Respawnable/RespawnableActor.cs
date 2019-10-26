using UnityEngine;


/// <summary>
///  An AACtor with a Damagable component that can be respawned one way or another.
///  
/// A component should register to the "death" even by calling RegisterToDestroyEvent()
/// function on initiation state to get called when actor dies.
/// </summary>
public class RespawnableActor : ARespawnable {

    private Damageable _dmgbl;

    public override void Update() {
        base.Update();
        if (!this.bIsRegistered) {
            if (DamagableCmp != null) {
                DamagableCmp.EOnDeath += OnDestroyInvoked;
                this.bIsRegistered = true;
            }
        }
    }//Update


    public override void Respawn() {
        DamagableCmp.Reset();
    }//Respawn


    //public void RegisterToDestroyEvent(Damagable.OnDeathEvent callback) {
    //    DamagableCmp.EOnDeath -= callback;
    //    DamagableCmp.EOnDeath += callback;
    //}//RegisterToDestroyEvent

    public void OnDestroyInvoked(GameObject instigator) {
        this.EOnDestroyed?.Invoke(this);
    }


    public Damageable DamagableCmp {
        get {
            if(_dmgbl == null)
                _dmgbl = GetComponent<Damageable>();
            return _dmgbl;
        }
    }

}//RespawnableActor
