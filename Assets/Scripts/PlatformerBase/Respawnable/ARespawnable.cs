using UnityEngine;


public abstract class ARespawnable : MonoBehaviour, IRespawnable {

#if UNITY_EDITOR
    [Tooltip("UnityEditor variable only! Used to debug Respawn() trigger.")]
    public bool ToggleRespawn;
#endif

    protected bool bIsRegistered;


    public virtual void Update() {
        this.Init();
#if UNITY_EDITOR
        if (ToggleRespawn) {
            this.Respawn();
            ToggleRespawn = false;
        }
#endif
    }//Update


    public virtual void Init() {
        if(this.bIsRegistered)
            return;
    }//Init


    public delegate void OnDestroyEvent(ARespawnable instigator);
    /// <summary>
    /// Signature: void OnDestroyEvent(GameObject instigator);
    /// </summary>
    public OnDestroyEvent EOnDestroyed;

    public abstract void Respawn();

    public virtual bool GetIsActive() { return true; }

    //public abstract void RegisterToDestroyEvent(Damagable.OnDeathEvent callback);

}//IRespawnable
