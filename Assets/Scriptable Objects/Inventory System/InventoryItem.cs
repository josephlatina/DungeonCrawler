/*
 * InventoryItem.cs
 * Author: Jehdi Aizon,
 * Created: January 21, 2024
 * Description: methods created here are used in the consumable item or weapaon item script
 */

using UnityEditor;
using UnityEngine;

public abstract class InventoryItem : ScriptableObject
{
    public string itemName;
    [TextArea(1, 5)] public string description;
    public int price;
    public int quantity = 1;
    [HideInInspector] public GameObject gameObject;

    public Sprite itemSprite;

    // Add actions that are common in all items
    // but treated differently based on type to an item below 
    // TODO: ADD MORE
    
    /// <summary>
    /// Drop current item into given position
    /// </summary>
    /// <param name="dropPosition">position where to drop this current item</param>
    public void DropItemAt(Vector2 dropPosition)
    {
        gameObject.transform.position = dropPosition;
        gameObject.SetActive(true);
    }
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true; // Allow for string wrapping in the inspector
        description = EditorGUILayout.TextArea(description, style);
    }
}