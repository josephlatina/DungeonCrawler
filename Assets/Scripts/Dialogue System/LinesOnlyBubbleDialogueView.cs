/*
 * LinesOnlyBubbleDialogueView.cs
 * Author: Jehdi Aizon,
 * Created: February 14, 2024
 * Description: a sub class of BubbleDialogueView to override Bubble Dialogue view
 * with the default options view instead of showing bubble view
 */

using Yarn.Unity;
using Yarn.Unity.Addons.SpeechBubbles;

public class LinesOnlyBubbleDialogueView : BubbleDialogueView {
    public override void RunOptions(DialogueOption[] dialogueOptions, System.Action<int> onOptionSelected) {
        // Do nothing - the Options List View will handle the options, not the bubble
        
    }
}