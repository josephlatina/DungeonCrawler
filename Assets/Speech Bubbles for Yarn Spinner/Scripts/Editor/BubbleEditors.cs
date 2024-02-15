
#if UNITY_EDITOR
namespace Yarn.Unity.Addons.SpeechBubbles
{
    using UnityEditor;
    using Yarn.Unity.Addons.SpeechBubbles.Editor;

    [CustomEditor(typeof(Bubble), true)]
    [CanEditMultipleObjects]
    public class BubbleEditor : NaughtyInspector {
    }

    [CustomEditor(typeof(Bubble.PresentationState), true)]
    [CanEditMultipleObjects]
    public class BubblePresentationStateEditor : NaughtyInspector {
    }

    [CustomEditor(typeof(Bubble.BubbleContent), true)]
    [CanEditMultipleObjects]
    public class BubbleContentEditor : NaughtyInspector {
    }

    [CustomEditor(typeof(BubbleDialogueView), true)]
    [CanEditMultipleObjects]
    public class BubbleDialogueViewEditor : NaughtyInspector {
    }

    [CustomEditor(typeof(CharacterBubbleAnchor), true)]
    [CanEditMultipleObjects]
    public class CharacterBubbleAnchorEditor : NaughtyInspector {
    }
}
#endif