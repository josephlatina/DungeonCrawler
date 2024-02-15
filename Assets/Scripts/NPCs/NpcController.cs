/*
 * NpcController.cs
 * Author: Jehdi Aizon,
 * Created: February 12, 2024
 * Description:
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;
using Yarn.Unity.Addons.SpeechBubbles;
using Random = System.Random;

public class NpcController : MonoBehaviour
{
    // Scriptable object of NPC information
    [Tooltip("The Scriptable Object of NPC. Create > Scriptable Objects > New NPC")]
    public NPC npc;

    private CharacterBubbleAnchor bubbleAnchor;

    public GameObject interactableIndicator;
    public bool showIndictator = false;

    public YarnProject project;

    [Space, Header("Dialogue Bubble Settings")]
    public bool randomizeDialogue = true;
    public float dialogueDetectRange = 1f;
    private CapsuleCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<CapsuleCollider2D>();
        collider.size = new Vector2(dialogueDetectRange, collider.size.y);
        bubbleAnchor = GetComponent<CharacterBubbleAnchor>();
        bubbleAnchor.CharacterName = npc.displayName;
    }

    // Update is called once per frame
    void Update()
    {
        collider.size = new Vector2(dialogueDetectRange, collider.size.y);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowIndicator(showIndictator);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (showIndictator)
            {
                ShowIndicator(showIndictator);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowIndicator(false);
        }
    }

    public YarnProject GetProject()
    {
        return project;
    }

    public string[] GetNodes()
    {
        return project.NodeNames;
    }

    /// <summary>
    /// Get the node name of the very first element in the list
    /// </summary>
    /// <returns>string of the node name</returns>
    public string GetNode()
    {
        return npc.nodeNames[0];
    }

    public string GetRandomNode()
    {
        if (npc.nodeNames.Count > 1)
        {
            var rand = new Random();

            // return a random integer from 0 to amount of nodes
            // e.g. rand.Next (10) to generate 0 - 9
            return npc.nodeNames[rand.Next(npc.nodeNames.Count)];
        }

        return npc.nodeNames[0];
    }

    void ShowIndicator(bool visibility = true)
    {
        if (showIndictator)
        {
            interactableIndicator.SetActive(visibility);
        }
    }
}