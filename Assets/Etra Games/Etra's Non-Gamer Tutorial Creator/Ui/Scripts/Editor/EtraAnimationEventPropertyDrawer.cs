using UnityEditor;
using UnityEngine;
using Etra.NonGamerTutorialCreator;

namespace Etra.NonGamerTutorialCreator
{
    //[CustomEditor(typeof(EtraUiAnimation)), CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(EtraAnimationEvent))]
    public class EtraAnimationEventPropertyDrawer : PropertyDrawer
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
            EditorGUI.LabelField(new Rect(pos.x, pos.y + 25, 120, 20), "Tween Event:");
            EditorGUI.PropertyField(new Rect(pos.x + 120, pos.y + 25, pos.width - 80, 20), prop.FindPropertyRelative("chosenEvent"), GUIContent.none);



            EtraAnimationEvent.AnimationEvents state = (EtraAnimationEvent.AnimationEvents)prop.FindPropertyRelative("chosenEvent").enumValueIndex;


            switch (state)
            {
                case EtraAnimationEvent.AnimationEvents.MoveAndScale:
                    SerializedProperty scaleAndMovePosition = prop.FindPropertyRelative("scaleAndMovePosition");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), scaleAndMovePosition);

                    SerializedProperty scaleAndMoveScale = prop.FindPropertyRelative("scaleAndMoveScale");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), scaleAndMoveScale);

                    SerializedProperty scaleAndMoveTime = prop.FindPropertyRelative("scaleAndMoveTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 90, pos.width, 20), scaleAndMoveTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.Move:
                    SerializedProperty movePosition = prop.FindPropertyRelative("movePosition");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), movePosition);

                    SerializedProperty moveTime = prop.FindPropertyRelative("moveTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), moveTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.Scale:
                    SerializedProperty scaleScale = prop.FindPropertyRelative("scaleScale");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), scaleScale);

                    SerializedProperty scaleTime = prop.FindPropertyRelative("scaleTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), scaleTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.Wait:
                    SerializedProperty secondsToWait = prop.FindPropertyRelative("secondsToWait");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), secondsToWait);
                    break;

                case EtraAnimationEvent.AnimationEvents.ToStartTransform:
                    SerializedProperty toStartTime = prop.FindPropertyRelative("toStartTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), toStartTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.Flash:
                    SerializedProperty flashTimes = prop.FindPropertyRelative("flashTimes");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), flashTimes);

                    SerializedProperty flashDelay = prop.FindPropertyRelative("flashDelay");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), flashDelay);
                    break;

                case EtraAnimationEvent.AnimationEvents.FadeIn:
                    SerializedProperty opacity = prop.FindPropertyRelative("fadeInOpacity");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), opacity);

                    SerializedProperty fadeTime = prop.FindPropertyRelative("fadeInTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), fadeTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.FadeOut:
                    SerializedProperty fadeOutTime = prop.FindPropertyRelative("fadeOutTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), fadeOutTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.PlaySfx:
                    SerializedProperty sfxName = prop.FindPropertyRelative("sfxName");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), sfxName);
                    break;

                case EtraAnimationEvent.AnimationEvents.Rotate:
                    SerializedProperty rotation = prop.FindPropertyRelative("rotation");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), rotation);
                    SerializedProperty rotateTime = prop.FindPropertyRelative("rotateTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), rotateTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.MoveToGameObject:
                    SerializedProperty moveToObjectGameobject = prop.FindPropertyRelative("moveToObjectGameobject");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), moveToObjectGameobject);
                    SerializedProperty moveToObjectTime = prop.FindPropertyRelative("moveToObjectTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), moveToObjectTime);
                    SerializedProperty addedPosition = prop.FindPropertyRelative("addedPosition");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 90, pos.width, 20), addedPosition);
                    break;

                case EtraAnimationEvent.AnimationEvents.RotateToGameObject:
                    SerializedProperty rotToObjectGameobject = prop.FindPropertyRelative("rotToObjectGameobject");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), rotToObjectGameobject);
                    SerializedProperty rotToObjectTime = prop.FindPropertyRelative("rotToObjectTime");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), rotToObjectTime);
                    break;

                case EtraAnimationEvent.AnimationEvents.RunEtraAnimationActivatedScript:
                    SerializedProperty etraAnimationActivatedScript = prop.FindPropertyRelative("etraAnimationActivatedScript");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 50, pos.width, 20), etraAnimationActivatedScript);
                    SerializedProperty passedString = prop.FindPropertyRelative("passedString");
                    EditorGUI.PropertyField(new Rect(pos.x, pos.y + 70, pos.width, 20), passedString);
                    break;

                default:
                    break;
            }



        }


    }

}

