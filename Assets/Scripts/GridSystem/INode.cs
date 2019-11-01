using UnityEngine;

/// <summary>
/// 
/// </summary>
public interface INode{

    /// <summary>
    ///  Distance from this node to the start.
    /// </summary>
    int GetDistanceCost();
    void SetDistanceCost(int val);

    /// <summary>
    ///  Distance from this node to the destination.
    /// </summary>
    int GetHeuristicCost();
    void SetHeuristicCost(int val);

    /// <summary>
    ///  Sum of DistanceCost with HeuristicCost
    /// </summary>
    /// <param name="direction"> 0 is horizontal and vertical; 1 is diagonal;</param>
    int GetCost();

    void SetWalkable(bool state);
    bool GetWalkable();

    void SetPosition(Vector2 pos);
    Vector2 GetPosition();

    /// <summary>
    ///  Sets coords X and Y of the Node on the Grid.
    /// </summary>
    void SetCoord(int x, int y);
    
    /// <summary>
    ///  Return [x,y] coords of this Node on the Grid
    /// </summary>
    /// <returns></returns>
    int[] GetCoords();

    INode GetParent();
    void SetParent(INode parent);

}//class INode
