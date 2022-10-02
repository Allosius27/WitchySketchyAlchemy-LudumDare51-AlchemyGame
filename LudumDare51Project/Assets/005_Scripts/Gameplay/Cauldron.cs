using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    #region Fields

    private List<IngredientSlot> currentIngredients = new List<IngredientSlot>();

    #endregion

    #region Properties

    public List<IngredientSlot> CurrentIngredients => currentIngredients;

    #endregion

    #region Behaviour

    public void ResetCauldron()
    {
        currentIngredients.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);

        IngredientSlot ingredientSlot = collision.gameObject.GetComponent<IngredientSlot>();
        if(ingredientSlot != null && ingredientSlot.Draggable.dragging)
        {
            currentIngredients.Add(ingredientSlot);
            ingredientSlot.gameObject.SetActive(false);
        }
    }

    #endregion
}
