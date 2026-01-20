using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTriggerTest : MonoBehaviour
{
    [SerializeField] private UnityEvent _startDialogue;
    [SerializeField] private UnityEvent _forceEndDialogue;
    [SerializeField] private UnityEvent _chooseChoice;
    [SerializeField] private UnityEvent _goToPath;
    [SerializeField] private UnityEvent _forceToPath;
    [SerializeField] private UnityEvent _setDialogue;

    [ContextMenu("Trigger Start Dialogue")]
    private void TriggerStartDialogue()
    {
        _startDialogue?.Invoke();
    }

    [ContextMenu("Trigger Force End Dialogue")]
    private void TriggerForceEndDialogue()
    {
        _forceEndDialogue?.Invoke();
    }

    [ContextMenu("Trigger Choose Choice")]
    private void TriggerChooseChoice()
    {
        _chooseChoice?.Invoke();
    }

    [ContextMenu("Trigger Go To Path")]
    private void TriggerGoToPath()
    {
        _goToPath?.Invoke();
    }


    [ContextMenu("Trigger Force To Path")]
    private void TriggerForceToPath()
    {
        _forceToPath?.Invoke();
    }

    [ContextMenu("Trigger Set Dialogue")]
    private void TriggerSetDialogue()
    {
        _setDialogue?.Invoke();
    }
}
