using Etra.StarterAssets.Abilities;
using Etra.StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(AutoDialoguePlayer))]
public class DialogueController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private AutoDialoguePlayer _autoDialoguePlayer;

    [Header("Using Audio Clip Length As Wait Time")]
    [SerializeField] private float _additionalWaitTimeAfterAudioClip = 0.5f;

    private Coroutine _coroutineDialogue;
    private List<DialogueEntryNew> _entries = new List<DialogueEntryNew>();
    EtraCharacterMainController mainController;
    public static DialogueController Instance { get; private set; }

    public void StartNewAutoDialogue(List<DialogueEntryNew> entries)
    {
        if (_coroutineDialogue != null)
        {
            StopCoroutine(_coroutineDialogue);
        }
        _entries = entries;

        _coroutineDialogue = StartCoroutine(StartAutoDialogueCoroutine());
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _autoDialoguePlayer.ClearText();
        _autoDialoguePlayer.HideTextUI(false);
    }

    private void Start()
    {
        mainController = FindAnyObjectByType<EtraCharacterMainController>();
    }

    private void Reset()
    {
        _autoDialoguePlayer = GetComponent<AutoDialoguePlayer>();
    }

    private IEnumerator StartAutoDialogueCoroutine()
    {
        _autoDialoguePlayer.ClearText();
        _autoDialoguePlayer.HideTextUI(false);

        foreach (DialogueEntryNew dialogueEntry in _entries)
        {
            float waitTime = 0;
            switch (dialogueEntry.DialogueEvents)
            {
                case DialogueEvents.SetSpeaker:
                    SetSpeaker(dialogueEntry);
                    break;
                case DialogueEvents.NewLine:
                    waitTime = NewLine(dialogueEntry);
                    yield return new WaitForSeconds(waitTime);
                    break;
                case DialogueEvents.AddLine:
                    waitTime = AddLine(dialogueEntry);
                    yield return new WaitForSeconds(waitTime);
                    break;
                case DialogueEvents.SetSpeakerAndNewLine:
                    waitTime = SetSpeakerAndNewLine(dialogueEntry);
                    yield return new WaitForSeconds(waitTime);
                    break;
                case DialogueEvents.Wait:
                    yield return new WaitForSeconds(dialogueEntry.WaitTime);
                    break;
                case DialogueEvents.TextSpeed:
                    SetTextSpeed(dialogueEntry);
                    break;

                case DialogueEvents.MoveObject:
                    LeanTween.move(dialogueEntry.savedObject, dialogueEntry.targetVector3, dialogueEntry.WaitTime);
                    break;
                case DialogueEvents.RotateObject:
                    LeanTween.rotate(dialogueEntry.savedObject, dialogueEntry.targetVector3, dialogueEntry.WaitTime);
                    break;
                case DialogueEvents.MovePlayer:
                    if (dialogueEntry.savedObject != null)
                    {
                        LeanTween.move(mainController.gameObject, new Vector3 (dialogueEntry.savedObject.transform.position.x, mainController.gameObject.transform.position.y, dialogueEntry.savedObject.transform.position.z), dialogueEntry.WaitTime);
                    }
                    else
                    {
                        LeanTween.move(mainController.gameObject, dialogueEntry.targetVector3, dialogueEntry.WaitTime);
                    }

                    break;

                case DialogueEvents.RotatePlayerCam:
                    if (mainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
                    {
                        if (dialogueEntry.savedObject != null)
                        {
                            Vector3 savedRot = dialogueEntry.savedObject.transform.rotation.eulerAngles;
                            LeanTween.rotate(mainController.gameObject, savedRot, dialogueEntry.WaitTime).setOnComplete(() => setCameraRot(mainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>(), savedRot));
                        }
                        else
                        {
                            Vector3 savedRot = dialogueEntry.targetVector3;
                            LeanTween.rotate(mainController.gameObject, savedRot, dialogueEntry.WaitTime).setOnComplete(() => setCameraRot(mainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>(), savedRot));
                        }


                    }
                    break;
                case DialogueEvents.LockPlayer:
                    mainController.disableAllActiveAbilities();
                    break;

                case DialogueEvents.UnlockPlayer:
                    mainController.enableAllActiveAbilities();
                    break;

                case DialogueEvents.EnableMonoBehavior:
                    dialogueEntry.monoBehaviour.enabled = true;
                    break;

                case DialogueEvents.EnableTrigger:
                    if (dialogueEntry.savedObject.GetComponent<BoxCollider>() && dialogueEntry.savedObject.GetComponent<BoxCollider>().isTrigger)
                    {
                        dialogueEntry.savedObject.GetComponent<BoxCollider>().enabled = true;
                    }
                    if (dialogueEntry.savedObject.GetComponent<CapsuleCollider>() && dialogueEntry.savedObject.GetComponent<CapsuleCollider>().isTrigger)
                    {
                        dialogueEntry.savedObject.GetComponent<CapsuleCollider>().enabled = true;
                    }
                    if (dialogueEntry.savedObject.GetComponent<SphereCollider>() && dialogueEntry.savedObject.GetComponent<SphereCollider>().isTrigger)
                    {
                        dialogueEntry.savedObject.GetComponent<SphereCollider>().enabled = true;
                    }
                    if (dialogueEntry.savedObject.GetComponent<MeshCollider>() && dialogueEntry.savedObject.GetComponent<MeshCollider>().isTrigger)
                    {
                        dialogueEntry.savedObject.GetComponent<MeshCollider>().enabled = true;
                    }

                    break;
                case DialogueEvents.RunEvent:
                    dialogueEntry.unityEvent.Invoke();
                    break;

            }
        }
        _autoDialoguePlayer.HideTextUI(true);
    }

    public void setCameraRot(ABILITY_CameraMovement camScript, Vector3 rot)
    {
        camScript.manualSetCharacterAndCameraRotation(rot);
    }

    private void SetSpeaker(DialogueEntryNew dialogueEntry)
    {
        _autoDialoguePlayer.SetSpeaker(dialogueEntry.SpeakerName);
    }

    private float NewLine(DialogueEntryNew dialogueEntry)
    {
        _autoDialoguePlayer.StartDialogueRead(dialogueEntry.DialogueLine, dialogueEntry.AudioClip,true);
        if (dialogueEntry.AudioClip != null && dialogueEntry.UseAudioClipLengthAsWaitTime)
        {
            return dialogueEntry.AudioClip.length + _additionalWaitTimeAfterAudioClip;
        }
        else if (dialogueEntry.WaitTime != 0)
        {
            return dialogueEntry.WaitTime;
        }
        else
        {
            return 0;
        }
    }

    private float AddLine(DialogueEntryNew dialogueEntry)
    {
        _autoDialoguePlayer.StartDialogueRead(dialogueEntry.DialogueLine, dialogueEntry.AudioClip,false);
        if (dialogueEntry.AudioClip != null && dialogueEntry.UseAudioClipLengthAsWaitTime)
        {
            return dialogueEntry.AudioClip.length + _additionalWaitTimeAfterAudioClip;
        }
        else
        {
            return 0;
        }
    }

    private float SetSpeakerAndNewLine(DialogueEntryNew dialogueEntry)
    {

        SetSpeaker(dialogueEntry);
        return NewLine(dialogueEntry);
    }

    private void SetTextSpeed(DialogueEntryNew dialogueEntry)
    {
        _autoDialoguePlayer.SetTextSpeed(dialogueEntry.TextSpeed);
    }
}