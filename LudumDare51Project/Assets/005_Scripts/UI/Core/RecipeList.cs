using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeList : MonoBehaviour
{
    #region Fields

    private List<RecipeData> currentRecipes = new List<RecipeData>();

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private Transform recipesContent;

    [Required]
    [SerializeField] private RecipeItem recipeItemPrefab;

    #endregion

    #region Behaviour

    public void ResetCurrentRecipes()
    {
        currentRecipes.Clear();
    }

    public void AddCurrentRecipe(RecipeData recipeData)
    {
        currentRecipes.Add(recipeData);
    }

    public void SetRecipesList()
    {
        foreach(Transform item in recipesContent)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < currentRecipes.Count; i++)
        {
            RecipeItem newRecipeItem = Instantiate(recipeItemPrefab, recipesContent);
            newRecipeItem.SetItem(currentRecipes[i]);
        }
    }

    #endregion
}
