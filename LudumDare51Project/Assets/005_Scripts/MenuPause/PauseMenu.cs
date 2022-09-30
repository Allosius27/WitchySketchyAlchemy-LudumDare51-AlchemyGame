using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using AllosiusDevUtilities.Audio;
using AllosiusDevUtilities.Core.Menu;
using AllosiusDevUtilities.Core;
using AllosiusDevCore;

public class PauseMenu : MonoBehaviour
{
    public Button[] MenuButtons => menuButtons;

    public static bool canPause = true;

    public SettingsMenu settingsMenu;
    public Page pauseMenuUI;

    [SerializeField] private Button[] menuButtons;

    [SerializeField] private SceneData mainMenuSceneData;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuUI.onClosePage += MenuResume;

        if (SceneManager.GetActiveScene().buildIndex == (int)(object)Scenes.MainMenu || SceneManager.GetActiveScene().buildIndex == (int)(object)Scenes.BootScene)
        {
            canPause = false;
        }
        else
        {
            canPause = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            MenuPause();
        }
    }

    public void MenuPause()
    {
        Debug.Log("MenuPause");

        if (canPause)
        {
            if (!GameStateManager.gameIsPaused)
            {
                Paused();
            }
        }
    }

    public void MenuResume()
    {
        if (canPause)
        {
            if (GameStateManager.gameIsPaused)
            {
                Resume();
            }
        }
    }

    public void Paused()
    {
        Debug.Log("Pause");

        //Afficher le menu pause
        UICanvasManager.Instance.PageController.TurnPageOn(pauseMenuUI);
        UICanvasManager.Instance.EventSystem.SetSelectedGameObject(menuButtons[0].gameObject);

        // Arrêter le temps
        Time.timeScale = 0;
        // Changer le statut du jeu (l'état : pause ou jeu actif)
        GameStateManager.gameIsPaused = true;
    }

    public void ResumeAction()
    {
        UICanvasManager.Instance.PageController.TurnPageOff(pauseMenuUI);
    }

    public void Resume()
    {
        Debug.Log("Resume");

        Time.timeScale = 1;

        GameStateManager.gameIsPaused = false;

        settingsMenu.ExitSettings();
        
    }

    public void LoadSettings()
    {
        Debug.Log("Loading Settings menu");
        settingsMenu.LaunchSettings();
        //settingsMenu.SetActive(true);
    }

    public void LoadMainMenu()
    {
        ResumeAction();
        AudioController.Instance.StopAllMusics();
        canPause = false;
        SceneLoader.Instance.ActiveLoadingScreen(mainMenuSceneData, 1.0f);
    }
}
