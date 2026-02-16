using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject currentOpenMenu;
    public GameObject pauseMenuUI;
    public GameObject inventoryMenuUI;
    public GameObject optionsMenuUI;
    public GameObject quitUI;
    [SerializeField] public Slider[] _pauseSliders;
    [SerializeField] public TextMeshProUGUI _coinText;
    private FMOD.Studio.EventInstance _music;

    void Awake()
    {
        OpenInventory();
        


    }
    void Start()
    {
        Resume();

        if(_pauseSliders.Length > 0) { 
        if (PlayerPrefs.HasKey("MasterVolume") && PlayerPrefs.HasKey("SFXVolume") && PlayerPrefs.HasKey("MusicVolume"))
        {
            _pauseSliders[0].value = PlayerPrefs.GetFloat("MasterVolume");
            _pauseSliders[1].value = PlayerPrefs.GetFloat("SFXVolume");
            _pauseSliders[2].value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            _pauseSliders[0].value = 1;
            _pauseSliders[1].value = 1;
            _pauseSliders[2].value = 1;
        }
        }

        _music = RuntimeManager.CreateInstance("event:/Music/Hellish Sample");
        _music.start();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentOpenMenu == null)
            {
                // if game is running pause it, else close the current menu
                _coinText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
                Pause();
                

                

            }
            else
            {
                Resume();
                PlayerPrefs.SetFloat("MasterVolume", _pauseSliders[0].value);
                PlayerPrefs.SetFloat("SFXVolume", _pauseSliders[1].value);
                PlayerPrefs.SetFloat("MusicVolume", _pauseSliders[2].value);
                PlayerPrefs.Save();

            }
        }
        AudioManager.Instance.ModifyBusVolume(_pauseSliders[0], "Master");
        AudioManager.Instance.ModifyBusVolume(_pauseSliders[1], "SFX");
        AudioManager.Instance.ModifyBusVolume(_pauseSliders[2], "Music");

        if (Input.GetKeyDown(KeyCode.Tab))
        {
           if (!inventoryMenuUI.activeSelf)
            {
                OpenInventory();
            }
            else
            {
                Resume();
                
            }
        }
    }

    public void Resume()
    {
        ChangeMenu(null);
        _music.setParameterByName("Pause", 0f);

        
    }
    public void OpenInventory()
    {
        ChangeMenu(inventoryMenuUI);
    }
    public void OpenOptions()
    {
        ChangeMenu(optionsMenuUI);

    }

    public void OpenQuit()
    {
        if (quitUI != null)
            ChangeMenu(quitUI);
    }
    public void Restart()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex < 0)
        {
            Debug.LogError("Current scene is not in Build Settings! Add it to be able to reload.");
            return;
        }
        DOTween.KillAll();
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }



    public void Pause()
    {
        ChangeMenu(pauseMenuUI);
        _music.setParameterByName("Pause", 1f);
        PlayerPrefs.SetFloat("MasterVolume", _pauseSliders[0].value);
        PlayerPrefs.SetFloat("SFXVolume", _pauseSliders[1].value);
        PlayerPrefs.SetFloat("MusicVolume", _pauseSliders[2].value);
        PlayerPrefs.Save();
    }

    void ChangeMenu(GameObject newMenu)
    {
        if (newMenu == currentOpenMenu) return;
        pauseMenuUI.SetActive(false);
        inventoryMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        quitUI.SetActive(false);
        if (newMenu != null)
        {
            Time.timeScale = 0f;
            currentOpenMenu = newMenu;
            newMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            currentOpenMenu = null;
        }
    }
}
