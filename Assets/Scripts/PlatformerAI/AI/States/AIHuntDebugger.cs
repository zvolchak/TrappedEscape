using System.Collections.Generic;
using UnityEngine;
using GHAI;

public class AIHuntDebugger : MonoBehaviour{

    private AIController _controller;
    private PathDebugger _pathDebugger;
    private bool bHasInit = false;


    public void Start() {
        _controller = GetComponent<AIController>();

    }//Start


    public void Update() {
        if (!bHasInit) {
            _pathDebugger = PathDebugger.Instance;
            if (_pathDebugger == null)
                return;
            _pathDebugger.EOnPointSelected += OnNavigationSelected;
            bHasInit = true;
        }
    }//Update


    public void OnNavigationSelected(List<INode> path) {
        if(path == null)
            return;
        if (path.Count == 1) {
            _controller.StateMachineAnimator.SetTrigger("Hunt");
        }
    }//OnNavigationSelected


}//class
