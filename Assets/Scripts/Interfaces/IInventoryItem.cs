/*
 * PlayerController.cs
 * Author: Jehdi Aizon
 * Created: January 18, 2024
 * Description: Interface for an item
 */
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }
    string Description { get; }
    int Price { get; }
}
