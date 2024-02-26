namespace Yarn.Unity.Addons.SpeechBubbles
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Yarn.Unity;

    #nullable enable

    /// <summary>
    /// A <see cref="DialogueViewBase"/> subclass that handles the specific scenario of presenting, managing, and removing speech bubbles.
    /// </summary>
    public class BubbleDialogueView : DialogueViewBase
    {
        public enum ContentType {
            None,
            Line,
            Options
        }

        public ContentType CurrentContentType {
            get {
                if (currentContent.HasContent == false) {
                    return ContentType.None;
                } else if (currentContent.IsOptions) {
                    return ContentType.Options;
                } else {
                    return ContentType.Line;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this bubble dialogue view is
        /// currently displaying a bubble.
        /// </summary>
        public bool IsShowingBubble => currentBubble != null;

        /// <summary>
        /// The prefab to use when needing to instantiate a new bubble.
        /// </summary>
        /// <remarks>
        /// If the <see cref="CharacterBubbleAnchor.characterBubblePrefab"/> exists that will be used instead.
        /// </remarks>
        [SerializeField] private Bubble? bubblePrefab;
        
        /// <summary>
        /// The canvas upon which all bubbles will live
        /// </summary>
        [SerializeField] private Canvas? bubbleCanvas;

        [Header("Bubble Targets")]
        
        /// <summary>
        /// The name of the player character, this is used when a <see cref="Line"/> has no character value.
        /// </summary>
        [Tooltip("If a line has no character name, which " + nameof(CharacterBubbleAnchor) + " should the bubble be attached to?")]
        [SerializeField] private string PlayerName = "Player";

        [Header("Interactivity")]
        /// <summary>
        /// Should dialogue autoadvance onto the next line or not?
        /// </summary>
        [SerializeField] private bool autoAdvance = false;
        
        [Header("Timing")]
        
        /// <summary>
        /// How long the bubble takes to shrink down when it is dismissed.
        /// </summary>
        [SerializeField] private float bubbleHideDuration = 0.4f;
        
        /// <summary>
        /// How long the bubble takes to grow when being presented.
        /// </summary>
        [SerializeField] private float bubbleShowDuration = 0.4f;
        
        /// <summary>
        /// How long an already existing presented bubble takes to resize itself to present a new line
        /// </summary>
        [SerializeField] private float bubbleAdjustSizeDuration = 0.25f;

        /// <summary>
        /// How long will the text remain before advancing to the next line.
        /// </summary>
        /// <remarks>
        /// Only applicable when <see cref="autoAdvance"/> is set to true.
        /// </remarks>
        [ShowIf(nameof(autoAdvance))]
        [SerializeField] private float textRestDuration = 1f;

        
        [Header("Options")]

        /// <summary>
        /// When presenting options, can you wrap around from one end to the
        /// other?
        /// </summary>
        /// <remarks>
        /// Essentially if I have the first option selected, can I go to the
        /// last option directly from here?
        /// </remarks>
        [SerializeField] private bool AllowOptionWrapAround = true;
        
        /// <summary>
        /// Do we allow showing the unavailable options?
        /// </summary>
        /// <remarks>
        /// An option is unavailable if it has a condition, and that condition
        /// evaluates to false at run-time.
        /// </remarks>
        [SerializeField] private bool ShowUnavailableOptions = false;

        /// <summary>
        /// Do we allow selecting unavailable options?
        /// </summary>
        /// <remarks>
        /// This is very uncommon but sometimes might be what you want.
        /// Does nothing if <see cref="ShowUnavailableOptions"/> is set to false.
        /// </remarks>
        [ShowIf(nameof(ShowUnavailableOptions))]
        [SerializeField] private bool AllowSelectingUnavailableOption = false;

        [Header("Bubble Management")]
        [Tooltip("Should this dialogue view respond to commands like <<" + HideBubbleCommandName + ">>?")]
        [SerializeField] bool affectedByBubbleCommands = true;

        /// <summary>
        /// A dictionary of every character and the anchor target for that character.
        /// </summary>
        private readonly Dictionary<string, CharacterBubbleAnchor> characterTargets = new Dictionary<string, CharacterBubbleAnchor>();

        /// <summary>
        /// A dictionary mapping every character to their bubble.
        /// </summary>
        /// <remarks>
        /// Lets us reuse bubbles after finishing with one otherwise we'd have to instantiate a new bubble each time the conversation switch active speaker
        /// </remarks>
        private readonly Dictionary<string, Bubble> bubbles = new Dictionary<string, Bubble>();

        [Tooltip("When this view starts, pre-create a Bubble for every Character Anchor in the scene.")]
        [SerializeField] private bool preCreateBubblesOnStart = false;

        /// <summary>
        /// The current bubble being used to present a line/option
        /// </summary>
        private Bubble? currentBubble;

        /// <summary>
        /// The bubble content currently being presented in <see
        /// cref="currentBubble"/>.
        /// </summary>
        private Bubble.BubbleContent currentContent = Bubble.BubbleContent.None;

        /// <summary>
        /// The method to invoke when the user wants to select the current
        /// option.
        /// </summary>
        /// <remarks>
        /// This is non-null when <see cref="currentContent"/>'s <see
        /// cref="Bubble.BubbleContent.IsOptions"/> property is true. 
        /// </remarks>
        private Action<int>? currentOptionSelectionDelegate;

        /// <summary>
        /// A token used to control the interruption of animation effects
        /// </summary>
        private Effects.CoroutineInterruptToken currentStopToken = new Effects.CoroutineInterruptToken();

        private bool isPerformingBubbleAnimation = false;

        const string HideBubbleCommandName = "hide_bubbles";

        // we add in a special command <<hide_bubble>> you can use to dismiss a bubble
        // this is because we use a change in speaker as our means of dismissing bubbles
        // we can't tell if a command should also do this, hence this lets you control it
        [YarnCommand(HideBubbleCommandName)]
        public static void HideBubbles() {
            foreach (var bubbleDialogueView in FindObjectsOfType<BubbleDialogueView>()) {
                if (bubbleDialogueView.affectedByBubbleCommands) {
                    bubbleDialogueView.BubbleCommandHide();
                }
            }
        }

        void Start()
        {
            UpdateCharacterAnchors();
        }

        private void UpdateCharacterAnchors()
        {
            IEnumerable<CharacterBubbleAnchor> allAnchors;
#if UNITY_2023_1_OR_NEWER
            allAnchors = FindObjectsByType<CharacterBubbleAnchor>(FindObjectsSortMode.None);
#else
            allAnchors = FindObjectsOfType<CharacterBubbleAnchor>();
#endif

            // finding and associating every character target with their name
            foreach (var anchor in allAnchors)
            {
                characterTargets[anchor.CharacterName] = anchor;

                if (this.preCreateBubblesOnStart)
                {
                    CreateBubbleForAnchor(anchor);
                }
            }
        }

        public override void DialogueComplete()
        {
            // dialogue is finished, remove every bubble
            if (currentBubble != null)
            {
                StartCoroutine(DestroyAllBubbles());
                currentBubble = null;
            }
        }

        /// <summary>
        /// Goes through every bubble and removes them.
        /// </summary>
        /// <remarks>
        /// The current bubble is hidden first before being destroyed.
        /// </remarks>
        /// <returns>The coroutine that hides and then destroys the bubbles</returns>
        private IEnumerator DestroyAllBubbles()
        {
            // If we have a bubble up, dismiss it.
            if (currentBubble != null)
            {
                yield return currentBubble.DismissBubble(bubbleHideDuration, currentStopToken);
            }
            bubbles.Clear();
            this.currentContent = Bubble.BubbleContent.None;
        }

        /// <summary>
        /// Coroutine that runs when the <<hide_bubble>> command is reached.
        /// </summary>
        /// <remarks>
        /// Will hide the current bubble and null it out.
        /// </remarks>
        /// <returns>the coroutine itself</returns>
        public Coroutine? BubbleCommandHide()
        {
            if (currentBubble == null)
            {
                Debug.LogWarning("asked by a command to hide bubbles yet have no bubble to hide");
                return null;
            }

            return StartCoroutine(DestroyAllBubbles());
        }

        /// <inheritdoc />
        public override void DismissLine(System.Action onDismissalComplete)
        {
            onDismissalComplete();
        }
        
        /// <inheritdoc />
        public override void InterruptLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
        {
            // if we can stop any running animations do so
            if (currentStopToken.CanInterrupt)
            {
                currentStopToken.Interrupt();
                StopAllCoroutines();
            }
            onDialogueLineFinished();
        }

        /// <inheritdoc />
        public override void UserRequestedViewAdvancement()
        {
            // we need to be displaying *something* to advance
            if (currentBubble == null)
            {
                return;
            }

            // if we don't auto advance then we use the interrupt as our way of signalling we are done
            // in that case what we use the advancement signal for is to move onto the next line
            // and to also skip the current line if it is still being shown
            // this means we can only interrupt if something is being displayed
            // or we are set to not auto-advance
            var canInterrupt = currentStopToken.CanInterrupt || !autoAdvance;
            if (!canInterrupt)
            {
                return;
            }

            // If our current content is a collection of options, we don't want
            // to signal an interrupt. Instead, we just want to stop the
            // animations being run on that option.
            if (currentContent.IsOptions)
            {
                if (currentStopToken.CanInterrupt)
                {
                    currentStopToken.Interrupt();
                }
                return;
            }

            // otherwise we in the process of showing a line and want it interrupted
            requestInterrupt?.Invoke();
        }

        private IEnumerator PresentContentInBubble(Bubble bubbleForThisLine, Bubble.BubbleContent content)
        {
            this.isPerformingBubbleAnimation = true;
            if (bubbleForThisLine == currentBubble)
            {
                // We're using the same bubble as the last one. We'll tell it to
                // update to show the new content.
                yield return StartCoroutine(bubbleForThisLine.UpdateBubble(content, bubbleAdjustSizeDuration, currentStopToken));
            }
            else
            {
                // This bubble's speaker is different to the previous one
                // (or we don't have a bubble at all.) Dismiss the previous
                // one, if any, and present the current one.

                if (currentBubble != null)
                {
                    yield return StartCoroutine(
                        currentBubble.DismissBubble(bubbleHideDuration, currentStopToken)
                    );
                }
                currentBubble = bubbleForThisLine;
                yield return StartCoroutine(
                    bubbleForThisLine.PresentBubble(content, bubbleShowDuration, currentStopToken)
                );
            }

            isPerformingBubbleAnimation = false;
        }

        /// <inheritdoc />
        public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
        {
            this.currentContent = new Bubble.BubbleContent(dialogueLine, PlayerName);

            var bubbleForThisLine = GetOrCreateBubble(this.currentContent);

            if (bubbleForThisLine == null) {
                // We don't have a bubble we can use! That's an error.
                Debug.LogError($"No ${nameof(CharacterBubbleAnchor)} for the character {this.currentContent.BubbleOwnerName} was found. Cannot show a bubble for this character.");
                onDialogueLineFinished();
                return;
            }

            IEnumerator PerformBubble()
            {
                yield return StartCoroutine(PresentContentInBubble(bubbleForThisLine, this.currentContent));

                if (autoAdvance)
                {
                    yield return new WaitForSeconds(this.textRestDuration);
                    onDialogueLineFinished();
                }
            }

            StartCoroutine(PerformBubble());
        }

        /// <inheritdoc />
        public override void RunOptions(DialogueOption[] dialogueOptions, System.Action<int> onOptionSelected)
        {
            this.currentContent = new Bubble.BubbleContent(dialogueOptions, AllowOptionWrapAround, ShowUnavailableOptions, PlayerName);
            this.currentOptionSelectionDelegate = onOptionSelected;

            var bubbleForTheseOptions = GetOrCreateBubble(this.currentContent);

                if (bubbleForTheseOptions == null) {
                // We don't have a bubble we can use! That's an error.
                Debug.LogError($"Cannot show a bubble for the character {this.currentContent.BubbleOwnerName}. Hanging here!");
                return;
            }

            StartCoroutine(PresentContentInBubble(bubbleForTheseOptions, this.currentContent));
        }
        
        /// <summary>
        /// Called by a number of different pieces to change which is the currently displayed option.
        /// </summary>
        /// <param name="diff">the amount the <see cref="bundle.currentOptionIndex"/> should change by</param>
        public void ChangeOption(int diff)
        {
            // we need a bubble up or else we can't do anything
            if (currentBubble == null)
            {
                Debug.LogError("asked to change which option is shown yet no bubble exists");
                return;
            }
            if (currentContent.HasContent == false) {
                Debug.LogError("asked to change which option is shown but the current bubble has invalid content");
                return;
            }
            if (currentContent.IsOptions == false) {
                Debug.LogError("asked to change which option is shown but the current bubble is not showing options");
                return;
            }

            // it's likely users will mash next, next, next
            // so we will want to interrupt the options animating
            if (currentStopToken.CanInterrupt)
            {
                currentStopToken.Interrupt();
            }

            var previousIndex = this.currentContent.CurrentElementIndex;

            this.currentContent = this.currentContent.MoveToContent(diff);

            // It is possible with being asked to move through a lot of options
            // we are back where we started. If that's the case, don't re-present
            // the same option
            if (previousIndex == this.currentContent.CurrentElementIndex)
            {
                return;
            }

            StartCoroutine(PresentContentInBubble(this.currentBubble, this.currentContent));
        }
        
        /// <summary>
        /// Chooses the currently presented option and informs the dialogue runner of this.
        /// </summary>
        /// <remarks>
        /// Whatever the id of the <see cref="bundle.currentOptionIndex"/> is sent to the dialogue runners through the <see cref="bundle.onSelect"/> action.
        /// Bundle is then nullified to prevent any more selections.
        /// </remarks>
        public void SelectOption()
        {
            // we also need a bubble to be up and showing the option
            if (currentBubble == null)
            {
                Debug.LogWarning($"{nameof(SelectOption)} was called, but this {nameof(BubbleDialogueView)} is not currently presenting a bubble.");
                return;
            }

            if (currentContent.IsOptions == false) {
                // we need an option bundle, otherwise we can't do anything
                Debug.LogWarning($"{nameof(SelectOption)} was called, but this {nameof(BubbleDialogueView)} is not currently presenting options.");
                return;
            }

            if (currentOptionSelectionDelegate == null)
            {
                // We don't have a selection delegate. This can happen if the
                // user hits the 'confirm option' input multiple times - we've
                // already told the dialogue runner about our selection, so
                // ignore this input.
                return;
            }

            if (isPerformingBubbleAnimation) {
                // We're in the middle of animating the bubble. Don't accept
                // this input at the moment, because telling the dialogue runner
                // that we've selected an option could cause it to issue an
                // instruction to dismiss the bubble in the middle of this
                // animatiom (which would confuse us!)
                return;
            }

            if (currentContent.CurrentElement.IsAvailable == false && !AllowSelectingUnavailableOption)
            {
                // We're currently presenting options, but we can't select THIS
                // option because it's unavailable and we're configured to not
                // selecting it.
                return;
            }

            // it's likely users will mash next, next, next
            // so we will want to interrupt the options animating
            if (currentStopToken.CanInterrupt)
            {
                currentStopToken.Interrupt();
            }

            var action = this.currentOptionSelectionDelegate;
            int index = currentContent.CurrentElement.OptionID;
            this.currentOptionSelectionDelegate = null;

            action.Invoke(index);

            // We don't dismiss the bubble here, because we may want to re-use
            // this current bubble with the next piece of content. If we need to
            // dismiss or change the bubble, then that'll be up to the next
            // piece of incoming content.
        }
        
        /// <summary>
        /// Returns the specific bubble that is associated with that character.
        /// </summary>
        /// <remarks>
        /// If one doesn't exist, make a new one for that character and then
        /// return that. Where possible attempts to use the character specific
        /// prefab, otherwise will use the fallback one.
        /// </remarks>
        /// <param name="content">The bubble content to use. Its <see
        /// cref="Bubble.BubbleContent.BubbleOwnerName"/> must be non-null and
        /// must match a key inside of <see cref="characterTargets"/>.</param>
        /// <returns>The <see cref="Bubble"/> for the particular <paramref
        /// name="character"/></returns>
        private Bubble? GetOrCreateBubble(Bubble.BubbleContent content)
        {
            Bubble bubble;

            string? character = content.BubbleOwnerName;
            if (character == null) {
                // We don't have a name we can use to find who'll own this
                // bubble. We can't create the bubble!
                Debug.LogError($"Can't create a bubble because no character name was provided and the default character name isn't known!", this);
                return null;
            }

            // Try and get an existing bubble for the character.
            if (!bubbles.TryGetValue(character, out bubble))
            {
                // We don't have a bubble for this character. We'll need to
                // create a new one.


                if (characterTargets.TryGetValue(character, out var bubbleAnchor) == false)
                {
                    // We don't know about the character we're trying to create
                    // a bubble for! Try to update our list of characters - we
                    // might be trying to create a bubble for a character that
                    // has recently been added.
                    UpdateCharacterAnchors();

                    if (characterTargets.TryGetValue(character, out bubbleAnchor) == false)
                    {
                        // We still don't know! We can't create the bubble!
                        Debug.LogError($"Can't create a bubble for character {character} because no anchor named \"{character}\" could be found!", this);
                        return null;
                    }
                }

                return CreateBubbleForAnchor(bubbleAnchor);
            }

            // Return the bubble that we either created or fetched.
            return bubble;
        }

        private Bubble? CreateBubbleForAnchor(CharacterBubbleAnchor bubbleAnchor)
        {
            var character = bubbleAnchor.CharacterName;
            if (bubbleCanvas == null)
            {
                // We don't have a canvas we can add the bubble to!
                Debug.LogError($"Can't create a bubble for character {character} because {nameof(bubbleCanvas)} is null!", this);
                return null;
            }

            // If this character has their own specific bubble, use that.
            var prefab = bubbleAnchor.characterBubblePrefab;
            if (prefab == null)
            {
                // Otherwise, we will use the generic one.
                if (bubblePrefab == null) {
                    // We don't have a prefab we can use for this bubble!
                    Debug.LogError($"Can't create a bubble for character {character} because this character doesn't define their own bubble prefab, and the default bubble prefab was not set!", this);
                    return null;
                }
                prefab = bubblePrefab;
            }

            var bubble = GameObject.Instantiate<Bubble>(prefab, bubbleCanvas.transform);
            bubble.gameObject.name = $"{prefab.name} ({character})";
            bubbles[character] = bubble;
            // defaulting the bubble
            bubble.Target = bubbleAnchor;

            // Return the bubble that we either created or fetched.
            return bubble;
        }

        // this is copied from the lineview and later just use it directly from there, just currently it uses a different TMP type
        private static IEnumerator Typewriter(TMPro.TMP_Text text, float lettersPerSecond, System.Action onCharacterTyped, Effects.CoroutineInterruptToken? stopToken)
        {
            stopToken?.Start();

            // Start with everything invisible
            text.maxVisibleCharacters = 0;

            // Wait a single frame to let the text component process its
            // content, otherwise text.textInfo.characterCount won't be
            // accurate
            yield return null;

            // How many visible characters are present in the text?
            var characterCount = text.textInfo.characterCount;

            // Early out if letter speed is zero, text length is zero
            if (lettersPerSecond <= 0 || characterCount == 0)
            {
                // Show everything and return
                text.maxVisibleCharacters = characterCount;
                stopToken?.Complete();
                yield break;
            }

            // Convert 'letters per second' into its inverse
            float secondsPerLetter = 1.0f / lettersPerSecond;

            // If lettersPerSecond is larger than the average framerate, we
            // need to show more than one letter per frame, so simply
            // adding 1 letter every secondsPerLetter won't be good enough
            // (we'd cap out at 1 letter per frame, which could be slower
            // than the user requested.)
            //
            // Instead, we'll accumulate time every frame, and display as
            // many letters in that frame as we need to in order to achieve
            // the requested speed.
            var accumulator = Time.deltaTime;

            while (text.maxVisibleCharacters < characterCount)
            {
                if (stopToken?.WasInterrupted ?? false)
                {
                    yield break;
                }

                // We need to show as many letters as we have accumulated
                // time for.
                while (accumulator >= secondsPerLetter)
                {
                    text.maxVisibleCharacters += 1;
                    onCharacterTyped?.Invoke();
                    accumulator -= secondsPerLetter;
                }
                accumulator += Time.deltaTime;

                yield return null;
            }

            // We either finished displaying everything, or were
            // interrupted. Either way, display everything now.
            text.maxVisibleCharacters = characterCount;

            stopToken?.Complete();
        }
    }
}
