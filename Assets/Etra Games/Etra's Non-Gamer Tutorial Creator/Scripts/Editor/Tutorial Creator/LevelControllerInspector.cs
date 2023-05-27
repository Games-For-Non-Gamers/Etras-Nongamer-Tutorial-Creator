using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level.Editor
{
    [CustomEditor(typeof(LevelController))]
    public class LevelControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (LevelController)target;

            if (script.isPreview)
                EditorGUILayout.HelpBox("This object is temporary. Create it by finishing the Tutorial Creator wizard.", MessageType.Info);

            using (new EditorGUI.DisabledGroupScope(script.isPreview))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelController.chunks)));

                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                if (GUILayout.Button("Edit", GUILayout.Height(32f))) //TODO: make this work
                { }
            }
        }
    }
}
