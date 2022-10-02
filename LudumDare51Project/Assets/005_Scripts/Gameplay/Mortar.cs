using AllosiusDevCore;
using AllosiusDevUtilities.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    #region Fields

    private FeedbacksReader feedbacksReader;

    #endregion

    #region UnityInspector

    [SerializeField] private IngredientSlot ingredientSlotPrefab;

    [SerializeField] private IngredientData redPowderData, bluePowderData, yellowPowderData;

    [SerializeField] private Transform createPowderSpawnPoint;

    [SerializeField] private FeedbacksData feedbacksUseMortar;

    #endregion

    #region Behaviour

    private void Awake()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);

        IngredientSlot ingredientSlot = collision.gameObject.GetComponent<IngredientSlot>();
        if (ingredientSlot != null && ingredientSlot.Draggable.dragging)
        {
            IngredientSlot slot = Instantiate(ingredientSlotPrefab);
            slot.transform.position = createPowderSpawnPoint.position;
            slot.transform.rotation = Quaternion.identity;
            slot.Init();

            if (ingredientSlot.IngredientDataAssociated.typeColor == TypeColor.Red)
            {
                slot.SetIngredientData(redPowderData);
            }
            else if (ingredientSlot.IngredientDataAssociated.typeColor == TypeColor.Yellow)
            {
                slot.SetIngredientData(yellowPowderData);
            }
            else if (ingredientSlot.IngredientDataAssociated.typeColor == TypeColor.Blue)
            {
                slot.SetIngredientData(bluePowderData);
            }

            ingredientSlot.gameObject.SetActive(false);

            feedbacksReader.ReadFeedback(feedbacksUseMortar);
        }
    }

    #endregion
}
