using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

using UnityEngine;

public class AStarPathfinder: PathfinderInterface
{
    private string pathfinderName = "A*";

    private Node nodeStart;
    private Node nodeEnd;
    Node nodeCurrent;

    private HashSet<Node> openNodeSet;
    private HashSet<Node> closedNodeSet;

    public PathfinderStatus Status { get; private set; }
    public bool IsDone { get; private set; }

    public AStarPathfinder(Node nodeStart, Node nodeEnd)
    {
        this.nodeCurrent = null;
        this.nodeStart = nodeStart;
        this.nodeEnd = nodeEnd;

        openNodeSet = new HashSet<Node>();
        closedNodeSet = new HashSet<Node>();

        // Calculate startnode costs
        int h = PathfinderManager.Cost(nodeStart.Position, nodeEnd.Position);
        nodeStart.SetCost(0, h, h);

        // Start from start node.
        openNodeSet.Add(this.nodeStart);

        IsDone = false;
    }

    public override string ToString()
    {
        return pathfinderName;
    }

    public void RunIteration()
    {
        if (openNodeSet.Count == 0)
        {   
            IsDone = true;
            Debug.Log("Failed to find optimal path.");
            Status = PathfinderStatus.NO_PATH_FOUND;
            return;
        }

        // Find node with best f cost.
        nodeCurrent = openNodeSet.Min();
        openNodeSet.Remove(nodeCurrent);

        // Found end
        if (nodeCurrent == nodeEnd)
        {
            Debug.Log("Found optimal path.");
            Status = PathfinderStatus.PATH_FOUND;
            IsDone = true;
            return;
        }

        // Close the best node for potential best path.
        CloseNode(nodeCurrent);

        // Open neighbours for best node.
        OpenNeighbours(nodeCurrent);
    }

    private void OpenNeighbours(Node node)
    {
        foreach (Node neighbour in node.GetNeighbours())
        {
            if (neighbour.state != NodeState.OBSTACLE &&
                neighbour.state != NodeState.CLOSED &&
                neighbour.state != NodeState.START
                )
            {
                // Criteria to choose only open better paths by minimizing g.
                int tentativeGScore = node.GCost + PathfinderManager.Cost(node.Position, neighbour.Position);
                if(tentativeGScore < neighbour.GCost || neighbour.state != NodeState.OPEN)
                {
                    neighbour.CameFromNode = node;
                    
                    int h = PathfinderManager.Cost(neighbour.Position, nodeEnd.Position);
                    int f = tentativeGScore + h;
                    neighbour.SetCost(tentativeGScore, h, f);

                    if (neighbour.state != NodeState.END)
                    {
                        neighbour.state = NodeState.OPEN;
                        neighbour.UpdateAppearance();
                    }
                    openNodeSet.Add(neighbour);
                }
            }
        }
    }

    private void CloseNode(Node node)
    {
        if (node.state != NodeState.START)
        {
            closedNodeSet.Add(node);
            node.state = NodeState.CLOSED;
            node.UpdateAppearance();
        }
    }

    public HashSet<Node> OpenNodeSet => openNodeSet;
    public HashSet<Node> ClosedNodeSet => closedNodeSet;
    public int OpenNodesTotal => openNodeSet.Count;
    public int ClosedNodesTotal => closedNodeSet.Count;


    public int Distance => nodeCurrent.GCost;
}
