using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class MainMenuBehavior : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    private EventInstance musicInstance;


    private bool settingsMenuIsOpen;


    public void Start()
    {
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Title Music");

        musicInstance.start();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }
    public void OnPlayButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;

        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SceneManager.LoadScene(1);
    }

    public void OnSettingsButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;
        else
            settingsMenuIsOpen = true;

        settingsMenu.SetActive(true);
    }

    public void OnCreditsButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;

    }

    public void OnExitButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;

        if (Application.isPlaying)
        {
            Application.Quit();
        }
        #if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        #endif
    }

    public void SettingsMenuClosed(bool isClosed)
    {
        settingsMenuIsOpen = isClosed;
    }

}
