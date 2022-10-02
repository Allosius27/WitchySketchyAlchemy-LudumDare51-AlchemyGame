using AllosiusDevCore;
using AllosiusDevUtilities;
using AllosiusDevUtilities.Audio;
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

    private RecipeData currentBonusRecipe;
    private List<RecipeData> activeRecipes = new List<RecipeData>();
    private RecipeData currentMalusRecipe;

    private RecipeData currentRecipeChecked;

    private Sprite currentSpriteTransformed;

    private List<MemberCtrl> membersAvailables = new List<MemberCtrl>();
    private List<MemberCtrl> memberTransformed = new List<MemberCtrl>();

    private FeedbacksReader feedbacksReader;

    #endregion

    #region Properties

    public float CurrentTimer => currentTimer;

    public bool TimerActive => TimerActive;

    public bool GameEnd => gameEnd;

    public bool GameInitialized => gameInitialized;

    #endregion

    #region UnityInspector

    [SerializeField] private float initTimer = 5.0f;

    [SerializeField] private float roundTimer = 10.0f;

    [SerializeField] private Transform ingredientsSlotsParent;

    [SerializeField] private int minRecipesNumber = 2;
    [SerializeField] private int maxRecipesNumber = 3;

    [Space]

    [SerializeField] private SamplableLibrary ingredientsLibrary;

    [SerializeField] private SamplableLibrary recipesLibrary;

    [SerializeField] private List<Sprite> spritesHeadsTransformed = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesArmsTransformed = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesLegsTransformed = new List<Sprite>();
    [SerializeField] private List<Sprite> spritesTorsosTransformed = new List<Sprite>();

    [Space]

    [Required]
    [SerializeField] private AudioData mainMusic;

    [Required]
    [SerializeField] private FeedbacksData feedbackGetGoodPotion;




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

        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    private void Start()
    {
        GameCanvasManager.Instance.UpdateMaxTimerBar(initTimer);

        AudioController.Instance.PlayAudio(mainMusic);
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
        for (int i = 0; i < activeRecipes.Count; i++)
        {
            if (activeRecipes[i] != null && activeRecipes[i].ingredientsRequired.Length == cauldron.CurrentIngredients.Count)
            {
                bool check = true;

                for (int j = 0; j < activeRecipes[i].ingredientsRequired.Length; j++)
                {
                    if (cauldron.CurrentIngredients[j].IngredientDataAssociated != activeRecipes[i].ingredientsRequired[j])
                    {
                        Debug.Log("false");
                        check = false;
                        break;
                    }
                }

                if(check)
                {
                    Debug.Log("true");
                    currentRecipeChecked = activeRecipes[i];
                    return true;
                }
            }

        }

        return false;
    }

    public void Heal()
    {
        if (currentRecipeChecked != null)
        {
            if (memberTransformed.Count > 0)
            {
                int rndMembers = AllosiusDevUtils.RandomGeneration(0, memberTransformed.Count);

                memberTransformed[rndMembers].SetShapeShifting(null, false);
            }

            if(currentRecipeChecked == currentBonusRecipe)
            {
                // Add Score
                ScoreManager.Instance.SetCurrentScore(currentRecipeChecked.scorePointsBonus, currentRecipeChecked.ingredientsRequired.Length);
            }
            else if(currentBonusRecipe == currentMalusRecipe)
            {
                // Remove Score
                ScoreManager.Instance.SetMalus(currentRecipeChecked.scorePointsLost);
            }
            else
            {
                ScoreManager.Instance.SetCurrentScore(currentRecipeChecked.scorePointsGained, currentRecipeChecked.ingredientsRequired.Length);
            }

            feedbacksReader.ReadFeedback(feedbackGetGoodPotion);

            characterController.Drink();

            SetCurrentRecipe();
            currentTimer = 0.0f;

            currentRecipeChecked = null;
        }
    }

    public void ShapeShifting()
    {
        if (currentBonusRecipe != null)
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

            GetMemberAvailables();
            GetMemberTransformed();

            if (characterController.CheckStateCharacter() == false)
            {
                Debug.Log("Game Over");
                gameEnd = true;
                characterController.Die();
            }
        }
    }

    public void SetCurrentIngredients(bool resetCauldron = true)
    {
        if(resetCauldron)
            cauldron.ResetCauldron();

        List<IngredientData> tempIngredients = new List<IngredientData>();
        for (int i = 0; i < currentBonusRecipe.ingredientsRequired.Length; i++)
        {
            if (currentBonusRecipe.ingredientsRequired[i].isPowder == false)
            {
                tempIngredients.Add(currentBonusRecipe.ingredientsRequired[i]);
            }
            else
            {
                IngredientData ingredient = null;
                int count = 0;
                while((ingredient == null || ingredient.typeColor != currentBonusRecipe.ingredientsRequired[i].typeColor || ingredient.isPowder) && count < 500)
                {
                    count++;
                    int colorIngredientIndex = AllosiusDevUtils.RandomGeneration(0, ingredientsLibrary.library.Count);
                    ingredient = (IngredientData)ingredientsLibrary.library[colorIngredientIndex];
                }
                tempIngredients.Add(ingredient);
            }
            
        }
        for (int i = currentBonusRecipe.ingredientsRequired.Length; i < ingredientsSlots.Count; i++)
        {
            IngredientData ingredient = null;
            int count = 0;
            while ((ingredient == null || ingredient.isPowder) && count < 500)
            {
                count++;
                int rndIngrendient = AllosiusDevUtils.RandomGeneration(0, ingredientsLibrary.library.Count);
                ingredient = (IngredientData)ingredientsLibrary.library[rndIngrendient];
            }
            
            tempIngredients.Add(ingredient);
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
        activeRecipes.Clear();

        int rnd = AllosiusDevUtils.RandomGeneration(0, recipesLibrary.library.Count);
        currentBonusRecipe = (RecipeData)recipesLibrary.library[rnd];

        activeRecipes.Add(currentBonusRecipe);

        int numberRecipes = AllosiusDevUtils.RandomGeneration(minRecipesNumber, maxRecipesNumber);
        for (int i = 0; i < numberRecipes; i++)
        {
            RecipeData recipe = null;
            int count = 0;

            while((recipe == null || activeRecipes.Contains(recipe)) && count < 500)
            {
                count++;
                rnd = AllosiusDevUtils.RandomGeneration(0, recipesLibrary.library.Count);
                recipe = (RecipeData)recipesLibrary.library[rnd];
            }

            activeRecipes.Add(recipe);

        }

        int rndRecipeMalus = AllosiusDevUtils.RandomGeneration(1, activeRecipes.Count);
        currentMalusRecipe = activeRecipes[rndRecipeMalus];

        GameCanvasManager.Instance.RecipeList.ResetCurrentRecipes();

        for (int i = 0; i < activeRecipes.Count; i++)
        {
            GameCanvasManager.Instance.RecipeList.AddCurrentRecipe(activeRecipes[i]);
        }
        
        GameCanvasManager.Instance.RecipeList.SetRecipesList();

        SetCurrentIngredients();

        membersAvailables.Clear();
        memberTransformed.Clear();

        GetMemberAvailables();
        GetMemberTransformed();
    }

    private void GetMemberAvailables()
    {
        if (characterController.headSlot.NaturalState)
        {
            membersAvailables.Add(characterController.headSlot);
        }

        if (characterController.armsSlot.NaturalState)
        {
            membersAvailables.Add(characterController.armsSlot);
        }

        if (characterController.legsSlot.NaturalState)
        {
            membersAvailables.Add(characterController.legsSlot);
        }

        if (characterController.torsoSlot.NaturalState)
        {
            membersAvailables.Add(characterController.torsoSlot);
        }
    }

    private void GetMemberTransformed()
    {
        if (!characterController.headSlot.NaturalState)
        {
            memberTransformed.Add(characterController.headSlot);
        }

        if (!characterController.armsSlot.NaturalState)
        {
            memberTransformed.Add(characterController.armsSlot);
        }

        if (!characterController.legsSlot.NaturalState)
        {
            memberTransformed.Add(characterController.legsSlot);
        }

        if (!characterController.torsoSlot.NaturalState)
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
