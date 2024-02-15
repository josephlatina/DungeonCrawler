namespace Yarn.Unity.Addons.SpeechBubbles.Sample
{
    #if USE_INPUTSYSTEM
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    #nullable enable

    /// <summary>
    /// A basic input handling class that allows the user to select and change
    /// options
    /// </summary>
    /// <remarks>
    /// Not intended to be used or subclassed, exists for the samples only, your
    /// games input system should perform these roles.
    /// </remarks>
    public sealed class SampleBubbleOptionInput : MonoBehaviour, SampleControls.IDialogueActions
    {
        [SerializeField] InputActionReference? advanceDialogueAction;
        [SerializeField] InputActionReference? nextOptionAction;
        [SerializeField] InputActionReference? previousOptionAction;

        /// <summary>
        /// The dialogue view responsible for managing the bubbles.
        /// </summary>
        [SerializeField] private BubbleDialogueView bubbleDialogueView;

        private void UpdateActionCallbacks(bool enable)
        {
            var arr = new (InputActionReference? Action, Action<InputAction.CallbackContext> Callback)[] {
                (nextOptionAction, OnNextOption),
                (previousOptionAction, OnPreviousOption),
                (advanceDialogueAction, OnAdvanceDialogue),
            };
            foreach (var entry in arr)
            {
                if (entry.Action == null)
                {
                    continue;
                }
                if (enable)
                {
                    entry.Action.action.performed += entry.Callback;
                }
                else
                {
                    entry.Action.action.performed -= entry.Callback;
                }
            }
        }

        private void OnEnable()
        {
            UpdateActionCallbacks(enable: true);
        }

        private void OnDisable()
        {
            UpdateActionCallbacks(enable: false);
        }

        public void OnNextOption(InputAction.CallbackContext context)
        {
            if (context.performed && bubbleDialogueView.CurrentContentType == BubbleDialogueView.ContentType.Options)
            {
                bubbleDialogueView.ChangeOption(1);
            }
        }
        public void OnPreviousOption(InputAction.CallbackContext context)
        {
            if (context.performed && bubbleDialogueView.CurrentContentType == BubbleDialogueView.ContentType.Options)
            {
                bubbleDialogueView.ChangeOption(-1);
            }
        }

        public void OnAdvanceDialogue(InputAction.CallbackContext context)
        {
            if (bubbleDialogueView.IsShowingBubble == false) {
                return;
            }
            
            switch (bubbleDialogueView.CurrentContentType)
            {
                case BubbleDialogueView.ContentType.Options:
                    bubbleDialogueView.SelectOption();
                    break;
                case BubbleDialogueView.ContentType.Line:
                    bubbleDialogueView.UserRequestedViewAdvancement();
                    break;
            }
        }
    }
    #endif
}
