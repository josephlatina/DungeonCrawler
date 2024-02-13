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
using Yarn.Unity;
using Yarn.Unity.Addons.SpeechBubbles;

public class NpcController : MonoBehaviour
{
    public NPC npc;

    private CharacterBubbleAnchor bubbleAnchor;

    public GameObject interactableIndicator;
    public bool showIndictator = false;

    public YarnProject project;
    [YarnNode(nameof(project))] public string nodeName;

    // Set this to the bubble dialogue view you want to control
    public DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (showIndictator)
            {
                interactableIndicator.SetActive(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (showIndictator)
            {
                interactableIndicator.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactableIndicator.SetActive(false);
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

    public string GetNode()
    {
        return nodeName;
    }
}