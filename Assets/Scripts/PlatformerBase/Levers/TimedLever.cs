using System.Collections;
using UnityEngine;
using GHTriggers;

///<summery>
///</summery>
public class TimedLever : Lever {

    public float ToggledTime = 0.5f;
    public float Timeout = 0.3f;

    private Coroutine shutdownRoutine;
    private bool bIsCanToggle = true;


    public override void Toggle() {
        if(bIsCanToggle)
            base.Toggle();

        if (this.shutdownRoutine == null)
            this.shutdownRoutine = StartCoroutine(DelayShutdown());
        //if (state) {
        //}
    }//Toggle


    public IEnumerator DelayShutdown() {
        bIsCanToggle = true;

        yield return new WaitForSeconds(ToggledTime);
        Toggle(); //switch back to "off" state.
        bIsCanToggle = false;

        yield return new WaitForSeconds(Timeout);
        this.shutdownRoutine = null;
        bIsCanToggle = true;
        ToggleAnimation(false);
    }//DelayShutdown


    public override bool IsCanTrigger(GameObject theOneToTrigger) {
        bool isTrigger = base.IsCanTrigger(theOneToTrigger);
        return shutdownRoutine == null && isTrigger;
    }


}//TimedLever
