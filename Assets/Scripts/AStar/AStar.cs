/*
 * AStar.cs
 * Author: Joseph Latina
 * Created: March 10, 2024
 * Description: Script that holds logic for the AStar algorithm
 */

using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Gets the tilemap grid from the Room object passed in and builds a path between two points in a dungeon room
    /// Returns a Stack of steps which are the grid positions for each grid square the enemy will move between
    /// </summary>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Adjust grid positions by the room template's lower bounds
        startGridPosition -= (Vector3Int)room.templateLowerBounds; ;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // Initialize open list and closed hashset
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // Create instance of gridnodes for path finding by using this room's dimensions
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        // Retrieve start and target nodes from the grid nodes instance created and based on the passed in grid positions
        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        // Run the AStar algorithm to find the shortest path the enemy should take
        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);

        // If we've found a path, return the path (stack of steps) but translated to world positions
        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    /// <summary>
    /// Find the shortest path - returns the end Node if a path has been found, else returns null.
    /// </summary>
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // Add the passed in start node to the open list
        openNodeList.Add(startNode);

        // Loop while there are nodes in the open list
        while (openNodeList.Count > 0)
        {
            // Sort List (since nodes have been made comparable)
            openNodeList.Sort();

            // Retrieve the node in the open list with the lowest fCost and set it as the current node
            Node currentNode = openNodeList[0];
            // Remove the node from the list
            openNodeList.RemoveAt(0);

            // if the current node is the target node then we've found our path. Return this target node
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // add the current node (lowest fCost) to the closed list
            closedNodeHashSet.Add(currentNode);

            // evaluate fcost for each neighbour of the current node and populate open list with them
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
        }

        return null;

    }


    /// <summary>
    ///  Create a Stack<Vector3> of world positions containing the movement path by passing in the target Node and the room object
    /// </summary>
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        // Intiialize stack
        Stack<Vector3> movementPathStack = new Stack<Vector3>();
        // Set the target node
        Node nextNode = targetNode;

        // Get center point of each grid square in the room tilemap
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f; // don't need the z

        // if it hasn't reached the start node yet (since we are working backwards from the target node)
        while (nextNode != null)
        {
            // Convert grid position of node to world position
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x, nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

            // Set the world position to the middle of the grid cell
            worldPosition += cellMidPoint;

            // Add it to our movement stack
            movementPathStack.Push(worldPosition);

            // set the next node to be this current node's parent node
            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// <summary>
    /// Evaluate neighbour nodes
    /// </summary>
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // set the current node's grid position
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;
        // initialize a neighbouring node
        Node validNeighbourNode;

        // Loop through all directions of node
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                // check if this grid position is a valid neighbour
                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);

                // if valid, process it
                if (validNeighbourNode != null)
                {
                    // Calculate new gcost for neighbour
                    int newCostToNeighbour;

                    // Get the movement penalty
                    // Unwalkable paths have a value of 0. Default movement penalty is set in
                    // Settings and applies to other grid squares.
                    // int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                    // add on to current node's gCost with the distance between these two nodes
                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);

                    // Check if neighbour node is already in the open list
                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    // if neighbour node is not in the open list or it is but the new gCost we calculated is less than the neighbour node's gCost
                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        // Save the GCost and HCost we calculated to the neighbour node's
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        // Set this current node to be the neighbour node's parent
                        validNeighbourNode.parentNode = currentNode;

                        // if neighbour node is not already in open list, add it
                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns the distance int between nodeA and nodeB
    /// </summary>
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // 10 used instead of 1, and 14 is a pythagoras approximation SQRT(10*10 + 10*10) - to avoid using floats
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// <summary>
    /// Evaluate a neighbour node at its given position using the
    /// specified gridNodes, closedNodeHashSet, and instantiated room.  Returns null if the node isn't valid
    /// </summary>
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // If neighbour node position is outside the room template's bounds, return null
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        // Get neighbour node at the specified position
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // check for obstacle at that position
        // int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        // check for moveable obstacle at that position
        // int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];


        // if neighbour is an obstacle or neighbour is already in the closed list then skip. otherwise, return node
        if (closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }

    }
}
