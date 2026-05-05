using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    [SerializeField] private List<GameObject> _requiredPrefabs;
    [SerializeField] private PlayerMovementController _player;
    public PlayerMovementController CurrentPlayer { get { return _player; } }

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
    

    protected override void Awake()
    {
        base.Awake();
        foreach (GameObject prefab in _requiredPrefabs)
        {
            GameObject gameObject = Instantiate(prefab);
            gameObject.name = prefab.name;
            gameObject.transform.SetParent(transform);
        }
    }
}
