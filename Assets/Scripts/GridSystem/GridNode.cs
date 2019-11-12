using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  An invisable layer of the GridSystem that is used for pathfining algorythms.
/// This must not provide any interuction to the user space.
/// </summary>
public class GridNode : MonoBehaviour, INode{

    public List<GridNode> Connections;
    public int MovementCost = 1;

    private bool bIsWalkable = true;
    private Vector2 worldPosition;
    private int[] coords = new int[2];
    private int distanceCost;
    private INode parent;
    private BoxCollider2D _boxCollider;

    public List<INode> Neighbours = new List<INode>();


    public void Start() {
        List<INode> nodes = new List<INode>();
        for (int i = 0; i < Connections.Count; i++) {
            nodes.Add(Connections[i]);
        }
        if (Connections != null)
            this.AddNeighbour(nodes);
    }//Start


    public void AddNeighbour(INode node) {
        if (node == null)
            return;
        if (this.Neighbours == null)
            this.Neighbours = new List<INode>();
        if (this.Neighbours.Contains(node))
            return;

        if(!this.Neighbours.Contains(node))
            this.Neighbours.Add(node);
        //Add Self to the newlly added neighbour for the "cross reference".
        node.AddNeighbour(new List<INode>(new INode[] { this }));
    }//AddNeighbour


    public void AddNeighbour(List<INode> nodes) {
        if(nodes == null)
            return;
        for (int i = 0; i < nodes.Count; i++) {
            this.AddNeighbour(nodes[i]);
        }//for
    }//AddNeighbour


    /******************* Interface *******************/

    public int GetDistanceCost() {
        return this.distanceCost;
    }//GetDistanceGost


    /// <summary>
    ///  Getting set by the Pathfinder algorithm to track the distance from starting
    /// point to destination.
    /// </summary>
    /// <param name="val"></param>
    public void SetDistanceCost(int val) {
        this.distanceCost = val;
    }//SetDistanceCost


    public int GetHeuristicCost() {
        return this.MovementCost;
    }//GetHeuristicCost


    public void SetHeuristicCost(int val) {
        this.MovementCost = val;
    }//SetHeuristicCost


    public int GetCost() { return GetDistanceCost() + GetHeuristicCost(); }


    public void SetWalkable(bool state) { this.bIsWalkable = state; }
    public bool GetWalkable() { return this.bIsWalkable; }

    public void SetPosition(Vector2 pos) {
        this.transform.position = pos;
        //throw new System.NotImplementedException(this.name + " SetPosition not implemented!");
    }//SetPosition

    public Vector2 GetPosition() { return this.transform.position; }

    public void SetCoord(int x, int y) { this.coords = new int[] { x, y }; }
    public int[] GetCoords() { return this.coords; }


    public INode GetParent() {
        return this.parent;
    }


    public void SetParent(INode parent) { this.parent = parent; }


    public override string ToString() {
        return string.Format("Coord: [{0},{1}] -> World: {2}", 
            GetCoords()[0], GetCoords()[1], GetPosition());
    }//ToString


    public override bool Equals(object obj) {
        INode other = obj as INode;
        return this.GetCoords() == other.GetCoords();
    }//Equals


    public override int GetHashCode() { return GetCoords().GetHashCode(); }


    public Vector2 Size => ColliderCmp.size;


    public BoxCollider2D ColliderCmp {
        get {
            if (_boxCollider == null)
                _boxCollider = GetComponentInParent<BoxCollider2D>();
            return _boxCollider;
        }//get
    }//ColliderCmp


    public void OnDrawGizmos() {
        if (this.Neighbours == null || this.Neighbours.Count == 0)
            return;

        for (int i = 0; i < this.Neighbours.Count; i++) {
            INode node = this.Neighbours[i];
            if (node == null)
                continue;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, node.GetPosition());
        }//for
    }//OnDrawGizmos

    public List<INode> GetNeighbours() {
        return this.Neighbours;
    }
}//class GridNode
