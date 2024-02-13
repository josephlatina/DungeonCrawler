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
using Yarn.Unity;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/New NPC", order = 0)]
public class NPC : ScriptableObject
{
    // list of node titles character can use
    public List<string> nodes = new List<string>();
    [SerializeField] private YarnProject project;

    [Tooltip("A list of nodes an NPC is able to speak"), YarnNode(nameof(project))]
    public List<string> nodeNames;
}