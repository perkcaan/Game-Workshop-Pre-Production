using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BubbleDialogueActor : MonoBehaviour
{
    [SerializeField] private Vector2 _bubbleOffset;
    [SerializeField] private TextAsset _inkJSON;
    
    private BubbleDialogue _dialogue;
    private Story _story;
    private bool _isInDialogue = false;

    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetDialogue(TextAsset newAsset)
    {
        _inkJSON = newAsset;
    }
    
    public void StartDialogue() {
        if (_dialogue == null) CreateDialogue();
    }

    public void ForceEndDialogue()
    {
        if (_isInDialogue || _dialogue != null)
        {
            _isInDialogue = false;
            _animator.SetBool("inDialogue", false);
            _dialogue.End();
            _story = null;
            _dialogue = null;
        }

    }

    private void CreateDialogue()
    {
        _story = new Story(_inkJSON.text);
        _dialogue = BubbleDialogue.TryCreate(gameObject, new Vector2Int(60, 40), _bubbleOffset);

        if (_story != null && _dialogue != null) NextDialogueLine();
    }

    private void NextDialogueLine()
    {
        if (_story.canContinue)
        {
            _isInDialogue = true;
            _animator.SetBool("inDialogue", true);
            _dialogue.Write(_story.Continue(), NextDialogueLine);
        } else
        {
            _dialogue.Close();
            _isInDialogue = false;
            _animator.SetBool("inDialogue", false);
            if (_story.currentChoices.Count > 0)
            {
                
            } else
            {
                
                _dialogue.End();
                _story = null;
                _dialogue = null;
            }
        }
    }
    
    // choice has to be exactly the correct string minus case
    public void ChooseChoice(string choice)
    {
        if (_story == null || _dialogue == null) {
            Debug.LogWarning("Story or dialogue is not prepared.");
            return;
        }

        int index = 0;
        foreach (Choice aChoice in _story.currentChoices)
        {
            if (aChoice.text.ToLower() == choice.ToLower())
            {
                if (_isInDialogue) // In dialogue when choice is picked
                {
                    _story.ChooseChoiceIndex(index);
                } else { // Dialogue already finished when choice is picked
                    _dialogue.Open();
                    _story.ChooseChoiceIndex(index);
                    NextDialogueLine();
                }
                //continue dialogue
                return;
            }
            index++;
        }

        Debug.LogWarning("Chosen choice for story was not found.");
    }


    //If path is invalid it will break the story.
    //Will redirect next dialogue to be on new path
    public void GoToPath(string path) 
    {
        if (_story == null || _dialogue == null) {
            Debug.LogWarning("Story or dialogue is not prepared.");
            return;
        }
        _story.ChoosePathString(path);
    }

    //If path is invalid it will break the story.
    //Will interrupt dialogue to be on new path
    public void ForceToPath(string path)
    {
        if (_story == null || _dialogue == null) {
            Debug.LogWarning("Story or dialogue is not prepared.");
            return;
        }
        _dialogue.CancelWrite();
        _story.ChoosePathString(path);
        NextDialogueLine();
    }

}
