using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItem : MonoBehaviour
{
    #region Properties

    public RecipeData recipeDataAssociated { get; protected set; }

    private int numberActiveIngredients;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private Image[] ingredientsRequiredIcons;

    [Required]
    [SerializeField] private TextMeshProUGUI[] textsTransitionAdd;

    #endregion

    #region Behaviour

    public void SetItem(RecipeData newRecipeData)
    {
        recipeDataAssociated = newRecipeData;

        numberActiveIngredients = 0;

        for (int i = 0; i < ingredientsRequiredIcons.Length; i++)
        {
            if(i < recipeDataAssociated.ingredientsRequired.Length)
            {
                ingredientsRequiredIcons[i].gameObject.SetActive(true);
                ingredientsRequiredIcons[i].sprite = recipeDataAssociated.ingredientsRequired[i].ingredientSprite;

                numberActiveIngredients++;
            }
            else
            {
                ingredientsRequiredIcons[i].sprite = null;
                ingredientsRequiredIcons[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < textsTransitionAdd.Length; i++)
        {
            if(i < numberActiveIngredients - 1)
            {
                textsTransitionAdd[i].gameObject.SetActive(true);
            }
            else
            {
                textsTransitionAdd[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
