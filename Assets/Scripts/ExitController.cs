using GHGameManager;
using GHTriggers;
using UnityEngine;


public class ExitController : TriggerListener{

    public EnemyTracker EnemyTrackerCmp;

    private Animator _animator;
    private bool bIsOpened = false;
    private bool bHasDoorUsed = false;


    public override void Start() {
        base.Start();
        _animator = GetComponent<Animator>();
        EnemyTrackerCmp.EOnAllEnemiesDied += EnemiesCleared;

        _animator.SetBool("IsOpened", this.bIsOpened);
    }//Start


    public void Update() {
        if (bHasDoorUsed) {
            return;
        } else {
            if(MmUi.LevelCompleteScreen.activeSelf)
                MmUi.LevelCompleteScreen.SetActive(false);
        }

        if (IsStaying && bIsOpened) {
            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0) {
                bHasDoorUsed = true;
                Debug.Log("Exit Door Used... Level complete");
                MmUi.LevelCompleteScreen.SetActive(true);
            }
        }
    }//Update


    public void EnemiesCleared(EnemyTracker et) {
        bIsOpened = true;

        _animator.SetBool("IsOpened", this.bIsOpened);
    }//EnemiesCleared


    public MainMenuUI MmUi => MainMenuUI.Instance;

}//class
