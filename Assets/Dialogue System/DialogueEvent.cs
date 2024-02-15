using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
public class DialogueEvent : MonoBehaviour
{
    private DialogueRunner dialogueRunner;

    private void Start()
    {
        // Get the DialogueRunner component attached to the Dialogue System game object
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    void OnDialogueStart()
    {
        Debug.Log("BONK");
    }
}

