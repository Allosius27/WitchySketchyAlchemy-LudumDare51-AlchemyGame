using AllosiusDevCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Fields

    private Sprite defaultBodySprite;
    private Sprite defaultLegsSprite;
    private Sprite defaultArmsSprite;
    private Sprite defaultHatSprite;
    private Sprite defaultHeadSprite;
    private Sprite defaultHairSprite;

    private FeedbacksReader feedbacksReader;

    #endregion

    #region Properties

    public GameObject PotionObj => potionObj;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] public MemberCtrl headSlot, armsSlot, legsSlot, torsoSlot;

    [Required]
    [SerializeField] private SpriteRenderer humanBodySprite;
    [Required]
    [SerializeField] private SpriteRenderer humanLegsSprite;
    [Required]
    [SerializeField] private SpriteRenderer humanArmsSprite;
    [Required]
    [SerializeField] private SpriteRenderer humanHatSprite;
    [Required]
    [SerializeField] private SpriteRenderer humanHeadSprite;
    [Required]
    [SerializeField] private SpriteRenderer humanHairSprite;

    [Required]
    [SerializeField] private Animator anim;

    [Required]
    [SerializeField] private GameObject potionObj;

    [Required]
    [SerializeField] private FeedbacksData feedbacksPlayerDrink;

    [Required]
    [SerializeField] private FeedbacksData feedbacksPlayerShapeShifting;

    #endregion

    #region Behaviour

    private void Awake()
    {
        defaultBodySprite = humanBodySprite.sprite;
        defaultLegsSprite = humanLegsSprite.sprite;
        defaultArmsSprite = humanArmsSprite.sprite;
        defaultHatSprite = humanHatSprite.sprite;
        defaultHeadSprite = humanHeadSprite.sprite;
        defaultHairSprite = humanHairSprite.sprite;

        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    private void Start()
    {
        potionObj.SetActive(false);
    }

    public void Fail()
    {
        anim.SetTrigger("isWrong");
    }

    public void ShapeShifting()
    {
        feedbacksReader.ReadFeedback(feedbacksPlayerShapeShifting);
    }

    public void Drink()
    {
        potionObj.SetActive(true);
        anim.SetTrigger("isRight");

        feedbacksReader.ReadFeedback(feedbacksPlayerDrink);
    }

    public void Die()
    {
        anim.SetTrigger("isDead");
    }

    public bool CheckStateCharacter()
    {
        if(headSlot.NaturalState)
        {
            return true;
        }
        else if (armsSlot.NaturalState)
        {
            return true;
        }
        else if (legsSlot.NaturalState)
        {
            return true;
        }
        else if (torsoSlot.NaturalState)
        {
            return true;
        }

        return false;
    }

    public void SetNewHumanForm(Sprite newBody, Sprite newLegs, Sprite newArms, Sprite newHat, Sprite newHead, Sprite newHair)
    {
        humanBodySprite.sprite = newBody;
        humanLegsSprite.sprite = newLegs;
        humanArmsSprite.sprite = newArms;
        humanHatSprite.sprite = newHat;
        humanHeadSprite.sprite = newHead;
        humanHairSprite.sprite = newHair;
    }

    public void ResetHumanForm()
    {
        humanBodySprite.sprite = defaultBodySprite;
        humanLegsSprite.sprite = defaultLegsSprite;
        humanArmsSprite.sprite = defaultArmsSprite;
        humanHatSprite.sprite = defaultHatSprite;
        humanHeadSprite.sprite = defaultHeadSprite;
        humanHairSprite.sprite = defaultHairSprite;
    }

    #endregion
}
