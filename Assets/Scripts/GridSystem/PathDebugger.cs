using System.Collections.Generic;
using UnityEngine;


public class PathDebugger : MonoBehaviour {

    public static PathDebugger Instance;
    public int MaxTargetPoints = 1;

    public List<INode> TargetPoints = new List<INode>();
    public List<INode> Path = new List<INode>();

    private Pathfinder _pathFinder;

    public delegate void OnPointSelected(List<INode> path);
    public OnPointSelected EOnPointSelected;


    public void Start() {
        if(Instance == null) Instance = this;
        else return;
        _pathFinder = GetComponent<Pathfinder>();
    }//Start


    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.AddPoint(mousePos);
            if (TargetPoints.Count == 2) {
                Path = _pathFinder.FindPath(TargetPoints[0].GetPosition(), 
                                        TargetPoints[1].GetPosition());
            }
        }
    }//Update


    public void AddPoint(Vector3 worldPosition) {
        INode node = Grid.GetNodeFromWorld(worldPosition);
        if(node == null)
            return;

        if (TargetPoints.Contains(node)) {
            TargetPoints.Remove(node);
            Path.Clear();
            return;
        }

        if (TargetPoints.Count >= MaxTargetPoints) {
            TargetPoints.RemoveAt(TargetPoints.Count - 1);
        }

        TargetPoints.Add(node);
        this.EOnPointSelected?.Invoke(this.TargetPoints);
    }//AddPoint


    public void OnDrawGizmos() {
        if (TargetPoints == null || TargetPoints.Count == 0)
            return;

        for (int i = 0; i < TargetPoints.Count; i++) {
            INode node = TargetPoints[i];
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.GetPosition(), 0.3f);
        }//for

        if (this.Path == null)
            return;

        for (int i = 0; i < this.Path.Count; i++) {
            Gizmos.color = Color.blue;
            if (i > 0) {
                Gizmos.DrawLine(this.Path[i - 1].GetPosition(), this.Path[i].GetPosition());
            }
            if (i == this.Path.Count - 1)
                continue;

            INode node = this.Path[i];
            Gizmos.DrawSphere(node.GetPosition(), 0.3f);
        }//for
    }//OnDrawGizmos


    public GridSystem Grid => GridSystem.Instance;

}//class
