using GHMisc;
using GHPlatformerControls;
using UnityEngine;

public class AIPatrol : AIBase {

    public float WaitTimeAtPoint = 0f;

    private WaypointControl _wpCtrl;
    private bool bHasInit = false;
    private float timeOfArrival = -1f;


    public override bool Action() {
        if (NPC.StateMachineAnimator.GetBool("IsDead"))
            Interrupt();
        if (!bHasInit && _wpCtrl == null) {
            _wpCtrl = NPC.GetComponent<WaypointControl>();
            bHasInit = true;
        }
        if(_wpCtrl == null)
            return false;

        Transform currDest = _wpCtrl.GetActivePoint(NPC.ControlledBy.gameObject);
        if (currDest == null) {
            currDest = _wpCtrl.GetNext(NPC.ControlledBy.gameObject);
            _wpCtrl.SetActivePoint(currDest);
        }

        if (currDest == null)
            return false;
        Vector3 npcPos = NPC.ControlledBy.transform.position;
        Vector3 direction = currDest.position - npcPos;
        float distance = Vector3.Distance(npcPos, currDest.position);

        if (distance <= 0.05f) {
            if (this.WaitTimeAtPoint > 0) {
                if (this.timeOfArrival < 0)
                    this.timeOfArrival = Time.timeSinceLevelLoad;
                if (Time.timeSinceLevelLoad - this.timeOfArrival < this.WaitTimeAtPoint)
                    return false;
                else
                    this.timeOfArrival = -1f;
            }//if waittime

            currDest = _wpCtrl.GetNext(NPC.ControlledBy.gameObject);
            _wpCtrl.SetActivePoint(currDest);
        }// if distance

        MovementControls mvmt = NPC.MvmntCmp;
        SwitchDirection dirSwitcher = NPC.DirSwitcherCmp;
        dirSwitcher.OnSwitchDirection(Mathf.Sign(direction.x));
        mvmt.SetVelocityX((mvmt.RunSpeed) * dirSwitcher.Direction);

        return true;
    }//Action

    public override bool Interrupt() {
        if(_wpCtrl != null)
            _wpCtrl.SetActivePoint(null);

        return true;
    }//Interrupt
}//class
