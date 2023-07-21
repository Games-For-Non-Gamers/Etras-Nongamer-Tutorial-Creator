using UnityEngine;
using UnityEditor;
using Etra.NonGamerTutorialCreator.Level;

namespace Etra.NonGamerTutorialCreator.Editor
{
    [CustomEditor(typeof(LevelChunk))]
    internal class LevelChunkInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LevelChunk script = (LevelChunk)target;

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Chunk Name");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.chunkName)), GUIContent.none);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.required)), GUIContent.none, GUILayout.Width(12f));
                        GUILayout.Label(new GUIContent("Is Required", "If true, the chunk will be undeletable in the level builder."));
                    }
                }

                script.icon = (Sprite)EditorGUILayout.ObjectField(script.icon, typeof(Sprite), false, GUILayout.Height(64f), GUILayout.Width(64f));
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.useSingle)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.recommended)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.orderPriority)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.teachingPriority)));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Abilities", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.testedAbilities)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.taughtAbilities)));

            EditorGUILayout.LabelField("Object", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LevelChunk.chunkObject)));

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            serializedObject.UpdateIfRequiredOrScript();
        }

    }

}
