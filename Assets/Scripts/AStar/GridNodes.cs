/*
 * GridNodes.cs
 * Author: Joseph Latina
 * Created: March 10, 2024
 * Description: Script to hold information regarding nodes in every grid position
 */

using UnityEngine;

public class GridNodes
{
    // size of the grid
    private int width;
    private int height;

    // 2D array of nodes
    private Node[,] gridNode;

    /// <summary>
    /// Constructor method
    /// </summary>
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        // for every position in the 2D array, we want to create a new node at that position
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x,y] = new Node(new Vector2Int(x,y));
            }
        }
    }

    /// <summary>
    /// Getter method for getting the grid node at a specified position
    /// </summary>
    public Node GetGridNode(int xPosition, int yPosition)
    {
        // check if position passed in is actually within bounds of the array
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }
}
