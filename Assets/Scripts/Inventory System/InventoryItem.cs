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
    public GameObject gameObject;

    // Add actions that are common in all items
    // but treated differently based on type to an item below 
    public abstract void Add();
    // TODO: ADD MORE

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true; // Allow for string wrapping in the inspector
        description = EditorGUILayout.TextArea(description, style);
    }
}