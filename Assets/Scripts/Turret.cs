using System.Collections.Generic;
using UnityEngine;


///<summery>
/// A Turret is an object that has the Gun and AutoShoot components. It will start
/// firiging in the direction its localScale.x is pointing to whenever it sees
/// the TargetLayer. Turret will stop shooting after N seconds of not seeing the
/// target.
///</summery>
[RequireComponent(typeof(AutoShoot))]
public class Turret : MonoBehaviour {

    public float VisionDistance = 10f;
    [Tooltip("Time between turret notice the target and starts shooting.")]
    public float StartDealy = 2f;
    [Tooltip("How many seconds of not seeing a TargetLayer for the Turret to stop.")]
    public float ShutOffTimeout = 3f;
    public LayerMask TargetLayer;
    public List<string> IgnoreTags;
    public Sfx TargetSpottedEffect;

    private AutoShoot _autoShoot;
    private RaycastHit2D visionRay;
    private float timeSinceLastVision = 0;
    /// <summary>
    ///  Flag to indigate that the Target has been spoted and Turret is turning on
    /// to start shooting.
    /// </summary>
    private bool bIsTurningOn = false;
    private bool bHasSpotted = false;


    public void Update() {
        CheckTarget();

        if (!this.visionRay && this.AutoShootCmp.IsShooting) {
            if (Time.timeSinceLevelLoad - timeSinceLastVision >= ShutOffTimeout) {
                this.AutoShootCmp.Trigger(false);
                this.timeSinceLastVision = 0f;
                bHasSpotted = false;
            }
        }//if

        if (this.timeSinceLastVision > 0 && !this.AutoShootCmp.IsShooting) {
            ToggleTurret();
        }//if
    }//Update


    public void CheckTarget() {
        //FIXME: Add Solid layer mask to the raycast!
        this.visionRay = Physics2D.Raycast(this.transform.position, VisionVector.normalized, VisionDistance , TargetLayer);

        if(!this.visionRay)
            return;

        if (IgnoreTags.Contains(this.visionRay.collider.tag)) {
            return;
        }

        if (!bHasSpotted) {
            if (TargetSpottedEffect != null) {
                TargetSpottedEffect.PlayParticles(TargetSpottedEffect.transform.position);
            }
            bHasSpotted = true;
        }

        if(!bIsTurningOn)
            this.timeSinceLastVision = Time.timeSinceLevelLoad;
    }//CheckTarget


    public void ToggleTurret() {
        if (Time.timeSinceLevelLoad - timeSinceLastVision <= StartDealy) {
            bIsTurningOn = true;
            return;
        }

        if (this.AutoShootCmp.IsShooting)
            return;

        this.AutoShootCmp.Trigger(true);
        bIsTurningOn = false;
    }//ToggleTurret


    /// <summary>
    ///  Return a vector direction (length) towards which this turret is looking
    /// for the target.
    /// </summary>
    public Vector3 VisionVector {
        get {
            return this.transform.right * VisionDistance * Mathf.Sign(this.transform.localScale.x);
        }//get
    }//VisionVector


    public void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector3 fromPos = this.transform.position;
        Gizmos.DrawLine(fromPos, VisionVector + fromPos);
    }//OnDrawGizmos


    public AutoShoot AutoShootCmp {
        get {
            if(_autoShoot == null)
                _autoShoot = GetComponent<AutoShoot>();
            return _autoShoot;
        }
    }


}//Turret
