using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject currentOpenMenu;
    public GameObject pauseMenuUI;
    public GameObject inventoryMenuUI;
    public GameObject optionsMenuUI;

    void Awake()
    {
        OpenInventory();
    }
    void Start()
    {
        Resume();
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

        SceneManager.LoadScene(sceneIndex);
    }

    public void Pause()
    {
        ChangeMenu(pauseMenuUI);
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
