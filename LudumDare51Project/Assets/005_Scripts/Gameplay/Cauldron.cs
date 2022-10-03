using AllosiusDevCore;
using AllosiusDevUtilities.Audio;
using AllosiusDevUtilities.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    #region Fields

    private List<IngredientSlot> slotsStocked = new List<IngredientSlot>();

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

    [Required]
    [SerializeField] private IngredientSlot ingredientSlotPrefab;

    [Required]
    [SerializeField] private List<SlotCauldron> slotsCauldron;

    #endregion

    #region Behaviour

    private void Awake()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    public void ResetCauldron()
    {
        ResetSlotsStockedCauldron();

        for (int i = 0; i < currentIngredients.Count; i++)
        {
            Destroy(currentIngredients[i].gameObject);
        }
        for (int i = 0; i < slotsCauldron.Count; i++)
        {
            slotsCauldron[i].IsTaken = false;
        }
        currentIngredients.Clear();
    }

    public void ResetSlotsStockedCauldron()
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
        if(ingredientSlot != null && ingredientSlot.Draggable.dragging && ingredientSlot.IsStocked == false && slotsStocked.Contains(ingredientSlot) == false)
        {
            IngredientSlot slot = Instantiate(ingredientSlotPrefab);
            slot.transform.position = transform.position + new Vector3(Random.Range(0, 0.3f), Random.Range(0, 0.3f), 0);
            for (int i = 0; i < slotsCauldron.Count; i++)
            {
                if(slotsCauldron[i].IsTaken == false)
                {
                    slot.transform.SetParent(slotsCauldron[i].transform);
                    slot.transform.localPosition = Vector3.zero;
                    slotsCauldron[i].IsTaken = true;
                    break;
                }
                
            }


            slot.transform.rotation = Quaternion.identity;
            slot.Init();
            slot.SetIngredientData(ingredientSlot.IngredientDataAssociated);
            slot.Draggable.dragActive = false;
            currentIngredients.Add(slot);

            feedbacksReader.ReadFeedback(feedbacksUseCauldron);

            slotsStocked.Add(ingredientSlot);
            ingredientSlot.IsStocked = true;
            ingredientSlot.gameObject.SetActive(false);

            GameCore.Instance.Mortar.SelectableArrowGuide.SetActive(false);
            selectableArrowGuide.SetActive(false);
        }
    }

    #endregion
}
