using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using FMOD.Studio;
using FMODUnity;

public class SettingsMenuBehavior : MonoBehaviour
{

    [SerializeField] Button returnButton;
    public Slider[] _menuSliders;
    [SerializeField] Toggle colorblindToggle;
    [SerializeField] GameObject settingsMenu;
    private Bus _masterBus;
    private Bus _sfxBus;
    private Bus _musicBus;

    public UnityEvent<bool> settingsMenuClosed;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume") && PlayerPrefs.HasKey("SFXVolume") && PlayerPrefs.HasKey("MusicVolume"))
        {
            _menuSliders[0].value = PlayerPrefs.GetFloat("MasterVolume");
            _menuSliders[1].value = PlayerPrefs.GetFloat("SFXVolume");
            _menuSliders[2].value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            
        }
        
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/SFX");
        _sfxBus = RuntimeManager.GetBus("bus:/MUSIC");

        
        
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            settingsMenuClosed.Invoke(false);
            settingsMenu.SetActive(false);
            
        }

       
        
        
        
            AudioManager.Instance.ModifyBusVolume(_menuSliders[0], "Master");
            AudioManager.Instance.ModifyBusVolume(_menuSliders[1], "SFX");
            AudioManager.Instance.ModifyBusVolume(_menuSliders[2], "Music");
        
        

        


        //_masterBus.setVolume(_menuSliders[0].value);
        //_musicBus.setVolume(_menuSliders[2].value);
        //_sfxBus.setVolume(_menuSliders[1].value);
    }

    public void OnReturnButtonPressed()
    {
        Debug.Log("Return Button Pressed");

        PlayerPrefs.SetFloat("MasterVolume", _menuSliders[0].value);
        PlayerPrefs.SetFloat("SFXVolume", _menuSliders[1].value);
        PlayerPrefs.SetFloat("MusicVolume", _menuSliders[2].value);
        PlayerPrefs.Save();
        settingsMenuClosed.Invoke(false);
        settingsMenu.SetActive(false);
    }






}
