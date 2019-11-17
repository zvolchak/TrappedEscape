using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source based of: https://github.com/SebLague/Pathfinding/blob/master/Episode%2003%20-%20astar/Assets/Scripts/Pathfinding.cs
/* A* Sudo code:

OPEN = priority queue containing START
CLOSED = empty set
while lowest rank in OPEN is not the GOAL:
  current = remove lowest rank item from OPEN
  add current to CLOSED
  for neighbors of current:
    cost = g(current) + movementcost(current, neighbor)
    if neighbor in OPEN and cost less than g(neighbor):
      remove neighbor from OPEN, because new path is better
    if neighbor in CLOSED and cost less than g(neighbor): ⁽²⁾
      remove neighbor from CLOSED
    if neighbor not in OPEN and neighbor not in CLOSED:
      set g(neighbor) to cost
      add neighbor to OPEN
      set priority queue rank to g(neighbor) + h(neighbor)
      set neighbor's parent to current

reconstruct reverse path from goal to start
by following parent pointers
*/
/// </summary>
public class Pathfinder : MonoBehaviour {

    //public static Pathfinder Instance;

    public float SearchTimeout = 20f;
    //public float DebugWaitTime = 0.1f;

    private List<INode> calculatedCosts = new List<INode>();


    /// <summary>
    ///  A delegate function that is called when FindPath function
    /// took too long to find a path.
    /// </summary>
    //public delegate void SearchFailedEvent();
    /// <summary>
    ///  Event called then FindPath function has completed a search.
    /// </summary>
    /// <param name="path">List of nodes forming a complete path.</param>
    /// <returns></returns>
    //public delegate List<GridNode> SearchCompleteEvent(List<GridNode> path);
    

    public List<INode> FindPath(Vector3 from, Vector3 to) {
        INode startNode = GlobalGrid.GetNodeFromWorld(from);
        INode targetNode = GlobalGrid.GetNodeFromWorld(to);

        if (startNode == null) {
#if UNITY_EDITOR
            Debug.LogWarning("FindPath: startNode is null for pos: " + from);
#endif
            return null;
        }

        List<INode> opened = new List<INode>();
        List<INode> closed = new List<INode>();
        opened.Add(startNode);

        float startTime = Time.timeSinceLevelLoad;
        float searchingTime = 0;
        //|| opened.Count > 20
        while (opened.Count > 0 || searchingTime > 0) {
            if(opened.Count == 0)
                break;

            searchingTime = Time.timeSinceLevelLoad - startTime; //safety to avoid long pathsearch and infinite loop.
            INode stepNode = this.findCheapestNode(ref opened);
            opened.Remove(stepNode);
            closed.Add(stepNode);

            if (stepNode == targetNode) {
                var path = this.retracePath(startNode, targetNode);
                return path;
            }

            List<INode> neighbours = stepNode.GetNeighbours();
            for (int n = 0; n < neighbours.Count; n++) {
                INode neighbour = neighbours[n];

                if (!neighbour.GetWalkable() || closed.Contains(neighbour))
                    continue;

                int fromToDistance = this.GetDistance(stepNode, neighbour);

                int newCost = stepNode.GetDistanceCost() + fromToDistance;
                if (closed.Contains(neighbour) && newCost < neighbour.GetDistanceCost())
                    closed.Remove(neighbour);

                if (!opened.Contains(neighbour) && !closed.Contains(neighbour)) {
                    neighbour.SetDistanceCost(newCost);
                    opened.Add(neighbour);
                    int newHeuristic = this.GetDistance(neighbour, targetNode);
                    neighbour.SetHeuristicCost(newHeuristic);
                    neighbour.SetParent(stepNode);
                    this.retracePath(startNode, neighbour);

                    this.calculatedCosts.Add(neighbour);
                }

            }//for
        }//while

        return null;
    }//OnFindPath


    protected INode findCheapestNode(ref List<INode> opened) {
        if (opened.Count == 0)
            return null;

        INode node = opened[0];
        for (int i = 1; i < opened.Count; i++) { //0 index is a default cheapest node
            float currCost = node.GetCost();
            float newCost = opened[i].GetCost();

            if (newCost <= currCost) {
                if(opened[i].GetHeuristicCost() < node.GetHeuristicCost())
                    node = opened[i];
            }
        }//for
        return node;
    }//findCheapestNode


    /// <summary>
    ///  When path from start to end is found, need to retrace, e.g update path
    /// list for line visualization later.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    protected List<INode> retracePath(INode start, INode end) {
        List<INode> path = new List<INode>();
        //Retracing from End to Start and reverse list back in the end.
        INode currentNode = end;

        while (currentNode != start) {
            path.Add(currentNode);
            currentNode = currentNode.GetParent();
        }//while

        //revercing, since list is backwards from End to Start.
        path.Reverse();
        //if(GlobalGrid != null)
        //    GlobalGrid.TracePath = path;

        return path;
    }//retracePath


    public int GetDistance(INode from, INode to) {
        int distX = Mathf.Abs(from.GetCoords()[0] - to.GetCoords()[0]);
        int distY = Mathf.Abs(from.GetCoords()[1] - to.GetCoords()[1]);

        if (distX > distY) return 14 * distY + 10 * (distX - distY);
        else return 14 * distX + 10 * (distY - distX);
    }//GetDistance


    public void Reset() {
        foreach (INode node in this.calculatedCosts) {
            node.SetHeuristicCost(0);
            node.SetDistanceCost(0);
            node.SetParent(null);
        }

        //GridSystem.Instance.GenerateGrid();
    }//Reset


    public GridSystem GlobalGrid {
        get {
            return GridSystem.Instance;
        }
    }//GlobalGrid

}//class Pathfinder
