using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class MainMenuBehavior : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject credits;
    [SerializeField] TextMeshProUGUI creditText;
    private Vector3 creditsPosition;
    private EventInstance musicInstance;



    private bool settingsMenuIsOpen;


    public void Start()
    {
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Title Music");
        creditsPosition = creditText.GetComponent<RectTransform>().position;
        musicInstance.start();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


    }

    public void Update()
    {
        if (credits.activeSelf)
        {
            creditText.transform.Translate(Vector3.up * Time.deltaTime * 50 ,Space.World);
            if (creditText.GetComponent<RectTransform>().position.y > creditsPosition.y + 3600)
            {
                creditText.GetComponent<RectTransform>().position = creditsPosition;
            }

        }
        else
        {
            creditText.GetComponent<RectTransform>().position = creditsPosition;
        }
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

        credits.SetActive(true);
        
    }

    public void OnCreditsReturnClicked()
    {
        if (settingsMenuIsOpen)
            return;
        credits.SetActive(false);
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
