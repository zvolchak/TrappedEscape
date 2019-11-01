using System.Collections.Generic;
using UnityEngine;


public class PathDebugger : MonoBehaviour {

    public List<GridNode> TargetPoints;
    public List<GridNode> Path;

    private Pathfinder _pathFinder;


    public void Start() {
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
        GridNode node = Grid.GetNodeFromWorld(worldPosition);
        if(node == null)
            return;

        if (TargetPoints.Contains(node)) {
            TargetPoints.Remove(node);
            Path.Clear();
            return;
        }

        if (TargetPoints.Count == 2) {
            TargetPoints.RemoveAt(1);
        }

        TargetPoints.Add(node);
    }//AddPoint


    public void OnDrawGizmos() {
        if(TargetPoints == null || TargetPoints.Count == 0)
            return;

        for (int i = 0; i < TargetPoints.Count; i++) {
            GridNode node = TargetPoints[i];
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.GetPosition(), 0.3f);
        }//for

        for (int i = 0; i < Path.Count; i++) {
            Gizmos.color = Color.blue;
            if (i > 0) {
                Gizmos.DrawLine(Path[i - 1].GetPosition(), Path[i].GetPosition());
            }
            if(i == Path.Count - 1)
                continue;

            GridNode node = Path[i];
            Gizmos.DrawSphere(node.GetPosition(), 0.3f);
        }//for
    }//OnDrawGizmos


    public GridSystem Grid => GridSystem.Instance;

}//class
