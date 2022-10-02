using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeData", menuName = "RecipeData")]
public class RecipeData : ScriptableObject
{
    #region UnityInspector

    [SerializeField] public string recipeName;

    [SerializeField] public IngredientData[] ingredientsRequired;

    //[SerializeField] public TypeRecipe typeRecipe;

    [SerializeField] public bool hasSpecialEffect;
    //Define Effect

    //[SerializeField] public bool hasNegativeEffect;
    //Define Effect

    #endregion
}

public enum TypeRecipe
{
    RecipeBonus,
    RecipeMalus,
    RecipeNeutral,
}
