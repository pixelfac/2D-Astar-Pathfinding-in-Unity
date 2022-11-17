using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[ExecuteInEditMode]
public class Grid2D : MonoBehaviour
{
    [Header("Grid Parameters")]
    [Tooltip("Grid snapping to INT values.")]
    public bool IntegerGrid = false;
    [Tooltip("Is your grid moving / resizing at runtime?")]
    public bool DynamicGrid = false;
    [Tooltip("Accounts for colliders outside of the Tilemaps (any other non-tile Game Objects we want to collide with).")]
    public bool OffTileColliders = false;
    [Tooltip("Enables diagonal movement.")]
    public bool DiagonalMovement = false;
    [Tooltip("Keep Grid perfectly squared?")]
    public bool SquareGrid = true;
    public int GridSizeSquare;
    [Tooltip("Size of a non-square Grid.")]
    public Vector2 GridSizeRectangle;
    [Tooltip("Layer for OffTileColliders.")]
    public LayerMask CollisionLayer;
    [Tooltip("Global Grid offset.")]
    public Vector3 Offset;
    [Tooltip("Half of your Tilemap scale. Default scale is 1, so keep it at 0.5.")]
    public float nodeRadius = 0.5f;
    public Node2D[,] Grid;
    [Tooltip("Tilemap with obstacles.")]
    public Tilemap obstaclemap;
    public List<Node2D> path;


    private Vector3 worldBottomLeft;
    public Vector3 gridWorldSize;
    private Vector3 offset;
    private Vector2 minMaxX;
    private Vector2 minMaxY;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
  
    private void Awake() {
        path.Clear();
        if(IntegerGrid) AdjustGrid();
    }

    private void Start() {
        CreateGrid();
        AdjustGrid();
    }
 
    private void Update() {
        if(DynamicGrid){
            CreateGrid();
            AdjustGrid();
        }

        // #if UNITY_EDITOR
        //     CreateGrid();
        //     AdjustGrid();
        // #endif
    }



    private void OnValidate() {
        CreateGrid();
        AdjustGrid();
        // grid size should be an even number, to avoid misalignment with the tilemap
        if(!IntegerGrid) return;
        if(GridSizeSquare <= 0) GridSizeSquare = 2;
        GridSizeSquare = (int)(Mathf.Round(GridSizeSquare / 2f) * 2f);

        if(GridSizeRectangle.x <= 0) GridSizeRectangle.x = 2;
        if(GridSizeRectangle.y <= 0) GridSizeRectangle.y = 2;
        GridSizeRectangle.x = (int)(Mathf.RoundToInt(GridSizeRectangle.x / 2f) * 2f);
        GridSizeRectangle.y = (int)(Mathf.RoundToInt(GridSizeRectangle.y / 2f) * 2f);

    }

    private void AdjustGrid(){
        if(SquareGrid){
            gridWorldSize = new Vector3(GridSizeSquare,GridSizeSquare,0);
        }else{
            gridWorldSize = new Vector3(GridSizeRectangle.x,GridSizeRectangle.y,0);
        }

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        // adjust the offset based on the object's position
        offset = new Vector3(-transform.position.x + Offset.x,-transform.position.y + Offset.y);

        // only move the object by int amount (avoiding misalignments)
        if(!IntegerGrid) return;
        int newX = Mathf.RoundToInt(transform.position.x);
        int newY = Mathf.RoundToInt(transform.position.y);
        Vector3 newPos = new Vector3(newX,newY,0);
        transform.position = newPos;
    }

    void CreateGrid()
    {
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        Grid = new Node2D[gridSizeX, gridSizeY];

        // grid borders, so we don't get any NullRefs when target is outside of the grid
        float minX = worldBottomLeft.x;
        float maxX = worldBottomLeft.x + gridWorldSize.x;

        float minY = worldBottomLeft.y;
        float maxY = worldBottomLeft.y + gridWorldSize.y;

        minMaxX = new Vector2(minX,maxX);
        minMaxY = new Vector2(minY,maxY);


        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                Grid[x, y] = new Node2D(false, worldPoint, x, y);

                //raycast for obstacles
                RaycastHit2D hit;
                Vector3 screenP = Camera.main.WorldToScreenPoint(Grid[x,y].worldPosition);
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenP),Vector2.zero,11,CollisionLayer);

                if(hit.collider != null && OffTileColliders){
                    Grid[x, y].SetCollision(true);
                }else{
                    Grid[x, y].SetCollision(false);
                }

                if(obstaclemap == null) continue;
                if (obstaclemap.HasTile(obstaclemap.WorldToCell(Grid[x, y].worldPosition))){
                    Grid[x, y].SetObstacle(true);
                }else{
                    Grid[x, y].SetObstacle(false);
            }
            }
        }
    }

    //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
    public List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        //checks and adds top neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY + 1]);

        //checks and adds bottom neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY - 1]);

        //checks and adds right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY]);

        //checks and adds left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY]);



        if(DiagonalMovement){
            //checks and adds top right neighbor
            if (node.GridX + 1 >= 0 && node.GridX + 1< gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX + 1, node.GridY + 1]);

            //checks and adds bottom right neighbor
            if (node.GridX + 1>= 0 && node.GridX + 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX + 1, node.GridY - 1]);

            //checks and adds top left neighbor
            if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY + 1>= 0 && node.GridY + 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX - 1, node.GridY + 1]);

            //checks and adds bottom left neighbor
            if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY  - 1>= 0 && node.GridY  - 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX - 1, node.GridY - 1]);
        }
        



        return neighbors;
    }


    public Node2D NodeFromWorldPoint(Vector3 worldPosition)
    {
        if(Grid == null) return null;
        if(worldPosition.x < minMaxX.x || worldPosition.x > minMaxX.y || worldPosition.y < minMaxY.x || worldPosition.y > minMaxY.y) return null; 

        int x = Mathf.RoundToInt(worldPosition.x + offset.x + (gridSizeX / 2));
        int y = Mathf.RoundToInt(worldPosition.y + offset.y + (gridSizeY / 2));

        return Grid[x, y];
    }

    //Draws visual representation of grid
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (Grid != null)
        {
            foreach (Node2D n in Grid)
            {
                if (n.obstacle){

                    Gizmos.color = Color.red;
                }
                else{
                    Gizmos.color = Color.white;                   
                }

                if(n.collision) Gizmos.color = Color.blue;

                if (path != null && path.Contains(n))
                    Gizmos.color = Color.green;
                
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));

            }
        }
    }
}
