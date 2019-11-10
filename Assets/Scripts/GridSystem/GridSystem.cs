using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Source based of: https://github.com/SebLague/Pathfinding/blob/master/Episode%2002%20-%20grid/Assets/Grid.cs
/// </summary>
public class GridSystem : MonoBehaviour{

    public static GridSystem Instance;

    public float TileHalfSize = 1f;
    public LayerMask Unwalkable;
    public DebugGridProps DebugProps;

    //public List<INode> TracePath { get; set; }

    public List<INode> allNodes = new List<INode>();
    public Rect GridBounds = new Rect();


    public void Start() {
        if(Instance == null)
            Instance = this;
        else
            return;

        GenerateGrid();
    }//Start


    public void OnDrawGizmos() {
        if(!DebugProps.IsDrawGrid)
            return;

        if (DebugProps.IsDrawGridBounds) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(GridBounds.center, new Vector3(GridBounds.size.x, GridBounds.size.y, 0.2f));
            Gizmos.color = Color.gray;
        }

        if (Grid == null)
            return;

        //INode actorNode = null;
        //if (GameState.Instance != null) {
        //    if(GameState.Instance.SelectedActor != null)
        //        actorNode = GetNodeFromWorld(GameState.Instance.SelectedActor.Position);
        //}
        foreach (INode node in Grid) {
            if(node == null)
                continue;
            //bool isActor = false;

            //if (actorNode != null && actorNode == node) {
            //    Gizmos.color = DebugProps.ActorColor;
            //    isActor = true;
            //}

            //if (node.GetWalkable())
            //    Gizmos.color = DebugProps.CellColor;

            //if (DebugProps.IsDrawPath && !isActor && TracePath != null) {
            //    if (TracePath.Contains(node))
            //        Gizmos.color = DebugProps.PathColor;
            //}

            //if (!isActor && !node.GetWalkable())
            //    Gizmos.color = DebugProps.CollisionColor;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(node.GetPosition(), 0.15f);
        }//foreach
    }//OnDrawGizmos


    public void GenerateGrid() {
        this.allNodes = new List<INode>(GetComponentsInChildren<INode>());
        Rect box = new Rect();
        box.xMin = float.PositiveInfinity;
        box.xMax = float.NegativeInfinity;
        box.yMin = float.PositiveInfinity;
        box.yMax = float.NegativeInfinity;

        this.GridBounds = this.computeWorldBounds(this.allNodes);

        int height, width = 0;
        height = Mathf.CeilToInt(this.WorldSize.x / TileHalfSize);
        width = Mathf.CeilToInt(this.WorldSize.y / TileHalfSize);

        height = height > 0 ? height : 1;
        width = width > 0 ? width : 1;

        this.Grid = new INode[height, width];

        for (int i = 0; i < this.allNodes.Count; i++) {
            INode tile = this.allNodes[i];
            tile.SetWalkable(true);
            int[] nodeCoord = this.CoordsFromWorld(tile.GetPosition());
            int x = nodeCoord[0];
            int y = nodeCoord[1];

            //FIXME: DO SOMETHING ABOUT THIS POSITOIN OVERWRITE STUFF!!
            if (this.Grid[x,y] != null) {
                string msg = string.Format("Grid node at {0}:{1} is taken! Will be overwritten!",
                    x, y);
                //Debug.LogWarning(msg);
            }

            tile.SetCoord(x, y);
            this.Grid[x,y] = tile;
        }//for

        //this.Grid = this.fillEmpty(this.Grid, height, width);

        //Set nodes' neighbours
        for (int i = 0; i < this.allNodes.Count; i++) {
            INode tile = this.allNodes[i];
            List<INode> neighbours = GetNeighbours(tile);
            tile.AddNeighbour(neighbours);
        }//for
    }//GenerateGrid


    protected INode[,] fillEmpty(INode[,] grid, int height, int width) {
        for (int h = 0; h < height; h++) {
            for (int w = 0; w < width; w++) {
                INode tile = grid[h,w];
                if (tile != null)
                    continue;

                //tile = new GameObject().AddComponent<INode>();
                tile.SetWalkable(false);
                tile.SetCoord(w, h);
                tile.SetPosition(this.GetWorldFromNode(w, h));
                tile.SetHeuristicCost(10);
                grid[h,w] = tile;
                //Debug.Log("Null Tile at [" + h + "," + w + "]");
            }//for w
        }//for h
        return grid;
    }


    protected Rect computeWorldBounds(List<INode> nodes) {
        Rect box = new Rect();
        box.xMin = float.PositiveInfinity;
        box.xMax = float.NegativeInfinity;
        box.yMin = float.PositiveInfinity;
        box.yMax = float.NegativeInfinity;

        for (int i = 0; i < nodes.Count; i++) {
            INode tile = nodes[i];
            Vector3 pos = tile.GetPosition();

            box.xMin = box.xMin <= pos.x ? box.xMin : pos.x;
            box.xMax = box.xMax >= pos.x ? box.xMax : pos.x;

            box.yMin = box.yMin <= pos.y ? box.yMin : pos.y;
            box.yMax = box.yMax >= pos.y ? box.yMax : pos.y;
        }//for
        box.width = box.width > 0 ? box.width : 1;
        box.height = box.height > 0 ? box.height : 1;
        return box;
    }//findWorldBounds


    private bool checkWalkable(Vector2 worldPoint, Vector2 size, LayerMask mask) {
        return !(Physics2D.OverlapBox(worldPoint,
                                    size,
                                    mask)
                                    );
    }


    /// <summary>
    ///  Return list of neighbours for the given Node.
    /// </summary>
    /// <param name="node">Node to get neighbours of.</param>
    public List<INode> GetNeighbours(INode node) {
        List<INode> neighbours = new List<INode>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if( x == 0 && y == 0) //is self node to find neighbours of
                    continue;

                int neighbourX = node.GetCoords()[0] + x;
                int neighbourY = node.GetCoords()[1] + y;
                bool isInsideX = neighbourX >= 0 && neighbourX < GridSizeX;
                bool isInsideY = neighbourY >= 0 && neighbourY < GridSizeY;

                if(isInsideX && isInsideY)
                    neighbours.Add(Grid[neighbourX, neighbourY]);
            }//for y
        }//for x

        return neighbours;
    }//GetNeighbours


    /// <summary>
    ///  Same as GetNeighbours(INode node), except using world position Vector.
    /// </summary>
    /// <param name="worldPos">Vecto Position in the world space (not grid coords).</param>
    public List<INode> GetNeighbours(Vector3 worldPos) {
        worldPos = this.transform.TransformPoint(worldPos);

        GridNode node = GetNodeFromWorld(worldPos) as GridNode;
        return GetNeighbours(node);
    }//GetNeighbours


    public INode GetNodeFromWorld(Vector3 worldPoint) {
        int[] coords = CoordsFromWorld(worldPoint);
        return Grid[coords[0], coords[1]];
    }//GetNodeFromWorld


    public Vector3 GetWorldFromNode(int x, int y) {
        float posX = -1;
        float posY = -1;
        float posZ = this.transform.position.z;

        INode node = GetNode(x, y);
        if (node == null)
            return new Vector3(posX, posY, posZ);

        return new Vector3(node.GetPosition().x, node.GetPosition().y, posZ);
    }//GetWorldFromNode


    public int[] CoordsFromWorld(Vector3 worldPoint) {
        int x = Mathf.FloorToInt((worldPoint.x - this.GridBounds.xMin) / Diameter);
        int y = Mathf.FloorToInt((worldPoint.y - this.GridBounds.yMin) / Diameter);
        x = Mathf.Abs(x);
        y = Mathf.Abs(y);
        return new int[] { x, y };
        //float percentX = (worldPoint.x + WorldSize.x / 2) / WorldSize.x;
        //float percentY = (worldPoint.y + WorldSize.y / 2) / WorldSize.y;
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        //int x = Mathf.RoundToInt((GridSizeX - 1) * percentX);
        //int y = Mathf.RoundToInt((GridSizeY - 1) * percentY);
        //return new int[] { x, y };
    }//GetWorldFromNode


    public void SetWalkableNode(Vector3 worldPoint, bool isWalkable) {
        INode node = GetNodeFromWorld(worldPoint);
        if (node == null) {
            #if UNITY_EDITOR
            Debug.LogWarning("Trying to set Walkable for node at " + worldPoint.ToString() + " but it is null!");
            #endif
            return;
        }

        node.SetWalkable(isWalkable);
    }//SetWalkableNode

    /* **************** GETTERS *********************** */

    public INode[,] Grid { get; private set; }


    /// <summary>
    ///  2x of the TileHalfSize value.
    /// </summary>
    public float Diameter { get { return this.TileHalfSize * 2; } }


    /// <summary>
    /// How many Tiles can feet in the WorldSize.x
    /// </summary>
    public int GridSizeX { get { return Mathf.RoundToInt(this.WorldSize.x / Diameter); } }
    

    /// <summary>
    /// How many Tiles can feet in the WorldSize.y
    /// </summary>
    public int GridSizeY { get { return Mathf.RoundToInt(this.WorldSize.y / Diameter); } }


    public Vector2 WorldSize => new Vector2(this.GridBounds.width, this.GridBounds.height);
    

    public INode GetNode(int x, int y) {
        if(Grid == null)
            Grid = new INode[GridSizeX, GridSizeY];

        bool isInHorizontal = (x >= 0 && x < Grid.Length);
        bool isInVertical = (x >= 0 && x < Grid.GetLength(0));
        if (!isInHorizontal || !isInVertical)
            return null;
        return Grid[x, y];
    }//GetNode
}//class GridSystem


[System.Serializable]
public class DebugGridProps {

    public bool IsDrawGrid = true;
    public bool IsDrawGridBounds = true;
    public Color ActorColor = Color.green;
    public bool IsDrawPath = true;
    public Color PathColor = Color.blue;
    public Color CellColor = Color.white;
    public Color CollisionColor = Color.red;


}//class DebugGridProps