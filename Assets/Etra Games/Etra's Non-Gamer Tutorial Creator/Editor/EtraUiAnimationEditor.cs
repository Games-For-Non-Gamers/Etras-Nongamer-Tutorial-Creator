using Etra.NonGamerTutorialCreator;
using Etra.StarterAssets.Source.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


//[CustomEditor(typeof(EtraUiAnimation)), CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(EtraUiAnimation))]
    public class EtraUiAnimationEditor : PropertyDrawer
    {

    //Set block height, sadly I wasn't able to make this variable    
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {

        return base.GetPropertyHeight(prop, label) + 100;
    }

    
    

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        // we go one indent inside to make things nicer
     //   EditorGUI.indentLevel = 1;
        EditorGUI.BeginChangeCheck();

       // EditorGUI.LabelField(new Rect(pos.x, pos.y, 60, 20), "Speed :", EditorStyles.boldLabel);

        SerializedProperty tweenedObjectProperty = prop.FindPropertyRelative("tweenedObject");
        EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width, 20), tweenedObjectProperty);


        // Rect rect1 = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
        EditorGUI.LabelField(new Rect(pos.x, pos.y+25, 120, 20), "Tween Event:");
        EditorGUI.PropertyField(new Rect(pos.x+120, pos.y+25, pos.width-80, 20), prop.FindPropertyRelative("chosenTweenEvent"), GUIContent.none);



        EtraUiAnimation.AnimationEvents state = (EtraUiAnimation.AnimationEvents)prop.FindPropertyRelative("chosenTweenEvent").enumValueIndex;


        switch (state)
        {
            case EtraUiAnimation.AnimationEvents.MoveAndScale:
                SerializedProperty scaleAndMovePosition = prop.FindPropertyRelative("scaleAndMovePosition");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y+50, pos.width, 20), scaleAndMovePosition);

                SerializedProperty scaleAndMoveScale = prop.FindPropertyRelative("scaleAndMoveScale");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), scaleAndMoveScale);

                SerializedProperty scaleAndMoveTime = prop.FindPropertyRelative("scaleAndMoveTime");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 90, pos.width, 20), scaleAndMoveTime);
                break;

            case EtraUiAnimation.AnimationEvents.Move:
                SerializedProperty movePosition = prop.FindPropertyRelative("movePosition");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), movePosition);

                SerializedProperty moveTime = prop.FindPropertyRelative("moveTime");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), moveTime);
                break;

            case EtraUiAnimation.AnimationEvents.Scale:
                SerializedProperty scaleScale = prop.FindPropertyRelative("scaleScale");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), scaleScale);

                SerializedProperty scaleTime = prop.FindPropertyRelative("scaleTime");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), scaleTime);
                break;

            case EtraUiAnimation.AnimationEvents.Flash:
                SerializedProperty flashTimes = prop.FindPropertyRelative("flashTimes");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), flashTimes);

                SerializedProperty flashDelay = prop.FindPropertyRelative("flashDelay");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), flashDelay);
                break;

            case EtraUiAnimation.AnimationEvents.Wait:
                SerializedProperty secondsToWait = prop.FindPropertyRelative("secondsToWait");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), secondsToWait);
                break;

            case EtraUiAnimation.AnimationEvents.Fade:
                SerializedProperty opacity = prop.FindPropertyRelative("opacity");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), opacity);

                SerializedProperty fadeTime = prop.FindPropertyRelative("fadeTime");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), fadeTime);
                break;

            case EtraUiAnimation.AnimationEvents.UnlockAbility:
                break;

            case EtraUiAnimation.AnimationEvents.ToStartTransform:
                SerializedProperty toStartTime = prop.FindPropertyRelative("toStartTime");
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), toStartTime);
                break;

            default:
                break;
        }
        


    }


}


