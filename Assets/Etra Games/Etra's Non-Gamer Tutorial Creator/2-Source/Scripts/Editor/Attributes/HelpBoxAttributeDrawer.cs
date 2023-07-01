using UnityEngine;
using UnityEditor;
using Etra.StarterAssets.Source.Editor;

namespace Etra.NonGamerTutorialCreator.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    internal class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight() =>
            _height + EditorGUIUtility.standardVerticalSpacing;

        float _height = 16f;

        public override void OnGUI(Rect position)
        {
            position = position
                .BorderBottom(EditorGUIUtility.standardVerticalSpacing);

            HelpBoxAttribute helpBoxAttribute = (HelpBoxAttribute)attribute;

            GUIContent content = new GUIContent(helpBoxAttribute.Message);

            _height = EditorStyles.helpBox.CalcHeight(content, position.width);

            EditorGUI.LabelField(position, content, EditorStyles.helpBox);
        }
    }
}
