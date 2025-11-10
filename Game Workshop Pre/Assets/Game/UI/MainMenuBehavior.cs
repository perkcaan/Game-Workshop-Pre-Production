using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{


    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject settingsMenu;

    private bool settingsMenuIsOpen;

    public void Awake()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Music/Title Music", Vector3.zero);
    }
    public void OnPlayButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;

        Debug.Log("Play Clicked");
        SceneManager.LoadScene("October Level");
    }

    public void OnSettingsButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;
        else
            settingsMenuIsOpen = true;

            Debug.Log("Settings Clicked");
        settingsMenu.SetActive(true);
    }

    public void OnCreditsButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;

        Debug.Log("Credits Clicked");
    }

    public void OnExitButtonClicked()
    {
        if (settingsMenuIsOpen)
            return;

        Debug.Log("Exit Button Clicked");
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
