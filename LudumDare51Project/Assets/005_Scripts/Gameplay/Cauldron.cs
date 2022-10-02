using AllosiusDevCore;
using AllosiusDevUtilities.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    #region Fields

    private List<IngredientSlot> currentIngredients = new List<IngredientSlot>();

    private FeedbacksReader feedbacksReader;

    #endregion

    #region Properties

    public List<IngredientSlot> CurrentIngredients => currentIngredients;

    public GameObject SelectableArrowGuide => selectableArrowGuide;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private FeedbacksData feedbacksUseCauldron;

    [Required]
    [SerializeField] private GameObject selectableArrowGuide;

    #endregion

    #region Behaviour

    private void Awake()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
    }

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
            feedbacksReader.ReadFeedback(feedbacksUseCauldron);

            currentIngredients.Add(ingredientSlot);
            ingredientSlot.gameObject.SetActive(false);

            GameCore.Instance.Mortar.SelectableArrowGuide.SetActive(false);
            selectableArrowGuide.SetActive(false);
        }
    }

    #endregion
}
