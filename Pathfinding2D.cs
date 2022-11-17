using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Pathfinding2D : MonoBehaviour
{
    public bool ContiniousSeeking = false;
    public Transform TargetT;
    public Vector3 TargetPos;
    private Grid2D grid;
    private Node2D seekerNode, targetNode;
    public GameObject GridObj;

    private void Start()
    {
        //Instantiate grid
        grid = GridObj.GetComponent<Grid2D>();
    }

    public List<Vector3> GetPath(Transform targetT){
        FindPath(transform.position,TargetT.position);
        List<Node2D> path = grid.path;
        List<Vector3> finalPath = new List<Vector3>();

        foreach (var item in path)
        {
            finalPath.Add(item.worldPosition);
        }

        return finalPath;

    }

    public List<Vector3> GetPath(Vector3 targetPos){
        FindPath(transform.position,targetPos);
        List<Node2D> path = grid.path;
        List<Vector3> finalPath = new List<Vector3>();

        foreach (var item in path)
        {
            finalPath.Add(item.worldPosition);
        }

        return finalPath;
    }

    public void ClearPath(){
        grid.path.Clear();
    }

    private void Update() {
        if(TargetT != null) TargetPos = TargetT.position;

        if(ContiniousSeeking){
            if(TargetT == null) return;
            FindPath(transform.position,TargetPos);
        }
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if(grid.Grid == null || grid.Grid.Length == 0) return;
        //get player and target position in grid coords
        seekerNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        if(seekerNode == targetNode) return;

        if(seekerNode == null || targetNode == null) return;

        List<Node2D> openSet = new List<Node2D>();
        HashSet<Node2D> closedSet = new HashSet<Node2D>();
        openSet.Add(seekerNode);
        
        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            Node2D node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //If target found, retrace path
            if (node == targetNode)
            {
                if(targetNode.collision || targetNode.obstacle){
                    RetracePath(seekerNode, targetNode.parent);
                }else{
                    RetracePath(seekerNode, targetNode);
                }
                return;
            }
            
            //adds neighbor nodes to openSet
            foreach (Node2D neighbour in grid.GetNeighbors(node))
            {
                // neighbour.obstacle ||
                if (neighbour.obstacle || closedSet.Contains(neighbour) || neighbour.collision)
                {
                    if(neighbour != targetNode){
                        continue;

                    }
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    //reverses calculated path so first node is closest to seeker
    private void RetracePath(Node2D startNode, Node2D endNode)
    {
        List<Node2D> path = new List<Node2D>();
        Node2D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    //gets distance between 2 nodes for calculating cost
    private int GetDistance(Node2D nodeA, Node2D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);

        // if (dstX > dstY)
        //     return 14 * dstY + 10 * (dstX - dstY);
        // return 14 * dstX + 10 * (dstY - dstX);
    }
}