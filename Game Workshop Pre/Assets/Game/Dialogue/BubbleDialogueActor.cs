using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class BubbleDialogueActor : MonoBehaviour
{
    [SerializeField] private TextAsset _inkJSON;
    [SerializeField] private float _timeBetweenLines;

    private BubbleDialogue _dialogue;
    private Story _story;
    private void Start() {
        CreateDialogue();
    }

    private void CreateDialogue()
    {
        _story = new Story(_inkJSON.text);
        _dialogue = BubbleDialogue.TryCreate(gameObject, new Vector2Int(60, 40));

        if (_story != null && _dialogue != null) NextDialogueLine();
    }

    private void NextDialogueLine()
    {
        if (_story.canContinue)
        {
            _dialogue.Write(_story.Continue(), NextDialogueLine);
        }
    }
    

    private void Update()
    {
        if (_story == null || _dialogue == null) {
            Debug.LogWarning("Story or dialogue is not prepared.");
            return;
        }
        if (_story.currentChoices.Count > 0)
        {
            
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
            index++;
            if (aChoice.text.ToLower() == choice.ToLower())
            {
                _story.ChooseChoiceIndex(index);
                //continue dialogue
                return;
            }
        }

        Debug.LogWarning("Chosen choice for story was not found.");
    }


    // a bit dangerous. If path is invalid it will break the story.
    public void GoToPath(string path) 
    {
        if (_story == null || _dialogue == null) {
            Debug.LogWarning("Story or dialogue is not prepared.");
            return;
        }
        // cancel dialogue
        _story.ChoosePathString(path);
        // start new dialogue
    }

    public void ForceToPath(string path)
    {
        // This will go to path EVEN if its currently running
        //whereas GoToPath must be ended (or waits until its ended?) (wait might need to be another option?)
    }


    //TODO: create a script event trigger that has an event that can activate GoToPath or ChooseChoice 
}
