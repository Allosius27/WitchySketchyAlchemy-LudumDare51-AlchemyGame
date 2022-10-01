using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeData", menuName = "RecipeData")]
public class RecipeData : ScriptableObject
{
    #region UnityInspector

    [SerializeField] public string recipeName;

    [SerializeField] public IngredientData[] ingredientsRequired;

    [SerializeField] public TypeRecipe typeRecipe;

    #endregion
}

public enum TypeRecipe
{
    RecipeBonus,
    RecipeMalus,
}
