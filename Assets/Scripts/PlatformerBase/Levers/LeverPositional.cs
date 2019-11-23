using System.Collections;
using UnityEngine;
using GHTriggers;

///<summery>
///</summery>
public class LeverPositional : Lever {

    public float TriggerDelay = 0f;

    private Coroutine routine;


    public override void OnTriggerEnter2D(Collider2D collision) {
        if (!IsCanTrigger(collision.gameObject))
            return;
        //this.ToggleAnimation(this.currState);
        //ToggleAnimation(true);
        if (routine == null) {
            routine = StartCoroutine(DelayTrigger());
        }
    }//OnTriggerEnter2D


    public IEnumerator DelayTrigger() {

        yield return new WaitForSeconds(TriggerDelay);

        int size = this.Reactors.Count;
        for (int i = 0; i < size; i++) {
            var reactor = Reactors[i];
            reactor.Activate();
        }//for

        this.timeOfToggle = Time.timeSinceLevelLoad;

        if (ToggleTimeout == 0) {
            this.routine = null;
            yield break;
        }

        yield return new WaitForSeconds(ToggleTimeout);

        for (int i = 0; i < size; i++) {
            var reactor = Reactors[i];
            reactor.Deactivate();
        }//for

        this.routine = null;
    }//DelayTrigger


    public override void OnTriggerExit2D(Collider2D collision) {
        //if (!IsCanTrigger(collision.gameObject))
        //    return;
        if (!IsTargetLayer(collision.gameObject))
            return;

        if(ToggleTimeout > 0)
            return;

        int size = this.Reactors.Count;
        for (int i = 0; i < size; i++) {
            var reactor = Reactors[i];
            reactor.Deactivate();
        }//for
    }//OnTriggerExit2D


}//LeverPositional
