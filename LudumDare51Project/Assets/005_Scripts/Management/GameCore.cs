using AllosiusDevUtilities;
using AllosiusDevUtilities.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : Singleton<GameCore>
{
    #region Fields

    private bool gameEnd;

    private CharacterController characterController;
    private Cauldron cauldron;

    private float currentTimer;

    private bool gameInitialized;

    private bool timerActive;

    private List<IngredientSlot> ingredientsSlots = new List<IngredientSlot>();

    private RecipeData currentRecipe;

    private Sprite currentSpriteTransformed;

    private List<MemberCtrl> membersAvailables = new List<MemberCtrl>();
    private List<MemberCtrl> memberTransformed = new List<MemberCtrl>();

    #endregion

    #region Properties

    public bool GameEnd => gameEnd;

    public bool GameInitialized => gameInitialized;

    #endregion

    #region UnityInspector

    [SerializeField] private float initTimer = 5.0f;

    [SerializeField] private float roundTimer = 10.0f;

    [SerializeField] private Transform ingredientsSlotsParent;

    [Space]

    [SerializeField] private SamplableLibrary ingredientsLibrary;

    [SerializeField] private List<RecipeData> recipes = new List<RecipeData>();

    [SerializeField] private List<Sprite> spritesHeadsTransformed = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesArmsTransformed = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesLegsTransformed = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesTorsosTransformed = new List<Sprite>();


    #endregion

    #region Behaviour

    protected override void Awake()
    {
        base.Awake();

        characterController = FindObjectOfType<CharacterController>();
        cauldron = FindObjectOfType<Cauldron>();

        timerActive = true;
        
        foreach(Transform child in ingredientsSlotsParent)
        {
            IngredientSlot ingredientSlot = child.GetComponent<IngredientSlot>();
            if(ingredientSlot != null)
            {
                ingredientsSlots.Add(ingredientSlot);
                ingredientSlot.Init();
                ingredientSlot.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        GameCanvasManager.Instance.UpdateMaxTimerBar(initTimer);
    }

    private void Update()
    {
        if(gameEnd)
        {
            return;
        }

        if(timerActive)
        {
            currentTimer += Time.deltaTime;
            GameCanvasManager.Instance.UpdateTimerBar(currentTimer);

            if(gameInitialized)
            {
                if (currentTimer >= roundTimer)
                {
                    CheckGoodPotion();
                }
            }
            else
            {
                if(currentTimer >= initTimer)
                {
                    SetCurrentRecipe();
                    ShapeShifting();
                    GameCanvasManager.Instance.UpdateMaxTimerBar(roundTimer);
                    gameInitialized = true;
                    currentTimer = 0.0f;
                }
            }
        }
    }

    public void CheckGoodPotion()
    {
        if(MixIngredients())
        {
            Debug.Log("Good Potion");
            Heal();
        }
        else
        {
            Debug.Log("Wrong Potion");

            ShapeShifting();
            SetCurrentRecipe();
            currentTimer = 0.0f;
        }
    }

    public bool MixIngredients()
    {
        if(currentRecipe != null && currentRecipe.ingredientsRequired.Length == cauldron.CurrentIngredients.Count)
        {
            for (int i = 0; i < currentRecipe.ingredientsRequired.Length; i++)
            {
                if(cauldron.CurrentIngredients[i].IngredientDataAssociated != currentRecipe.ingredientsRequired[i])
                {
                    Debug.Log("false");
                    return false;
                }
            }

            Debug.Log("true");
            return true;
        }

        return false;
    }

    public void Heal()
    {
        if (currentRecipe != null && memberTransformed.Count > 0)
        {
            int rndMembers = AllosiusDevUtils.RandomGeneration(0, memberTransformed.Count);

            memberTransformed[rndMembers].SetShapeShifting(null, false);
            characterController.Drink();

            SetCurrentRecipe();
            currentTimer = 0.0f;
        }
    }

    public void ShapeShifting()
    {
        if (currentRecipe != null)
        {
            int rndMembers = AllosiusDevUtils.RandomGeneration(0, membersAvailables.Count);

            int rndSpriteTransformed = 0;
            if (membersAvailables[rndMembers].memberType == MemberType.Head)
            {
                rndSpriteTransformed = AllosiusDevUtils.RandomGeneration(0, spritesHeadsTransformed.Count);
                currentSpriteTransformed = spritesHeadsTransformed[rndSpriteTransformed];
            }
            else if (membersAvailables[rndMembers].memberType == MemberType.Arms)
            {
                rndSpriteTransformed = AllosiusDevUtils.RandomGeneration(0, spritesArmsTransformed.Count);
                currentSpriteTransformed = spritesArmsTransformed[rndSpriteTransformed];
            }
            else if (membersAvailables[rndMembers].memberType == MemberType.Legs)
            {
                rndSpriteTransformed = AllosiusDevUtils.RandomGeneration(0, spritesLegsTransformed.Count);
                currentSpriteTransformed = spritesLegsTransformed[rndSpriteTransformed];
            }
            else if (membersAvailables[rndMembers].memberType == MemberType.Torso)
            {
                rndSpriteTransformed = AllosiusDevUtils.RandomGeneration(0, spritesTorsosTransformed.Count);
                currentSpriteTransformed = spritesTorsosTransformed[rndSpriteTransformed];
            }

            membersAvailables[rndMembers].SetShapeShifting(currentSpriteTransformed, true);
            characterController.Fail();

            if (characterController.CheckStateCharacter() == false)
            {
                Debug.Log("Game Over");
                gameEnd = true;
                characterController.Die();
            }
        }
    }

    public void SetCurrentIngredients()
    {
        cauldron.ResetCauldron();

        List<IngredientData> tempIngredients = new List<IngredientData>();
        for (int i = 0; i < currentRecipe.ingredientsRequired.Length; i++)
        {
            tempIngredients.Add(currentRecipe.ingredientsRequired[i]);
        }
        for (int i = currentRecipe.ingredientsRequired.Length - 1; i < ingredientsSlots.Count; i++)
        {
            int rndIngrendient = AllosiusDevUtils.RandomGeneration(0, ingredientsLibrary.library.Count);
            tempIngredients.Add((IngredientData)ingredientsLibrary.library[rndIngrendient]);
        }
        tempIngredients = AllosiusDevUtils.RandomizeList(tempIngredients);

        for (int i = 0; i < ingredientsSlots.Count; i++)
        {
            ingredientsSlots[i].SetIngredientData(tempIngredients[i]);
            ingredientsSlots[i].SetActive(true);
        }
    }

    public void SetCurrentRecipe()
    {
        int rnd = AllosiusDevUtils.RandomGeneration(0, recipes.Count);
        currentRecipe = recipes[rnd];

        GameCanvasManager.Instance.RecipeList.ResetCurrentRecipes();
        GameCanvasManager.Instance.RecipeList.AddCurrentRecipe(currentRecipe);
        GameCanvasManager.Instance.RecipeList.SetRecipesList();

        SetCurrentIngredients();

        membersAvailables.Clear();
        memberTransformed.Clear();

        if(characterController.headSlot.NaturalState)
        {
            membersAvailables.Add(characterController.headSlot);
        }
        else
        {
            memberTransformed.Add(characterController.headSlot);
        }

        if (characterController.armsSlot.NaturalState)
        {
            membersAvailables.Add(characterController.armsSlot);
        }
        else
        {
            memberTransformed.Add(characterController.armsSlot);
        }

        if (characterController.legsSlot.NaturalState)
        {
            membersAvailables.Add(characterController.legsSlot);
        }
        else
        {
            memberTransformed.Add(characterController.legsSlot);
        }

        if (characterController.torsoSlot.NaturalState)
        {
            membersAvailables.Add(characterController.torsoSlot);
        }
        else
        {
            memberTransformed.Add(characterController.torsoSlot);
        }
    }

    #endregion
}

public enum MemberType
{
    Head,
    Arms,
    Legs,
    Torso,
}
