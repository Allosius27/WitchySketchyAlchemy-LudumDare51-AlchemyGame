using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using AllosiusDevUtilities.Audio;
using AllosiusDevCore;

public class MainMenu : MonoBehaviour
{

    #region Fields

    

    #endregion

    #region Properties
    public SettingsMenu SettingsMenu { get; protected set; }

    public bool activeSettings { get; set; }
    public bool activesButtons { get; set; }

    public Button[] MenuButtons => menuButtons;

    #endregion

    #region UnityInspector
    //public GameObject continueButton;
    //public static bool continueActive;
    //public Button buttonContinue;

    //public string loadGameScene;

    [SerializeField] private Button[] menuButtons;

    [SerializeField] private SceneData startLevelSceneData;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    [Space]

    public AudioData mainMenuMusic;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        SettingsMenu = UICanvasManager.Instance.SettingsMenu;
        SettingsMenu.mainMenu = this;

        activesButtons = true;

        AudioController.Instance.PlayAudio(mainMenuMusic);

        Cursor.SetCursor(null, hotSpot, cursorMode);

        PauseMenu.canPause = false;

        /*if (PlayerPrefs.HasKey("Current_Scene"))
        {
            continueActive = true;
        }
        else
        {
            continueActive = false;
        }*/

        UICanvasManager.Instance.EventSystem.SetSelectedGameObject(menuButtons[0].gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(continueActive)
        {
            buttonContinue.interactable = true;
        }
        else
        {
            buttonContinue.interactable = false;
        }*/

        
    }

    /*public void Continue()
    {
        SceneManager.LoadScene(loadGameScene);
    }*/

    public void NewGame(float _timeToWait)
    {
        PauseMenu.canPause = true;
        //StartCoroutine(SceneLoader.Instance.LoadAsynchronously(Scenes.Level));
        SceneLoader.Instance.ActiveLoadingScreen(startLevelSceneData, _timeToWait);

    }
   
    public void Options()
    {
        Debug.Log("Launch Menu Options");
        SettingsMenu.LaunchSettings();
        activesButtons = false;
        activeSettings = true;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}

