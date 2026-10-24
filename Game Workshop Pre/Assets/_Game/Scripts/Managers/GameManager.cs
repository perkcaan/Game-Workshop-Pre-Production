using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private List<GameObject> _requiredPrefabs;
    [SerializeField] private GameObject _sceneTransition;
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


    public static void LoadScene(string sceneName) {
        Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName) {

        //begin loading the scene
        GameObject transition = Instantiate(_sceneTransition);
        Animator transitionAnim = transition.GetComponent<Animator>();
        transitionAnim.SetTrigger("Start");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(0.5f); //pretend this is a cool transition effect

        //wait until scene is loaded 90&
        while (asyncLoad.progress < 0.9f) {
            yield return null;
        }

        transitionAnim.SetTrigger("End");
        //any last minute effect here
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        asyncLoad.allowSceneActivation = true;
        

        // Scene loads and transitions
    }
}
