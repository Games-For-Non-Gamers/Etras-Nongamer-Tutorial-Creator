using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DialogueEntryNew
{
    [SerializeField] private string _speakerName;
    [SerializeField,TextArea] private string _dialogueLine;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private bool _useAudioClipLengthAsWaitTime;
    [SerializeField] private float _waitTime;
    [SerializeField] private DialogueEvents _dialogueEvents = DialogueEvents.SetSpeaker;
    [SerializeField] private float _textSpeed;

    public string SpeakerName { get => _speakerName; }
    public string DialogueLine { get => _dialogueLine; }
    public AudioClip AudioClip { get => _audioClip; }
    public float WaitTime { get => _waitTime; }
    public DialogueEvents DialogueEvents { get => _dialogueEvents; }
    public bool UseAudioClipLengthAsWaitTime { get => _useAudioClipLengthAsWaitTime; }
    public float TextSpeed { get => _textSpeed; }

    //MoveOrRotateObject
    public GameObject savedObject;//target object at times
    public Vector3 targetVector3;
    public MonoBehaviour monoBehaviour;

    public UnityEvent unityEvent;

}

public enum DialogueEvents
{
    SetSpeaker,
    NewLine,
    AddLine,
    SetSpeakerAndNewLine,
    Wait,
    TextSpeed,
    MoveObject,
    RotateObject,
    MovePlayer,
    RotatePlayerCam,
    LockPlayer,
    UnlockPlayer,
    EnableMonoBehavior,
    EnableTrigger,
    RunEvent
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DialogueEntryNew))]
public class DialogueEntryPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        DialogueEvents state = (DialogueEvents)prop.FindPropertyRelative("_dialogueEvents").enumValueIndex;
        switch (state)
        {
            case DialogueEvents.SetSpeaker:
                return base.GetPropertyHeight(prop, label) + 30;
            case DialogueEvents.NewLine:
                return base.GetPropertyHeight(prop, label) + 170;
            case DialogueEvents.AddLine:
                return base.GetPropertyHeight(prop, label) + 150;
            case DialogueEvents.SetSpeakerAndNewLine:
                return base.GetPropertyHeight(prop, label) + 190;
            case DialogueEvents.Wait:
                return base.GetPropertyHeight(prop, label) + 30;
            case DialogueEvents.TextSpeed:
                return base.GetPropertyHeight(prop, label) + 30;
            case DialogueEvents.MoveObject:
                return base.GetPropertyHeight(prop, label) + 70;
            case DialogueEvents.RotateObject:
                return base.GetPropertyHeight(prop, label) + 70;
            case DialogueEvents.MovePlayer:
                return base.GetPropertyHeight(prop, label) + 70;
            case DialogueEvents.RotatePlayerCam:
                return base.GetPropertyHeight(prop, label) + 70;
            case DialogueEvents.LockPlayer:
                return base.GetPropertyHeight(prop, label) + 20;
            case DialogueEvents.UnlockPlayer:
                return base.GetPropertyHeight(prop, label) + 20;
            case DialogueEvents.EnableMonoBehavior:
                return base.GetPropertyHeight(prop, label) + 30;
            case DialogueEvents.EnableTrigger:
                return base.GetPropertyHeight(prop, label) + 30;
            case DialogueEvents.RunEvent:
                return base.GetPropertyHeight(prop, label) + 90;
            default:
                return base.GetPropertyHeight(prop, label) + 70;
        }
    }

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        // Rect rect1 = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
        EditorGUI.LabelField(new Rect(pos.x, pos.y, 120, 20), "Event:");
        EditorGUI.PropertyField(new Rect(pos.x + 120, pos.y, pos.width - 80, 20), prop.FindPropertyRelative("_dialogueEvents"), GUIContent.none);

        SerializedProperty _speakerName = prop.FindPropertyRelative("_speakerName");

        SerializedProperty _dialogueLine = prop.FindPropertyRelative("_dialogueLine");
        SerializedProperty _audioClip = prop.FindPropertyRelative("_audioClip");
        SerializedProperty _useAudioClipLengthAsWaitTime = prop.FindPropertyRelative("_useAudioClipLengthAsWaitTime");

        SerializedProperty _waitTime = prop.FindPropertyRelative("_waitTime");

        SerializedProperty _textSpeed = prop.FindPropertyRelative("_textSpeed");

        SerializedProperty savedObject = prop.FindPropertyRelative("savedObject");
        SerializedProperty targetVector3 = prop.FindPropertyRelative("targetVector3");
        SerializedProperty monoBehaviour = prop.FindPropertyRelative("monoBehaviour");
        SerializedProperty unityEvent = prop.FindPropertyRelative("unityEvent");

        DialogueEvents state = (DialogueEvents)prop.FindPropertyRelative("_dialogueEvents").enumValueIndex;
        switch (state)
        {
            case DialogueEvents.SetSpeaker:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), _speakerName);
                break;

            case DialogueEvents.NewLine:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 100), _dialogueLine);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 120, pos.width, 20), _audioClip);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 140, pos.width, 20), _useAudioClipLengthAsWaitTime);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 160, pos.width, 20), _waitTime);
                break;

            case DialogueEvents.AddLine:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 100), _dialogueLine);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 120, pos.width, 20), _audioClip);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 140, pos.width, 20), _useAudioClipLengthAsWaitTime);
                break;

            case DialogueEvents.SetSpeakerAndNewLine:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), _speakerName);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 40, pos.width, 100), _dialogueLine);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 140, pos.width, 20), _audioClip);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 160, pos.width, 20), _useAudioClipLengthAsWaitTime);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 180, pos.width, 20), _waitTime);
                break;

            case DialogueEvents.Wait:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), _waitTime);
                break;

            case DialogueEvents.TextSpeed:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), _textSpeed);
                break;

            case DialogueEvents.MoveObject:
            case DialogueEvents.RotateObject:
            case DialogueEvents.MovePlayer:
            case DialogueEvents.RotatePlayerCam:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), savedObject);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 40, pos.width, 20), targetVector3);
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 60, pos.width, 20), _waitTime);
                break;
            case DialogueEvents.EnableMonoBehavior:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), monoBehaviour);
                break;
            case DialogueEvents.EnableTrigger:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), savedObject);
                break;

            case DialogueEvents.RunEvent:
                EditorGUI.PropertyField(new Rect(pos.x, pos.y + 20, pos.width, 20), unityEvent);
                break;

            default:
                break;
        }
    }
}
#endif