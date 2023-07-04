using Etra.NonGamerTutorialCreator;
using UnityEditor;

    [CustomEditor(typeof(EtraAnimationHolder))]
    public class EtraAnimationHolderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty animatedObjects = serializedObject.FindProperty("animatedObjects");
            EditorGUILayout.PropertyField(animatedObjects);

            SerializedProperty hideAllObjectsAtStart = serializedObject.FindProperty("hideAllObjectsAtStart");
            EditorGUILayout.PropertyField(hideAllObjectsAtStart);

            SerializedProperty sfxPlayer = serializedObject.FindProperty("sfxPlayer");
            EditorGUILayout.PropertyField(sfxPlayer);

            SerializedProperty duplicateStartIndex = serializedObject.FindProperty("duplicateStartIndex");
            EditorGUILayout.PropertyField(duplicateStartIndex);

            SerializedProperty duplicateEndIndex = serializedObject.FindProperty("duplicateEndIndex");
            EditorGUILayout.PropertyField(duplicateEndIndex);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("separateKeyboardControllerAnimations"));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            bool separated = serializedObject.FindProperty("separateKeyboardControllerAnimations").boolValue;

            if (!separated)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("standardAnimation"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("keyboardAnimation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("controllerAnimation"));
            }

            serializedObject.ApplyModifiedProperties();
        }



    }
