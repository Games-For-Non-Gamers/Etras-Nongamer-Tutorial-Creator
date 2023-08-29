using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDialogueTrigger : MonoBehaviour
{
    [SerializeField] private string _tagName = "Player";
    [SerializeField] private DialogueTriggerEvent _dialogueTriggerEvent;
    [SerializeField] private List<DialogueEntryNew> _dialogueEntryNews = new List<DialogueEntryNew>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_tagName))
        {
            DialogueController.Instance.StartNewAutoDialogue(_dialogueEntryNews);

            switch (_dialogueTriggerEvent)
            {
                case DialogueTriggerEvent.None:
                    break;
                case DialogueTriggerEvent.Hide:
                    gameObject.SetActive(false);
                    break;
                case DialogueTriggerEvent.Destroy:
                    Destroy(gameObject);
                    break;
            }

        }
    }
}

public enum DialogueTriggerEvent
{
    None,
    Hide,
    Destroy
}