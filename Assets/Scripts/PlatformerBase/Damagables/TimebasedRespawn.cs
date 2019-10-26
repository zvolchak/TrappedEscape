using System.Collections;
using UnityEngine;


/// <summary>
///  This component registers to the ARespawnable component. Respawn() is called
/// when registered ACtor dies. Then, the countdown timer starts to respawn the
/// Actor.
/// </summary>
public class TimebasedRespawn : MonoBehaviour{

    public float RespawnTime = 5f;

    private ARespawnable _respawnable;
    private Coroutine currRoutine;
    private bool bHasRegistered;


    public void Start() {
    }//Start


    public void Update() {
        if (!bHasRegistered) {
            if(RespawnableCmp == null)
                return;
            //RespawnableCmp.RegisterToDestroyEvent(Respawn);
            RespawnableCmp.EOnDestroyed += Respawn;
            bHasRegistered = true;
        }
    }//Update


    public void Respawn(ARespawnable instigator) {
        if(this.currRoutine != null)
            return;

        this.currRoutine = StartCoroutine(OnTimedRespawn());
    }//Respawn


    public IEnumerator OnTimedRespawn() {

        yield return new WaitForSeconds(RespawnTime);

        RespawnableCmp.Respawn();
        this.currRoutine = null;
    }//OnTimedRespawn


    public ARespawnable RespawnableCmp {
        get {
            if (_respawnable == null)
                _respawnable = GetComponent<ARespawnable>();
            return _respawnable;
        }
    }//DamagableCmp

}//TimebasedRespawn
