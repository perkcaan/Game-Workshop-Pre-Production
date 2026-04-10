using System;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    // Subscribe to event OnEnable (and unsubscribe on OnDisable) to update along with settings
    public Action SettingsChanged;

    // We can start using this as the full GameManager.
    [Header("Settings")]
    [SerializeField] private bool _useTrashballLabels = true;
    public bool UseTrashballLabels { 
        get { return _useTrashballLabels; } 
        set { _useTrashballLabels = value; 
        SettingsChanged?.Invoke(); } 
    }


    private void OnValidate()
    {
        SettingsChanged?.Invoke();
    }
}
