using AllosiusDevUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCanvasManager : Singleton<GameCanvasManager>
{
    #region Properties

    public RecipeList RecipeList => recipeList;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private Slider timerBar;

    [Required]
    [SerializeField] private RecipeList recipeList;

    [Required]
    [SerializeField] private TextMeshProUGUI scoreAmount;

    #endregion

    #region Behaviour
    
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

    #endregion
}
