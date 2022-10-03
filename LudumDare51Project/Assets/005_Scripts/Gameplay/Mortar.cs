using AllosiusDevCore;
using AllosiusDevUtilities.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    #region Fields

    private FeedbacksReader feedbacksReader;

    private List<IngredientSlot> slotsStocked = new List<IngredientSlot>();

    private List<IngredientSlot> powdersCreated = new List<IngredientSlot>();

    #endregion

    #region Properties

    public GameObject SelectableArrowGuide => selectableArrowGuide;

    #endregion

    #region UnityInspector

    [SerializeField] private IngredientSlot ingredientSlotPrefab;

    [SerializeField] private IngredientData redPowderData, bluePowderData, yellowPowderData;

    [SerializeField] private Transform createPowderSpawnPoint;

    [Required]
    [SerializeField] private GameObject selectableArrowGuide;

    [SerializeField] private FeedbacksData feedbacksUseMortar;

    #endregion

    #region Behaviour

    private void Awake()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    public void ResetMortar()
    {
        for (int i = 0; i < powdersCreated.Count; i++)
        {
            Destroy(powdersCreated[i].gameObject);
        }
        powdersCreated.Clear();

        ResetSlotsStockedMortar();
    }

    public void ResetSlotsStockedMortar()
    {
        for (int i = 0; i < slotsStocked.Count; i++)
        {
            slotsStocked[i].IsStocked = false;
        }
        slotsStocked.Clear();
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
            powdersCreated.Add(slot);

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

            slotsStocked.Add(ingredientSlot);
            ingredientSlot.IsStocked = true;
            ingredientSlot.gameObject.SetActive(false);

            selectableArrowGuide.SetActive(false);
            GameCore.Instance.Cauldron.SelectableArrowGuide.SetActive(false);

            feedbacksReader.ReadFeedback(feedbacksUseMortar);
        }
    }

    #endregion
}
