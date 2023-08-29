using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AutoDialoguePlayer : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TMP_Text _speakerNameTMP;
    [SerializeField] private TMP_Text _dialogueTextTMP;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _dialogueCanvasUi;

    [Header("Text")]
    [SerializeField] private float _textCharSpeed = 0.04f;
    [SerializeField] private bool _showTextOverTime = true;

    private Coroutine _coroutineText;

    public void ClearText()
    {
        _speakerNameTMP.text = string.Empty;
        _dialogueTextTMP.text = string.Empty;
    }

    public void SetTextSpeed(float textCharSpeed)
    {
        _textCharSpeed = textCharSpeed;
    }

    public void HideTextUI(bool hide)
    {
        _dialogueCanvasUi.SetActive(!hide);
    }

    public void SetSpeaker(string speakerName)
    {
        _speakerNameTMP.text = speakerName;
    }

    public void StartDialogueRead(string textDialogue, AudioClip audioClip, bool clearText)
    {
        if (_coroutineText != null)
        {
            StopCoroutine(_coroutineText);
        }

        _coroutineText = StartCoroutine(StartDialogueReadCoroutine(textDialogue, audioClip, clearText));
    }

    private IEnumerator StartDialogueReadCoroutine(string textDialogue, AudioClip audioClip, bool clearText)
    {
        _audioSource.Stop();
        if (audioClip != null)
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        if (clearText)
        {
            _dialogueTextTMP.text = string.Empty;
        }

        if (_showTextOverTime)
        {
            foreach (System.Char c in textDialogue.ToCharArray())
            {
                _dialogueTextTMP.text += c;
                if (c != ' ')
                {
                    yield return new WaitForSeconds(_textCharSpeed);
                }
            }
        }
        else
        {
            _dialogueTextTMP.text = textDialogue;
        }
    }

    private void Reset()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.reverbZoneMix = 0;
    }
}