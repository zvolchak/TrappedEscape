using UnityEngine;

/// <summary>
///  Crosshair is something a Weapon shoots towards. This component
/// should be added as a child of the Weapon object.
/// </summary>
public class Crosshair : MonoBehaviour{

    public CrossProps Props;

    /// <summary>
    /// Positive - going up; Negative - going down.
    /// </summary>
    private float direction = 1;
    private float initYPos;
    private bool bHasRegistered;
    private bool bIsFreeze;
    private Weapon _weapon;


    public void Start() {
        this.initYPos = this.transform.localPosition.y;

        this.init();
    }//Start


    public void OnEnable() {
        init();
        Reset();
    }//OnEnable


    public void FixedUpdate() {
        if(!bHasRegistered)
            this.init();
    }//FixedUpdate


    /// <summary>
    /// Called on Start (or OnEnable) to get the Weapon component in the
    /// parent object and subscribe to some of its events.
    /// </summary>
    private void init() {
        if (bHasRegistered)
            return;

        if(this.transform.parent == null)
            return;

        if (_weapon == null) {
            _weapon = this.transform.parent.GetComponent<Weapon>();
            if(_weapon == null)
                return;
        }

        _weapon.OnShootListeners -= FreezeUnfreeze;
        _weapon.OnShootListeners += FreezeUnfreeze;
        _weapon.OnShootListeners -= this.Props.ReduceRange;
        _weapon.OnShootListeners += this.Props.ReduceRange;
        bHasRegistered = true;
    }//init


    public void Update() {
        if(this.Props.IsRecoverRange)
            this.Props.RecoverRange();
        OnSwing();
    }//Start


    public void OnSwing() {
        if(bIsFreeze)
            return;
        if(Props.SwingRange <= 0)
            return;

        Vector3 currentPosition = this.transform.localPosition;
        Vector3 destination = new Vector3(currentPosition.x, SwingBorder, currentPosition.z);
        this.transform.Translate(direction * Vector3.up * this.Props.CurrentSpeed * Time.deltaTime);

        if (this.direction > 0) {
            if (this.transform.localPosition.y >= SwingBorder) {
                this.direction = -1;
            }
        } else {
            if (this.transform.localPosition.y <= SwingBorder)
                this.direction = 1;
        }
    }//Swing


    public float SwingBorder {
        get {
            if(direction > 0)
                return this.initYPos + this.Props.CurrentSwingRange;
            else
                return this.initYPos - this.Props.CurrentSwingRange;
        }
    }//SwingBorder


    public void FreezeUnfreeze() {
        CancelInvoke();
        SetFreeze();
        Invoke("SetUnfreeze", this.FreezeTime);
    }//FreezeUnfreeze


    public void SetFreeze() {
        bIsFreeze = true;
    }//SetFreeze


    public void SetUnfreeze() {
        bIsFreeze = false;
    }//SetUnfreeze


    public float FreezeTime {
        get {
            if(_weapon == null)
                return 0f;
            return _weapon.Props.RateOfFire * this.Props.FreezeRelativeToRate;
        }//get
    }//FreezeTime


    public void Reset() {
        this.direction = 1;
        bIsFreeze = false;
        this.Props.Reset();
    }//Reset

}//class Crosshair


[System.Serializable]
public class CrossProps {

    public float SwingSpeed = 0.3f;
    public float SwingRange = 0.5f;

    [Tooltip("Amount by which Range is decreased at each shot.")]
    public float RangeDecreaseRate = 0.05f;
    [Range(0f, 1f)]
    [Tooltip("How much swing can be decresed relative to original SwingRange (e.g. SwingRange * MaxDecrese)")]
    public float MaxDecreaseRatio = 0.5f;
    [Tooltip("How many seconds of unshooting before swing range starts returning to origin.")]
    public float RangeRegenTimeout = 0.2f;
    [Tooltip("Amount at which SwingRange recovers back to original Range")]
    public float RangeRecoverRate = 0.1f;

    [Range(0, 1f)] [Tooltip("Relative to the weapon RateOfFire how much time will cross freeze on shoot (e.g. Rate * Freeze)")]
    public float FreezeRelativeToRate = 0.5f;


    private float currSpeed = -1;
    private float currRange = -1;
    private float lastReduceRangeTimer = -1;


    public void ReduceRange(float rate = -1) {
        if (rate == -1)
            rate = this.RangeDecreaseRate;

        this.CurrentSwingRange -= rate;
        if (this.CurrentSwingRange < 0)
            this.CurrentSwingRange = 0;
        if (this.CurrentSwingRange <= this.MaxDecreaseRange)
            this.CurrentSwingRange = this.MaxDecreaseRange;

        this.lastReduceRangeTimer = Time.realtimeSinceStartup;
    }//ReduceRange


    public void ReduceRange() {
        ReduceRange(-1);
    }//ReduceRange


    public void RecoverRange(float rate = -1) {
        if (rate == -1)
            rate = this.RangeRecoverRate;
        this.CurrentSwingRange += rate;
        if (this.CurrentSwingRange >= this.SwingRange)
            this.CurrentSwingRange = this.SwingRange;
    }//RecoverRange


    public void RecoverRange() {
        RecoverRange(-1);
    }


    public float CurrentSwingRange {
        get {
            if(this.currRange == -1)
                this.currRange = SwingRange;
            return this.currRange;
        }
        private set { this.currRange = value; }
    }//CurrentSwingRange


    public float CurrentSpeed {
        get {
            var dc = this.SwingRange - this.CurrentSwingRange;
            var ratio = dc / this.SwingRange;
            return this.SwingSpeed - (this.SwingSpeed * ratio) / 2;
        }
        private set { this.currSpeed = value; }
    }//CurrentSpeed


    public float MaxDecreaseRange {
        get {
            return this.SwingRange - (this.SwingRange * this.MaxDecreaseRatio);
        }//get
    }//MaxDecreaseRange


    public bool IsRecoverRange {
        get {
            return this.lastReduceRangeTimer != -1 &&
                    Time.realtimeSinceStartup - this.lastReduceRangeTimer >= this.RangeRegenTimeout;
        }//get
    }//IsRevocerRange


    public void Reset() {
        this.CurrentSwingRange = SwingRange;
        this.CurrentSpeed = SwingSpeed;
        this.lastReduceRangeTimer = -1;
    }//Reset

}//class
