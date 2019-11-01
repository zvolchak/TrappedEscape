/*
 * This is Deprecated due to not being suitable for what I want and was based of this:
 * http://www.jgallant.com/nodal-pathfinding-in-unity-2d-with-a-in-non-grid-based-games/
 */
using System.Collections.Generic;
using UnityEngine;

public class WaypointsGenerator : MonoBehaviour{

    public Rect WorldBounds;
    public float Distance = 1f;
    public DebugGridProps DebugProps;

    public VirtualNode[,] Grid { get; private set; }
    private int gridHeight, gridWidth;

    public List<int[]> neighbours = new List<int[]>();


    public void Start() {
        for (int i = -2; i <= 2; i++) {
            for (int j = -2; j <= 2; j++) {
                if(Mathf.Abs(j) == 2 && Mathf.Abs(i) == 2)
                    continue;
                this.neighbours.Add(new int[] { i, j });
            }//for j
        }//for i

        this.generateNodes();
    }//Start


    protected void generateNodes() {
        int height = Mathf.CeilToInt(WorldBounds.height / Distance);
        int width = Mathf.CeilToInt(WorldBounds.width / Distance);
        Grid = new VirtualNode[height, width];
        for (int h = 0; h < height; h++) {
            int startIndex = h % 2;
            Vector3 worldPos = Vector3.zero;
            worldPos.y = WorldBounds.yMax - (h * Distance);

            for (int w = startIndex; w < width; w++) {
                if(w % 2 == startIndex)
                    continue;

                worldPos.x = WorldBounds.xMin + (w * Distance);
                VirtualNode node = new VirtualNode(true, worldPos, w, h);
                Grid[h, w] = node;
            }//for w
        }//for h
        this.gridHeight = height;
        this.gridWidth = width;

        this.generateNeighbours();
    }//generateNodes


    protected void generateNeighbours() {
        for (int h = 0; h < this.gridHeight; h++) {
            for (int w = 0; w < this.gridWidth; w++) {
                VirtualNode node = Grid[h,w];
                if(node == null)
                    continue;

                for (int i = 0; i < this.neighbours.Count; i++) {
                    int nX = node.GetCoords()[0] + this.neighbours[i][0];
                    int nY = node.GetCoords()[1] + this.neighbours[i][1];

                    bool isInsideY = nY >= 0 && nY < this.gridHeight;
                    bool isInsideX = nX >= 0 && nX < this.gridWidth;
                    if(isInsideY && isInsideX)
                        node.AddNeighbour(Grid[nY, nX]);
                }//for i
            }//for
        }//for h
    }//generateNeighbours


    public void OnDrawGizmos() {
        if (!DebugProps.IsDrawGrid)
            return;

        if (DebugProps.IsDrawGridBounds) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(WorldBounds.center, new Vector3(WorldBounds.size.x, WorldBounds.size.y, 0.2f));
            Gizmos.color = Color.gray;
        }

        if(Grid == null)
            return;

        foreach (VirtualNode node in Grid) {
            if (node == null)
                continue;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(node.GetPosition(), 0.15f);

            node.OnDrawGizmos();
        }//foreach
    }//OnDrawGizmos

}//class
