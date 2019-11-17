using UnityEngine;


public class SwitchDirection : MonoBehaviour {
    
    public float SwitchAnimationTime = 0.08f;    
    private Animator _animator;
    private bool bTrySwitchWhenGrounded;
    private float switchCallTime;


    public void Start() {
        _animator = GetComponent<Animator>();
    }//Start


    public void Update() {
        //if (!PlayerCmp.CanMove)
        //    return;

        //float horizontalAxis = Input.GetAxis("Horizontal");
        //if (horizontalAxis != 0) {
        //    float sign = Mathf.Sign(horizontalAxis);
        //    if(Mathf.Sign(this.transform.localScale.x) != sign)
        //        OnSwitchDirection();
        //}

        //if (bTrySwitchWhenGrounded && PlayerCmp.IsGrounded) {
        //    var timeDiff = Time.realtimeSinceStartup - this.switchCallTime;
        //    if (timeDiff <= 0.3f) {
        //        OnSwitchDirection(PlayerCmp.transform.localScale.x * -1);
        //    }
        //    bTrySwitchWhenGrounded = false;
        //}
    }//Update


    public void OnSwitchDirection(float val = 0) {
        if(!IsCanSwitch)
            return;
        Vector3 localScale = this.transform.localScale;

        if (val == 0)
            this.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
        else {
            val = Mathf.Sign(val);
            this.transform.localScale = new Vector3(val, localScale.y, localScale.z);
        }
        IsSwitching = false;
        //if (!_player.IsGrounded && val == 0) {
        //    bTrySwitchWhenGrounded = true;
        //    this.switchCallTime = Time.realtimeSinceStartup;
        //    return;
        //}
        //StartCoroutine(AnimateDirection(val));
    }//SwitchDirection


    public System.Collections.IEnumerator AnimateDirection(float val = 0) {
        if(_animator == null)
            _animator = GetComponent<Animator>();

        _animator.SetBool("SwitchDir", true);
        IsSwitching = true;

        yield return new WaitForSeconds(SwitchAnimationTime);

        Vector3 localScale = this.transform.localScale;
        if (val == 0)
            this.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
        else {
            val = Mathf.Sign(val);
            this.transform.localScale = new Vector3(val, localScale.y, localScale.z);
        }
        
        _animator.SetBool("SwitchDir", false);
        IsSwitching = false;
    }//AnimateDirection


    private bool checkCanSwitch() {
        return true;
    }//checkCanSwitch


    /* **************** GETTERS **************** */

    public bool IsCanSwitch { get { return this.checkCanSwitch(); } }
    public bool IsSwitching { get; private set; }
    public float Direction { get { return Mathf.Sign(this.transform.localScale.x); } }
    //public Player PlayerCmp => Player.Instance;

}//class SwitchDirection
