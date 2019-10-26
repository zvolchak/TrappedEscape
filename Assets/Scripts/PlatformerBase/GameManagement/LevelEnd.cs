using System.Collections;
using UnityEngine;
using GHTriggers;


public class LevelEnd : TriggerListener{

    public static LevelEnd Instance;
    public float DelayTrigger = 0.3f;

    public delegate void OnLevelEndTriggered();
    public OnLevelEndTriggered EOnLevelEndListeners;

    private Coroutine currRoutine;

    public override void Start() {
        if(LevelEnd.Instance != null)
            return;

        Instance = this;
        base.Start();
    }//Start


    public override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if(!HasEntered)
            return;

        if(this.currRoutine == null)
            this.currRoutine = StartCoroutine(this.LevelEndTrigger());
    }//OnTriggerEnter2D


    public IEnumerator LevelEndTrigger() {

        Time.timeScale = 0.4f;

        yield return new WaitForSeconds(DelayTrigger);

        //if (ScreenBlackout.Instance != null)
        //    ScreenBlackout.Instance.SetBlackout(true);

        Time.timeScale = 1;

        //if (UILvlEndCmp != null)
        //    UILvlEndCmp.SetUiState(true);

        this.currRoutine = null;
        EOnLevelEndListeners?.Invoke();
    }//LevelEndTrigger


    //public UILvlEnd UILvlEndCmp => UILvlEnd.Instance;

}//LevelEnd
