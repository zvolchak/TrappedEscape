using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    public static MainMenuUI Instance;

    [Tooltip("Index of the main menu scene to be loaded when there are no next levels.")]
    public int MainMenuIndex = 0;
    public GameObject DefaultScreen;
    public GameObject LevelCompleteScreen;


    //private GamepadNavigation _gamepadNav;
    private List<GameObject> menuScreens;
    private Dictionary<string, int> menuIndices;
    private Canvas _canvas;
    private bool bIsInited;
    private int origDefaultScreenIndex;

    public void Start() {
        if(Instance != null)
            return;

        Instance = this;
    }//Start


    public void Init() {
        //_gamepadNav = GetComponent<GamepadNavigation>();
        this.menuIndices = new Dictionary<string, int>();

        menuScreens = new List<GameObject>();
        int index = 0;
        foreach (Transform child in this.transform) {
            menuScreens.Add(child.gameObject);
            this.menuIndices.Add(child.name, index);

            if (child.gameObject == DefaultScreen)
                origDefaultScreenIndex = index;

            index++;
        }//foreach

        bIsInited = true;
    }//Start


    /// <summary>
    ///  Load next level available in the scene manager after currently loaded one. 
    /// If no level available - drop back to main menu.
    /// </summary>
    public void NextLevel() {
        int currLevel = SceneManager.GetActiveScene().buildIndex;
        int nextLevel = currLevel + 1;
        bool isNextLevelExist = true;

        try {
            SceneManager.GetSceneByBuildIndex(nextLevel).IsValid();
        } catch (System.ArgumentException) {
            isNextLevelExist = false;
        }

        if (!isNextLevelExist) {
#if UNITY_EDITOR
            GameUtils.Utils.WarningMessage("Next Level with index " + nextLevel + " does not exist!");
#endif
            SceneManager.LoadScene(MainMenuIndex);
        }
        SceneManager.LoadScene(nextLevel);

        //int gemsReqs = GameManager.Instance.GetGemsRequirement("lvl_" + nextLevel);
        //int gemsSoFar = SaveLoad.Instance.GetGemsCollected();
        //if (gemsSoFar < gemsReqs) {
        //    if (this.menuIndices.ContainsKey("LevelSelect"))
        //        this.DefaultScreen = menuScreens[this.menuIndices["LevelSelect"]];

        //    if(this.DefaultScreen != null)
        //        this.DefaultScreen.SetActive(true);

        //    SceneManager.LoadScene(MainMenuIndex);
        //} else {
        //    SceneManager.LoadScene(nextLevel);
        //}
    }//NextLevel


    public void ExitApplication() {
        Application.Quit();
    }//ExitApplication


    public void RestartScene() {
        Time.timeScale = 1f;
        var currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.buildIndex);
    }//RestartScene


    public void ResetGameProgress() {
        //if (SaveLoad.Instance == null)
        //    return;
        //SaveLoad.Instance.ResetProgress();
    }


    public void ResetGameStats() {
        //if(SaveLoad.Instance == null)
        //    return;
        //SaveLoad.Instance.ResetGameStats();
    }


    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }//LoadScene


    public void LoadSceneFromGOName(GameObject go) {
        if (go == null)
            return;
        SceneManager.LoadScene(go.name);
    }


    public void ToggleMenu(GameObject toShow) {
        bool state = !toShow.activeSelf;
        toShow.SetActive(state);

        //if (state) {
        //    if (_gamepadNav != null)
        //        _gamepadNav.SetDefaultButtonSelection(toShow);
        //}
    }//ToggleMenu


    public void HideUI(GameObject toHide) {
        toHide.gameObject.SetActive(false);
    }//HideUI


    public void ShowUI(GameObject toShow) {
        toShow.gameObject.SetActive(true);
    }//ShowUI


    public void SaveSettingsToFile() {
        //if (SaveLoad.Instance == null)
        //    return;
        //SaveLoad.Instance.DataBuffer.SaveGlobalData();
        //SaveLoad.Instance.Save(null);
    }//SaveSettingsToFile


    public void OnEnable() {
        if (!bIsInited)
            Init();
    }//OnEnable


    public void ResetDefaultScreenValue() {
        this.DefaultScreen = this.menuScreens[this.origDefaultScreenIndex];
    }


    public Canvas CanvasCmp {
        get {
            if (_canvas == null)
                _canvas = GetComponent<Canvas>();
            return _canvas;
        }
    }


    public List<GameObject> AllMenuScreens { get { return this.menuScreens; } }


    public GameObject LevelSelectMenu {
        get {
            if (this.menuIndices.ContainsKey("LevelSelect"))
                return menuScreens[this.menuIndices["LevelSelect"]];
            else
                return null;
        }
    }

}//class MainMenuUI
