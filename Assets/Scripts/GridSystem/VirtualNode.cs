using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualNode : INode{

    private bool bIsWalkable;
    private Vector2 worldPosition;
    private int[] coords = new int[2];
    private int distanceCost;
    private int heuristicCost;
    private INode parent;
    private BoxCollider2D _boxCollider;

    public List<INode> Neighbours { get; private set; }


    public VirtualNode(bool walkableState, Vector2 worldPos, int coordX, int coordY) {
        SetWalkable(walkableState);
        SetPosition(worldPos);
        SetCoord(coordX, coordY);
    }//ctor


    public void AddNeighbour(INode node) {
        if(node == null)
            return;
        if(this.Neighbours == null)
            this.Neighbours = new List<INode>();
        if(this.Neighbours.Contains(node))
            return;

        this.Neighbours.Add(node);
    }//node

    /******************* GETTERS / SETTERS *******************/

    public int GetDistanceCost() {
        return this.distanceCost;
    }//GetDistanceGost


    public void SetDistanceCost(int val) {
        this.distanceCost = val;
    }//SetDistanceCost


    public int GetHeuristicCost() {
        return this.heuristicCost;
    }//GetHeuristicCost


    public void SetHeuristicCost(int val) {
        this.heuristicCost = val;
    }//SetHeuristicCost


    public int  GetCost() { return GetDistanceCost() + GetHeuristicCost(); }
    public void SetWalkable(bool state) { this.bIsWalkable = state; }
    public bool GetWalkable() { return this.bIsWalkable; }

    public void SetPosition(Vector2 pos) { this.worldPosition = pos; }
    public Vector2 GetPosition() { return this.worldPosition; }

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


    public void OnDrawGizmos() {
        //if (this.Node == null)
        //    return;
        if (this.Neighbours == null || this.Neighbours.Count == 0)
            return;

        for (int i = 0; i < this.Neighbours.Count; i++) {
            INode node = this.Neighbours[i];
            if (node == null)
                continue;
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(this.worldPosition, node.GetPosition());
        }//for
    }//OnDrawGizmos

}//class
