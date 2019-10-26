using UnityEngine;


public class ActionAnimator : CharacterAnimator{

    private Damageable _damagable;


    public override void Start() {
        base.Start();
        _damagable = GetComponent<Damageable>();
    }//Start


    public override bool InitEvents() {
        if(_damagable == null)
            return false;

        _damagable.EOnDamageTaken += TakeDamageDelegate;
        _damagable.EOnDeath += PlayDeathDelegate;
        _damagable.EOnCritHit += TakeCritDamage;
        return true;
    }


    public void SetWalking(bool state) {
        //AnimatorCmp?.SetBool(Props.Idle, !state);
        AnimatorCmp?.SetBool(Props.Walking, state);
    }//SetWalking


    public void SetIdle(bool state) {
        AnimatorCmp?.SetBool(Props.Walking, !state);
        AnimatorCmp?.SetBool(Props.Idle, state);
    }//SetWalking


    public void TakeDamageDelegate(GameObject instigator, int amount) {
        this.PlayDamaged();
    }


    public void TakeCritDamage(GameObject instigator, int amount) {
        this.PlayCritDamaged();
        AnimatorCmp?.SetBool("HadCrit", true);
    }


    public void PlayDeathDelegate(GameObject instigator) {
        this.PlayDeath();
    }


    public virtual void PlayDamaged() {
        int max = Props.DamageReact.Length;
        if(max == 0)
            return;

        int randPick = Random.Range(0,  max);
        AnimatorCmp?.SetTrigger(Props.DamageReact[randPick]);
    }//PlayDamagedAnimation


    public virtual void PlayCritDamaged() {
        AnimatorCmp?.SetTrigger(Props.CritHitted);
    }//PlayCritDamaged


    public virtual void PlayDeath() {
        int triggerIndex = Random.Range(0, this.Props.DieTrigger.Length);
        AnimatorCmp?.SetTrigger(this.Props.DieTrigger[triggerIndex]);
        AnimatorCmp?.SetBool(Props.Died, true);
    }//PlayDeath


    public void Reset() {
       //AnimatorCmp?.SetBool("HadCrit", false);
       AnimatorCmp?.Rebind();
    }


    public void OnEnable() {
        Reset();
    }

}//class
