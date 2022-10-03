using AllosiusDevCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New RecipeData", menuName = "RecipeData")]
public class RecipeData : ScriptableObject
{
    #region UnityInspector

    [SerializeField] public string recipeName;

    [SerializeField] public List<IngredientData> ingredientsRequired = new List<IngredientData>(); 

    //[SerializeField] public TypeRecipe typeRecipe;

    [SerializeField] public bool hasSpecialEffect;
    //Define Effect
    [ShowIfGroup("hasSpecialEffect")]
    [Required]
    [SerializeField] public FeedbacksData feedbacksSpecialEffectActivation;

    //[SerializeField] public bool hasNegativeEffect;
    //Define Effect

    [SerializeField] public int scorePointsGained;
    [SerializeField] public int scorePointsLost;
    [SerializeField] public int scorePointsBonus;

    #endregion
}

/*public enum TypeRecipe
{
    RecipeBonus,
    RecipeMalus,
    RecipeNeutral,
}*/
