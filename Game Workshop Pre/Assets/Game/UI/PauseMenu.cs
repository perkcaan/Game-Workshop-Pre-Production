using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject currentOpenMenu;
    public GameObject pauseMenuUI;
    public GameObject inventoryMenuUI;
    public GameObject optionsMenuUI;
    [SerializeField] public Slider[] _pauseSliders;
    private SettingsMenuBehavior _behavior;

    private FMOD.Studio.EventInstance _music;

    void Awake()
    {
        OpenInventory();
    }
    void Start()
    {
        Resume();
        
        //_pauseSliders[0].value = _behavior._menuSliders[0].value;
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
                Pause();
                

            }
            else
            {
                Resume();
                
            }
        }

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

    public void Pause()
    {
        ChangeMenu(pauseMenuUI);
        _music.setParameterByName("Pause", 1f);
    }

    void ChangeMenu(GameObject newMenu)
    {
        if (newMenu == currentOpenMenu) return;
        pauseMenuUI.SetActive(false);
        inventoryMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
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
