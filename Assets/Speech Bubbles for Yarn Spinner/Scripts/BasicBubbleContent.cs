namespace Yarn.Unity.Addons.SpeechBubbles
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;
    using Yarn.Unity;
    using System;
    using System.Collections;
    using UnityEngine.Events;
#nullable enable

    /// <summary>
    /// A basic <see cref="BubbleContentView"/> subclass.
    /// </summary>
    /// <remarks>
    /// This is intended to handle the more common styles of speech bubbles without needing too much work.
    /// As such it can be used as is or used as the starting point/inspiration of your own bubbles.
    /// A side-effect of this is it is more flexible than one would normally bother with.
    /// <list type="bullet">
    ///     <item>Can show left and right option indicators and click on them to advance options</item>
    ///     <item>Can show a button list of where in the option group the current option is, and click on these to change option</item>
    ///     <item>Can show a nameplate to show the current speaker</item>
    /// </list>
    /// </remarks>
    [ExecuteAlways]
    public sealed class BasicBubbleContent : BubbleContentView
    {
        [Header("Bubble Background")]

        [SerializeField] private Graphic? background;

        [Header("Options Elements")]

        /// <summary>
        /// The UI element that represents the left option indicator.
        /// </summary>
        /// <remarks>
        /// This is intended to be used to let the user both see that there are more options available to the left
        /// and to be able to click on this to make changing between options easier.
        /// </remarks>
        [SerializeField] private RectTransform? leftIndicator;
        /// <summary>
        /// The UI element that represents the right option indicator.
        /// </summary>
        /// <remarks>
        /// This is intended to be used to let the user both see that there are more options available to the right
        /// and to be able to click on this to make changing between options easier.
        /// </remarks>
        [SerializeField] private RectTransform? rightIndicator;

        /// <summary>
        /// The UI container element that represents a collection of small dots showing how many options are available overall.
        /// </summary>
        /// <remarks>
        /// The individual dots are also buttons that can be clicked on to move between options.
        /// </remarks>
        [UnityEngine.Serialization.FormerlySerializedAs("dotIndicator")]
        [SerializeField] private RectTransform? dotContainer;

        /// <summary>
        /// The prefab to use when more dots need to be instantiated than currently exist in the <see cref="dots"/> pool.
        /// </summary>
        /// <remarks>
        /// This must have a button component.
        /// </remarks>
        [SerializeField] private GameObject dotPrefab;


        [Header("Nameplate")]

        /// <summary>
        /// UI element that represents the current speakers name plate
        /// </summary>
        [SerializeField] private RectTransform? nameplate;

        /// <summary>
        /// The text label
        /// </summary>
        [SerializeField] private TMP_Text? nameplateField;

        [Header("Sizing")]
        [SerializeField] private Vector2 maxSize = new Vector2(600, 999999);

        /// <summary>
        /// The amount of additional padding to be added to each side to space out the text and left and right indicators.
        /// </summary>
        [SerializeField] private float sideIndicatorPadding = 0;
        /// <summary>
        /// The amount of additional padding to be added to the bottom to space out the text and dot indicators.
        /// </summary>
        [SerializeField] private float dotIndicatorPadding = 0;
        
        /// <summary>
        /// The amount of additional padding to be added to the top to space out the text and name plate.
        /// </summary>
        [SerializeField] private float namePlatePadding = 0;
        

        [Header("Animation")]
        [Min(0.001f)]
        [SerializeField] float typewriterLettersPerSecond = 20f;

        /// <summary>
        /// All the little dot buttons added to the <see cref="dotContainer"/>.
        /// </summary>
        private List<GameObject> dots = new List<GameObject>();
        
        /// <summary>
        /// An animation curve used to adjust the presentation of the <see cref="nameplate"/>.
        /// </summary>
        /// <remarks>
        /// The values from this are used to to drive the localScale of the name plate.
        /// Time for this curve comes from the dialogue view in the form of the <see cref="pointInTime"/> value.
        /// </remarks>
        [UnityEngine.Serialization.FormerlySerializedAs("showTimer")]
        [SerializeField] private AnimationCurve presentationCurve = AnimationCurve.Linear(0, 0, 0, 0);

        [Header("Bubble Data")]

        [SerializeField]
        private BasicCharacterBubbleData defaultBubbleData;

        [Header("Events")]

        [SerializeField]
        private UnityEvent? OnTextAppeared;

        void Start()
        {
            // we want no text by default
            textField.text = "";
        }

        void Update()
        {
            // Clamp maxsize to (0,0)
            maxSize = Vector2.Max(maxSize, Vector2.zero);

            // if there is a nameplate we want it to appear relative to the current point in time
            if (nameplate != null)
            {
                nameplate.transform.localScale = Vector3.one * (float)presentationCurve.Evaluate(PresentationFactor);
            }

            if (leftIndicator != null) {
                leftIndicator.pivot = new Vector2(0f, 0.5f);
            }
            if (rightIndicator != null) {
                rightIndicator.pivot = new Vector2(1f, 0.5f);
            }
        }

        /// <inheritdoc />
        public override Vector2 GetContentSize(Bubble.BubbleContent content)
        {
            float leftSize = 0;
            float rightSize = 0;
            float top = 0;
            float bottom = 0;

            if (content.HasContent == false)
            {
                // We don't have any text to show. Return an arbitrary size.
                return new Vector2(100, 50);
            }

            // If we have more than one text to show in this bubble, then we're
            // showing options, and need to add space for the arrows and dots
            bool isOption = content.IsOptions;

            string currentText = content.CurrentElement.Text;

            leftSize = leftIndicator == null ? 0 : leftIndicator.rect.size.x + sideIndicatorPadding;
            rightSize = rightIndicator == null ? 0 : rightIndicator.rect.size.x + sideIndicatorPadding;
            if (isOption)
            {
                bottom = dotContainer == null ? 0 : dotContainer.rect.size.y + dotIndicatorPadding;
            }
            top = 0; //nameplate == null ? 0 : nameplate.gameObject.activeSelf == false ? 0 : nameplate.rect.size.y + namePlatePadding;

            Vector2 size = textField.GetRenderedValues(
                currentText, 
                maxWidth: maxSize.x,
                maxHeight: maxSize.y,
                onlyVisibleCharacters: false
            );

            size.x += leftSize + rightSize;
            size.y += top + bottom;


            return size;
        }

        /// <inheritdoc />
        public override void SetOptionNavigation(int currentOptionIndex, int totalAvailableOptions, bool decrementNavigationAvailable, bool incrementNavigationAvailable)
        {
            SetSideIndicators(decrementNavigationAvailable, incrementNavigationAvailable);
            SetDots(currentOptionIndex, totalAvailableOptions);
        }

        /// <summary>
        /// Configures the little left and right indicators, enabling them or disabling them based on the parameters.
        /// </summary>
        /// <param name="leftActive">Should the left indicator be active? Comes directly from <see cref="SetOptionNavigation"/></param>
        /// <param name="rightActive">Should the right indicator be active? Comes directly from <see cref="SetOptionNavigation"/></param>
        private void SetSideIndicators(bool leftActive, bool rightActive)
        {
            if (leftIndicator != null)
            {
                leftIndicator.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = leftActive;
            }
            if (rightIndicator != null)
            {
                rightIndicator.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = rightActive;
            }
        }

        /// <summary>
        /// The function called by various objects to change the current option.
        /// </summary>
        /// <param name="increment">How many options are we moving through in this change?</param>
        public void click(int increment)
        {
            if (increment == 0)
            {
                return;
            }
            FindObjectOfType<BubbleDialogueView>().ChangeOption(increment);
        }

        /// <summary>
        /// Configures the number of dots to show when presenting options.
        /// </summary>
        /// <remarks>
        /// Also sets which dot represents the current option.
        /// </remarks>
        /// <param name="currentOptionIndex">Which of the little dots should represent the current option? Comes directly from <see cref="SetOptionNavigation"/></param>
        /// <param name="numberOfOptions">How many option dots should there be? Comes directly from <see cref="SetOptionNavigation"/></param>
        private void SetDots(int currentOptionIndex, int numberOfOptions)
        {
            // Clear any dots that have been deleted.
            var dotsToRemove = new List<GameObject>();
            foreach (var dot in dots) {
                if (dot == null) {
                    dotsToRemove.Add(dot!);
                }                
            }
            foreach (var dotToRemove in dotsToRemove) {
                dots.Remove(dotToRemove);
            }

            // if we don't currently have enough dots we will need to make more dots
            // this unlikely to happen too often
            if (dots.Count < numberOfOptions)
            {
                var moreDots = numberOfOptions - dots.Count;
                for (int j = 0; j < moreDots; j++)
                {
                    var dot = Instantiate(dotPrefab, dotContainer);
                    dot.name = dotPrefab.name;
                    dot.hideFlags = HideFlags.DontSave;
                    dots.Add(dot);
                }
            }

            int i = 0;
            foreach (var dot in dots)
            {
                
                // running through each dot we need to know if it is needed at all
                // as we might have more dots in the pool than options in the group
                if (i >= numberOfOptions)
                {
                    dot.SetActive(false);
                }
                else
                {
                    // assuming it is we need to hook it up to the click event
                    dot.SetActive(true);

                    var b = dot.GetComponent<UnityEngine.UI.Button>();
                    b.onClick.RemoveAllListeners();

                    var offset = i - currentOptionIndex;
                    b.onClick.AddListener(delegate { click(offset); });
                    b.interactable = true;

                    // finally if this dot also happens to be the dot for the current option we make it a different colour
                    if (i == currentOptionIndex)
                    {
                        b.targetGraphic.color = b.colors.normalColor;
                    }
                    else
                    {
                        b.targetGraphic.color = b.colors.disabledColor;
                    }
                }
                i++;
            }
        }

        /// <inheritdoc />
        public override void PrepareForLine()
        {
            if (leftIndicator != null)
            {
                leftIndicator.gameObject.SetActive(false);
            }
            if (rightIndicator != null)
            {
                rightIndicator.gameObject.SetActive(false);
            }
            if (dotContainer != null)
            {
                dotContainer.gameObject.SetActive(false);
            }
        }
        /// <inheritdoc />
        public override void PrepareForOptions()
        {
            if (leftIndicator != null)
            {
                leftIndicator.gameObject.SetActive(true);
                leftIndicator.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            if (rightIndicator != null)
            {
                rightIndicator.gameObject.SetActive(true);
                rightIndicator.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            if (dotContainer != null)
            {
                dotContainer.gameObject.SetActive(true);
                // need to make sure we can't click on any of the little dots during any transitions
                foreach (var dot in dots)
                {
                    if (dot != null) {
                        dot.GetComponent<UnityEngine.UI.Button>().interactable = false;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void SetCharacter(CharacterBubbleData? characterBubbleData)
        {
            BasicCharacterBubbleData? basicBubbleData;
            if (characterBubbleData != null)
            {
                if (characterBubbleData is BasicCharacterBubbleData data)
                {
                    basicBubbleData = data;
                }
                else
                {
                    Debug.LogWarning($"{nameof(BasicBubbleContent)} received {characterBubbleData.GetType()} as its character data, but it expects a {typeof(BasicCharacterBubbleData)}. Falling back to default bubble data.", this);
                    basicBubbleData = null;
                }
            }
            else
            {
                basicBubbleData = null;
            }

            if (basicBubbleData == null)
            {
                if (this.defaultBubbleData == null)
                {
                    Debug.LogError($"{typeof(BasicBubbleContent)} {this.name} has no default bubble data to use!", this);
                    return;
                }
                basicBubbleData = this.defaultBubbleData;
            }

            if (nameplate != null)
            {
                bool showNameplate = string.IsNullOrEmpty(basicBubbleData.characterDisplayName) == false;
                nameplate.gameObject.SetActive(showNameplate);

                if (nameplateField != null)
                {
                    nameplateField.text = basicBubbleData.characterDisplayName;
                    nameplateField.color = basicBubbleData.nameplateTextColor;
                }

                nameplate.GetComponentInChildren<Graphic>().color = basicBubbleData.nameplateBackgroundColor;
            }

            this.textField.color = basicBubbleData.textColor;
        }

        DrivenRectTransformTracker tracker;
        public void OnEnable()
        {
            tracker.Clear();

            if (this.leftIndicator != null) {
                tracker.Add(this, leftIndicator, DrivenTransformProperties.Pivot);
            }
            if (this.rightIndicator != null) {
                tracker.Add(this, rightIndicator, DrivenTransformProperties.Pivot);
            }
        }

        private void OnDisable() {
            tracker.Clear();
        }

        // Called when the Typewriter effect displays any amount of new text.
        private void OnCharacterTyped() {
            OnTextAppeared?.Invoke();
        }

        public override IEnumerator PresentContent(Bubble.BubbleContent content, Effects.CoroutineInterruptToken interruptToken)
        {
            this.textField.text = content.CurrentElement.Text;
            this.textField.maxVisibleCharacters = 0;

            yield return StartCoroutine(textField.Typewriter(this.typewriterLettersPerSecond, OnCharacterTyped, interruptToken));
        }

        public override IEnumerator DismissContent(Effects.CoroutineInterruptToken interruptToken)
        {
            this.textField.SetText(" ");
            this.textField.maxVisibleCharacters = 0;
            yield break;
            // throw new NotImplementedException();
        }

        internal override void SetContentWithoutAnimation(Bubble.BubbleContent content)
        {
            PrepareForContent(content);
            this.textField.text = content.CurrentElement.Text;
            this.textField.maxVisibleCharacters = int.MaxValue;
        }

        public override void PrepareForContent(Bubble.BubbleContent content)
        {
            if (content.IsLine) {
                this.PrepareForLine();
            } else if (content.IsOptions) {
                this.PrepareForOptions();
                this.SetOptionNavigation(content.CurrentElementIndex, content.Elements.Length, content.CanMovePrevious, content.CanMoveNext);
            }
            this.textField.ForceMeshUpdate(true);
            
        }
    }

    internal static class TMPExtensions
    {
        // Gets the tightest bounds for the text by updating the text and using GetRenderedValues
        // Note this uses sizeDelta for sizing so won't work when using anchors.
        // This is wayyyy more reliable than the actual GetRenderedValues because it won't return stupid values, as GetRenderedValues is prone to doing.
        public static Vector2 GetRenderedValues(this TMP_Text textMeshPro, string text, float maxWidth = Mathf.Infinity, float maxHeight = Mathf.Infinity, bool onlyVisibleCharacters = true)
        {
            if (string.IsNullOrEmpty(text)) return Vector2.zero;

            float horizontalMargin = textMeshPro.margin.x + textMeshPro.margin.z;
            float verticalMargin = textMeshPro.margin.y + textMeshPro.margin.w;

            maxWidth += horizontalMargin;
            maxHeight += verticalMargin;

            var originalRenderMode = textMeshPro.renderMode;
            var originalText = textMeshPro.text;
            var originalDeltaSize = textMeshPro.rectTransform.sizeDelta;
            
            textMeshPro.renderMode = TextRenderFlags.DontRender;
            textMeshPro.text = text;
            textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
            textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
            textMeshPro.ForceMeshUpdate(true);

            if (text.Length == 0) return Vector2.zero;
            // This doesn't work if the component is disabled - but it's better!
            // I'm not even sure this function works while disabled...

            // if(textMeshPro.textInfo.characterCount == 0) return Vector2.zero;

            // If width/height is Infinity/<0 renderedSize can be NaN. In that
            // case, use preferredValues
            var renderedSize = textMeshPro.GetRenderedValues(onlyVisibleCharacters);
            if (IsInvalidFloat(renderedSize.x) || IsInvalidFloat(renderedSize.y))
            {
                var preferredSize = textMeshPro.GetPreferredValues(text, maxWidth, maxHeight);
                // I've seen this come out as -4294967000.00 when the string has
                // only a zero-width space (\u200B) with onlyVisibleCharacters
                // true. In any case it makes no sense for the size to be < 0.
                preferredSize = new Vector2(Mathf.Max(preferredSize.x, 0), Mathf.Max(preferredSize.y, 0));
                if (IsInvalidFloat(renderedSize.x)) renderedSize.x = preferredSize.x;
                if (IsInvalidFloat(renderedSize.y)) renderedSize.y = preferredSize.y;
            }

            bool IsInvalidFloat(float f) { return float.IsNaN(f) || f == Mathf.Infinity || f < 0; }

            textMeshPro.renderMode = originalRenderMode;
            textMeshPro.text = originalText;
            textMeshPro.rectTransform.sizeDelta = originalDeltaSize;
            // textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalDeltaSize.x);
            // textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalDeltaSize.y);
            textMeshPro.ForceMeshUpdate(true);

            renderedSize.x += horizontalMargin;
            renderedSize.y += verticalMargin;

            return renderedSize;
        }

        internal static IEnumerator Typewriter(this TMPro.TMP_Text text, float lettersPerSecond, System.Action onCharacterTyped, Effects.CoroutineInterruptToken? stopToken)
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

        internal static void Typewriter(this TMPro.TMP_Text text, float lettersPerSecond, System.Action onCharacterTyped, Effects.CoroutineInterruptToken stopToken, System.Action onComplete) {
            text.StartCoroutine(RunTypewriter(text, lettersPerSecond, onCharacterTyped, stopToken, onComplete));

            IEnumerator RunTypewriter(TMP_Text text, float lettersPerSecond, Action onCharacterTyped, Effects.CoroutineInterruptToken stopToken, Action onComplete) {
                yield return text.Typewriter(lettersPerSecond, onCharacterTyped, stopToken);
                onComplete();
            }
        }
    }
}
