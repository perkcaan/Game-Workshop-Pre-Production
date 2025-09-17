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



    public void OnPlayButtonClicked()
    {
        Debug.Log("Play Clicked");
        SceneManager.LoadScene("LoadingScreen");
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings Clicked");
    }

    public void OnCreditsButtonClicked()
    {
        Debug.Log("Credits Clicked");
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked");
        if (Application.isPlaying)
        {
            Application.Quit();
        }

        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
    }


}
