using UnityEngine;
using GHAbilities;
using GHWeaponry;

using GHPlatformerControls;

///<summery>
///
///</summery>
[RequireComponent(typeof(Animator))]
public class AIController : MonoBehaviour {

    public AIProps Props;
    public AActor2D ControlledBy;
    public Holster HolsterCmp;
    public SprintControls SprintCmp;
    public AActor2D Target;

    private Animator _animator;
    public Collider2D ColliderCmp { get; private set; }


    public void Start() {
        //Not set manually? Therefore either in parent or in self.
        if (ControlledBy == null) {
            ControlledBy = GetComponentInParent<AActor2D>();
            if(ControlledBy == null)
                ControlledBy = GetComponent<AActor2D>();
        }

        ColliderCmp = GetComponentInChildren<Collider2D>();

        DamagableCmp = ControlledBy.DamagableCmp;
        DamagableCmp.EOnDamageTaken += OnDamageTaken;
        DamagableCmp.EOnCritHit += OnDamageTaken;
        StateMachineAnimator.Rebind();
    }//Start


    public void Update() {
    }//Update


    public void OnDamageTaken(GameObject instigator, int amount) {
        _animator.SetTrigger("TakingDamage");
    }


    public bool IsInVisibleRange() {
        if(Player.Instance == null)
            return false;

        if (this.Target == null)
            this.Target = Player.Instance;

        var distance = (this.transform.position - this.Target.transform.position).magnitude;
        if (Props == null) {
#if UNITY_EDITOR
            Debug.LogError("AIProps of the AIController for " + this.name + " not set!");
#endif
            return false;
        }
        return distance <= Props.VisionRange;
    }//CalculateDistance


    public bool IsVisionBlocked(LayerMask blockingMask, bool isDebug=false) {
        Vector3 aiPos = this.transform.position;
        Vector3 dir = Target.transform.position - aiPos;
        RaycastHit2D ray = Physics2D.Raycast(aiPos, dir, dir.magnitude, blockingMask);

        if (isDebug) {
            Color rayColor = ray ? Color.red : Color.green;
            Debug.DrawRay(aiPos, dir, rayColor);
        }

        if (!ray) {
            return false;
        }

        if (isDebug) {
            dir = (ray.collider.transform.position - aiPos);
        }

        return true;
    }//IsVisionBlocked


    public bool IsLookingAtTarget {
        get {
            Vector3 dir = Target.transform.position - this.transform.position;
            float currDirSign = Mathf.Sign(this.transform.parent.localScale.x);
            return Mathf.Sign(dir.x) == currDirSign;
        }//get
    }//IsLookingAtTarget


    public Animator StateMachineAnimator {
        get {
            if(_animator == null)
                _animator = GetComponent<Animator>();
            return _animator;
        }
    }

    public SwitchDirection DirSwitcherCmp => ControlledBy.DirectionSwitcherCmp;
    public MovementControls MvmntCmp => ControlledBy.MvmntCmp;
    public Animator AnimationCmp => ControlledBy.AACmp.AnimatorCmp;
    public Damageable DamagableCmp { get; private set; }

}//AIController
