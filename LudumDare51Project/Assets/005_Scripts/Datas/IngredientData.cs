using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IngredientData", menuName = "IngredientData")]
public class IngredientData : ScriptableObject
{
    #region UnityInspector

    [SerializeField] public Sprite ingredientSprite;

    #endregion
}
