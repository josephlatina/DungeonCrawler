using UnityEngine;
using Yarn.Unity;

#nullable enable

public class ConversationTrigger : MonoBehaviour
{
    [SerializeField] DialogueRunner? dialogueRunner;

    [SerializeField] YarnProject project;

    [YarnNode(yarnProjectAttribute: nameof(project))]
    [SerializeField] string nodeName;

    public void StartConversation()
    {
        if (dialogueRunner == null)
        {
            return;
        }

        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(nodeName);
        }
    }
}