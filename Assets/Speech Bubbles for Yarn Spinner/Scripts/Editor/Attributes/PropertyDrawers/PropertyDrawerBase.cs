/*

MIT License

Copyright (c) 2017 Denis Rizov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/


using UnityEditor;
using UnityEngine;

namespace Yarn.Unity.Addons.SpeechBubbles.Editor
{
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Check if visible
            bool visible = PropertyUtility.IsVisible(property);
            if (!visible)
            {
                return;
            }

#if YS_EDITORATTRS_UNUSED
            // Validate
            ValidatorAttribute[] validatorAttributes = PropertyUtility.GetAttributes<ValidatorAttribute>(property);
            foreach (var validatorAttribute in validatorAttributes)
            {
                validatorAttribute.GetValidator().ValidateProperty(property);
            }
#endif

            // Check if enabled and draw
            EditorGUI.BeginChangeCheck();
            bool enabled = PropertyUtility.IsEnabled(property);

            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                OnGUI_Internal(rect, property, PropertyUtility.GetLabel(property));
            }

            // Call OnValueChanged callbacks
            if (EditorGUI.EndChangeCheck())
            {
                PropertyUtility.CallOnValueChangedCallbacks(property);
            }
        }

        protected abstract void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label);

        sealed override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool visible = PropertyUtility.IsVisible(property);
            if (!visible)
            {
                return 0.0f;
            }

            return GetPropertyHeight_Internal(property, label);
        }

        protected virtual float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        protected float GetPropertyHeight(SerializedProperty property)
        {
#if YS_EDITORATTRS_UNUSED
            SpecialCaseDrawerAttribute specialCaseAttribute = PropertyUtility.GetAttribute<SpecialCaseDrawerAttribute>(property);
            if (specialCaseAttribute != null)
            {
                return specialCaseAttribute.GetDrawer().GetPropertyHeight(property);
            }
#endif

            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        public virtual float GetHelpBoxHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.0f;
        }

        public void DrawDefaultPropertyAndHelpBox(Rect rect, SerializedProperty property, string message, MessageType messageType)
        {
            float indentLength = NaughtyEditorGUI.GetIndentLength(rect);
            Rect helpBoxRect = new Rect(
                rect.x + indentLength,
                rect.y,
                rect.width - indentLength,
                GetHelpBoxHeight());

            NaughtyEditorGUI.HelpBox(helpBoxRect, message, MessageType.Warning, context: property.serializedObject.targetObject);

            Rect propertyRect = new Rect(
                rect.x,
                rect.y + GetHelpBoxHeight(),
                rect.width,
                GetPropertyHeight(property));

            EditorGUI.PropertyField(propertyRect, property, true);
        }
    }
}
