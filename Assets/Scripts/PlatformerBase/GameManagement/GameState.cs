using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour{

    public static GameState Instance;

    public enum State { playing, pause, levelend }
    public State CurrentState { get; private set; }

    private bool bHasInit;
#if UNITY_EDITOR
    public State StateDebugger;
#endif

    public void Start() {
        if (Instance != null) {
#if UNITY_EDITOR
            Debug.LogWarning(string.Format("{0} is destroyed! GameState instance duplicate!", this.name));
#endif
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }//Start


    public void Update() {
#if UNITY_EDITOR
        this.StateDebugger = this.CurrentState;
#endif
        if (!bHasInit)
            this.init();

        if (Input.GetButtonDown("Reload")) {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }//Update


    private void init() {
        if(LevelEnd.Instance == null)
            return;

        LevelEnd.Instance.EOnLevelEndListeners -= SetStateLevelEnd;
        LevelEnd.Instance.EOnLevelEndListeners += SetStateLevelEnd;

        SetStatePlaying();
        bHasInit = true;
    }//init


    public void SetStatePlaying() { this.CurrentState = State.playing; }
    public void SetStatePause() { this.CurrentState = State.pause; }
    public void SetStateLevelEnd() { this.CurrentState = State.levelend; }


    public virtual void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }//OnEnable


    public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        bHasInit = false;
    }//OnSceneLoaded

}//class
