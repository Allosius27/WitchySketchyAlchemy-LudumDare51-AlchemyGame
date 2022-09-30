using AllosiusDevUtilities.Audio;
using AllosiusDevCore.TranslationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AllosiusDevUtilities.Core;
using AllosiusDevCore;

public class EssentialsLoaders : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    [SerializeField] private LangueManager langueManager;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private UICanvasManager uiCanvasManager;
    [SerializeField] private GameStateManager gameStateManager;

    private void Awake()
    {
        if (AudioController.Instance == null)
        {
            Instantiate(audioController);
        }

        if (LangueManager.Instance == null)
        {
            Instantiate(langueManager);
        }

        if(SceneLoader.Instance == null)
        {
            Instantiate(sceneLoader);
        }

        if(UICanvasManager.Instance == null)
        {
            Instantiate(uiCanvasManager);
        }

        if(GameStateManager.Instance == null)
        {
            Instantiate(gameStateManager);
        }
    }
}
