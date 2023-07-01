using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Etra.NonGamerTutorialCreator.Level.LevelController;

namespace Etra.NonGamerTutorialCreator.Level.Editor
{
    [CustomEditor(typeof(LevelController))]
    public class LevelControllerInspector : UnityEditor.Editor
    {
        SerializedProperty bridgeOptions;
        LevelController script;
        private void OnEnable()
        {
            // hook up the serialized properties
            bridgeOptions = serializedObject.FindProperty(nameof(LevelController.bridgeOptions));
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            EditorGUILayout.PropertyField(bridgeOptions);
            serializedObject.ApplyModifiedProperties();
            LevelController script = (LevelController)target;



            if (script.isPreview)
                EditorGUILayout.HelpBox("This object is temporary. Create it by finishing the Tutorial Creator wizard.", MessageType.Info);

            using (new EditorGUI.DisabledGroupScope(script.isPreview))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelController.chunks)));

                GUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            if (GUILayout.Button("Apply Changes", GUILayout.Height(32f))) 
            { script.ResetAllChunksPositions(); }

        }
    }
}
