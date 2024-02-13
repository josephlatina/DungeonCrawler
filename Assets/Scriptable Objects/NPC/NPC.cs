/*
 * CreateNpc.cs
 * Author: Jehdi Aizon,
 * Created: February 12, 2024
 * Description:
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/New NPC", order = 0)]
public class NPC : ScriptableObject
{
    // list of node names character can use
    public List<string> nodes = new List<string>();
}