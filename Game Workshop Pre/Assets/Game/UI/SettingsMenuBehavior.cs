using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsMenuBehavior : MonoBehaviour
{

    [SerializeField] Button returnButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle colorblindToggle;
    [SerializeField] GameObject settingsMenu;

    public UnityEvent<bool> settingsMenuClosed;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            settingsMenuClosed.Invoke(false);
            settingsMenu.SetActive(false);
        }
    }

    public void OnReturnButtonPressed()
    {
        Debug.Log("Return Button Pressed");
        settingsMenuClosed.Invoke(false);
        settingsMenu.SetActive(false);
    }






}
