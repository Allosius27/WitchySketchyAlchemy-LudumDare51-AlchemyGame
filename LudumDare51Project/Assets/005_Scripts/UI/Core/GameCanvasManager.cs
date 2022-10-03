using AllosiusDevUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AllosiusDevCore.DialogSystem;
using AllosiusDevCore;
using AllosiusDevUtilities.Core.Menu;
using AllosiusDevUtilities.Core;
using AllosiusDevUtilities.Audio;

public class GameCanvasManager : Singleton<GameCanvasManager>
{
    #region Fields

    private IEnumerator gameOverCoroutine;

    #endregion

    #region Properties

    public RecipeList RecipeList => recipeList;

    //public ScoreUI ScoreUI => scoreUI;

    public DialogueDisplayUI DialogUI => dialogUI;

    public HealthIconsCtrl HealthIconsCtrl => healthIconsCtrl;

    public Transform ScorePoint => scorePoint;
    public Transform SuccessTypeTextPoint => successTypeTextPoint;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private Slider timerBar;

    [Required]
    [SerializeField] private RecipeList recipeList;

    //[Required]
    //[SerializeField] private ScoreUI scoreUI;

    [Required]
    [SerializeField] private TextMeshProUGUI scoreAmount;

    [Required]
    [SerializeField] private Transform scorePoint;

    [Required]
    [SerializeField] private Transform successTypeTextPoint;

    [Required]
    [SerializeField] private DialogueDisplayUI dialogUI;

    [Required]
    [SerializeField] private HealthIconsCtrl healthIconsCtrl;

    [Required]
    [SerializeField] private PageController pageController;
    [Required]
    [SerializeField] private Page gameOverPage;

    #endregion

    #region Behaviour

    protected override void Awake()
    {
        base.Awake();

        dialogUI.gameObject.SetActive(true);
    }

    public void UpdateMaxTimerBar(float value)
    {
        timerBar.maxValue = value;
    }

    public void UpdateTimerBar(float value)
    {
        timerBar.value = value;
    }

    public void UpdateScoreAmount(int value)
    {
        scoreAmount.text = value.ToString();
    }

    public void MixIngredientsSelection()
    {
        if(GameCore.Instance.GameEnd == false && GameCore.Instance.GameInitialized)
        {
            GameCore.Instance.CheckGoodPotion();
        }
    }

    public void ResetCurrentIngredientsSelection()
    {
        if (GameCore.Instance.GameEnd == false && GameCore.Instance.GameInitialized)
        {
            GameCore.Instance.SetCurrentIngredients();
        }
    }

    public void NewIngredients()
    {
        if (GameCore.Instance.GameEnd == false && GameCore.Instance.GameInitialized)
        {
            GameCore.Instance.SetCurrentIngredients(false, false);
        }
    }

    public void GameOverMenu()
    {
        pageController.TurnPageOn(gameOverPage);

        if(gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }

        gameOverCoroutine = CoroutineGameOver();

        StartCoroutine(gameOverCoroutine);
    }

    private IEnumerator CoroutineGameOver()
    {
        yield return new WaitForSeconds(3f);

        // Arrêter le temps
        Time.timeScale = 0;
        // Changer le statut du jeu (l'état : pause ou jeu actif)
        GameStateManager.gameIsPaused = true;
    }

    public void Retry()
    {
        Time.timeScale = 1;

        GameStateManager.gameIsPaused = false;

        AudioController.Instance.StopAllMusics();

        SceneLoader.Instance.ActiveLoadingScreen(GameCore.Instance.CurrentSceneData, 1.0f);
    }

    public void ReturnMainMenu()
    {
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }

        Time.timeScale = 1;

        GameStateManager.gameIsPaused = false;

        UICanvasManager.Instance.PauseMenu.LoadMainMenu();
    }

    #endregion
}
