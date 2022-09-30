using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using AllosiusDevUtilities.Core.Menu;
using AllosiusDevUtilities.Core;
using AllosiusDevCore;

public class SettingsMenu : MonoBehaviour
{
    #region Fields

    private Resolution[] resolutions;
    private List<Resolution> listRes = new List<Resolution>();
    private List<string> options = new List<string>();

    #endregion

    #region Properties

    public MainMenu mainMenu { get; set; }

    public bool windowControls { get; set; }

    public Button[] TabButtonsCtrls => tabButtonsCtrls;
    public TabButtonCtrl CurrentActiveTabSettingMenu => currentActiveTabSettingMenu;

    #endregion

    #region UnityInspector

    [SerializeField] private AudioMixer musicsAudioMixer, sfxAudioMixer, ambientsAudioMixer;

    [SerializeField] private Dropdown resolutionDropDown;

    [SerializeField] private Page settings;

    [SerializeField] private Button[] tabButtonsCtrls;
    [SerializeField] private TabButtonCtrl currentActiveTabSettingMenu;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        //resolutions = Screen.resolutions;
        Debug.Log(resolutions.Length);

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (listRes.Contains(resolutions[i]) == false)
            {
                float ratio = (float)(resolutions[i].width) / (float)(resolutions[i].height);
                Debug.Log(resolutions[i].width + " " + resolutions[i].height);
                Debug.Log(ratio);

                if (ratio >= 1.77f - 0.1f && ratio < 1.77f + 0.1f)
                {
                    listRes.Add(resolutions[i]);
                }
            }
        }

        resolutionDropDown.ClearOptions();

        //listRes.Add(Screen.currentResolution);
        //options.Add(Screen.currentResolution.width + "x" + Screen.currentResolution.height);

        int currentResolutionIndex = 0;
        for (int i = 0; i < listRes.Count; i++)
        {
            //listRes.Add(resolutions[i]);

            string option = listRes[i].width + "x" + listRes[i].height;
            Debug.Log(option);
            options.Add(option);

            if (listRes[i].width == Screen.width &&
                listRes[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);

        Debug.Log(listRes.Count);

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();

        Screen.fullScreen = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(mainMenu != null && mainMenu.activeSettings == true)
        {
            mainMenu.enabled = false;
            mainMenu.activeSettings = false;
        }

        if (Input.GetButtonDown("Escape") || Input.GetButtonDown("Return"))
        {
            ExitSettings();
        }
    }

    public void SetCurrentActiveTabSettingMenu(TabButtonCtrl _button)
    {
        currentActiveTabSettingMenu = _button;

        for (int i = 0; i < tabButtonsCtrls.Length; i++)
        {
            Navigation newNav = new Navigation();
            newNav.mode = Navigation.Mode.Explicit;
            newNav.selectOnUp = tabButtonsCtrls[i].navigation.selectOnUp;
            newNav.selectOnDown = tabButtonsCtrls[i].navigation.selectOnDown;
            newNav.selectOnLeft = tabButtonsCtrls[i].navigation.selectOnLeft;
            newNav.selectOnRight = tabButtonsCtrls[i].navigation.selectOnRight;

            if(currentActiveTabSettingMenu.FirstTabButton != null)
                newNav.selectOnDown = currentActiveTabSettingMenu.FirstTabButton.GetComponent<Selectable>();
            else
            {
                newNav.selectOnDown = null;
            }
            tabButtonsCtrls[i].navigation = newNav;
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = listRes[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetMusicsVolume(float volume)
    {
        musicsAudioMixer.SetFloat("volume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        sfxAudioMixer.SetFloat("volume", volume);
    }

    public void SetAmbientsVolume(float volume)
    {
        ambientsAudioMixer.SetFloat("volume", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void LaunchSettings()
    {
        //settings.SetActive(true);
        UICanvasManager.Instance.PageController.TurnPageOn(settings);
        UICanvasManager.Instance.EventSystem.SetSelectedGameObject(currentActiveTabSettingMenu.gameObject);
    }

    public void ExitSettings()
    {
        if (mainMenu != null)
        {
            mainMenu.activesButtons = true;
            mainMenu.enabled = true;
            UICanvasManager.Instance.EventSystem.SetSelectedGameObject(mainMenu.MenuButtons[0].gameObject);
        }
        else if(GameStateManager.gameIsPaused)
        {
            UICanvasManager.Instance.EventSystem.SetSelectedGameObject(UICanvasManager.Instance.PauseMenu.MenuButtons[0].gameObject);
        }

        //settings.SetActive(false);
        UICanvasManager.Instance.PageController.TurnPageOff(settings);
    }
}
