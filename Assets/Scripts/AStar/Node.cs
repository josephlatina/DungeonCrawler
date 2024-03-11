/*
 * Node.cs
 * Author: Joseph Latina
 * Created: March 10, 2024
 * Description: Script to make nodes sortable for Astar algorithm
 */

using System;
using UnityEngine;

public class Node: IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; // distance from starting node
    public int hCost = 0; // distance from finishing node
    public Node parentNode;

    /// <summary>
    /// Constructor method
    /// </summary>
    public Node(Vector2Int gridPosition)
    {
        // set the grid position of this node to the past grid position
        this.gridPosition = gridPosition;
        // initialize the parent node
        parentNode = null;
    }

    /// <summary>
    /// Getter method for getting the FCost value of this node
    /// </summary>
    public int FCost
    {
        get 
        {
            return gCost + hCost;
        }
    }

    /// <summary>
    /// Returns > 0 if this instance Fcost is greater than nodeToCompare.FCost
    /// Returns < 0 if this instance Fcost is less than nodeToCompare.FCost
    /// Returns 0 if values are the same
    /// </summary>
    public int CompareTo(Node nodeToCompare) 
    {
        // Compare the FCost int values
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        // If they are the same, then compare using the hCost int values
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return compare;
    }
}
