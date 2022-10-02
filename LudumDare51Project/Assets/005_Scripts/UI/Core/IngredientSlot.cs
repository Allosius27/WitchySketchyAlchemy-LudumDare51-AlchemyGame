using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSlot : MonoBehaviour
{
    #region Fields

    private bool isInteractive;

    private IngredientData ingredientDataAssociated;

    private Draggable draggable;

    private Vector3 startPos;

    #endregion

    #region Properties

    public IngredientData IngredientDataAssociated => ingredientDataAssociated;

    public Draggable Draggable => draggable;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private SpriteRenderer visual;

    [Required]
    [SerializeField] private Animator anim;

    #endregion

    #region Behaviour

    private void Awake()
    {
        
    }

    private void OnMouseDown()
    {
        anim.SetBool("isPick", true);
    }

    private void OnMouseDrag()
    {
        GameCore.Instance.Mortar.SelectableArrowGuide.SetActive(true);
        GameCore.Instance.Cauldron.SelectableArrowGuide.SetActive(true);
    }

    private void OnMouseUp()
    {
        anim.SetBool("isPick", false);

        GameCore.Instance.Mortar.SelectableArrowGuide.SetActive(false);
        GameCore.Instance.Cauldron.SelectableArrowGuide.SetActive(false);
    }

    public void Init()
    {
        draggable = GetComponent<Draggable>();

        startPos = transform.position;
    }

    public void SetActive(bool value)
    {
        isInteractive = value;
        gameObject.SetActive(value);
        if(draggable == null)
        {
            draggable = GetComponent<Draggable>();
        }
        draggable.dragActive = value;
    }

    public void SetIngredientData(IngredientData ingredientData)
    {
        ResetPos();
        ingredientDataAssociated = ingredientData;
        SetVisual(ingredientDataAssociated.ingredientSprite);
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }

    public void SetVisual(Sprite newSprite)
    {
        visual.sprite = newSprite;
    }

    #endregion
}
