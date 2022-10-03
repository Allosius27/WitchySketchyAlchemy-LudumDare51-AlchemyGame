using AllosiusDevUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AllosiusDevCore;

public class ScoreManager : Singleton<ScoreManager>
{
    #region Fields

    private int currentScore;
    private RecipeData recipedate;

    private FeedbacksReader feedbacksReader;

    #endregion

    #region Properties

    public int pointsModifiers { get; set; }

    public int CurrentScore => currentScore;

    #endregion

    #region UnityInspector

    public GameObject ScoreFeedback;
    public GameObject MalusFeedback;
    //public GameObject ParentUI;
    public int GetPoints = 50;
    public int GetMalus = 300;

    [Required]
    [SerializeField] private FeedbacksData feedbacksAddScore;

    #endregion

    #region Behaviour

    protected override void Awake()
    {
        base.Awake();

        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    private void Start()
    {
        SetCurrentScore(0, 1);
    }

    [Button(ButtonSizes.Large)]
    public void SetCurrentScore(int _GetPoints, int _multiplierPoints = 1)
    {
        Debug.Log(_GetPoints);
        int timeScore = 10 - (int)Math.Round(GameCore.Instance.CurrentTimer);
        Debug.Log(timeScore);
        pointsModifiers = (_GetPoints * (1 + timeScore)) *_multiplierPoints;
        currentScore += pointsModifiers;
        Debug.Log(pointsModifiers);
        GameCanvasManager.Instance.UpdateScoreAmount(currentScore);

        feedbacksReader.ReadFeedback(feedbacksAddScore);

        if (currentScore > 0)
        {
            var myNewScore = Instantiate(ScoreFeedback, GameCanvasManager.Instance.ScorePoint.position, Quaternion.identity);
            myNewScore.transform.parent = GameCanvasManager.Instance.transform;
            myNewScore.GetComponent<PopUpText>().SetPoints(pointsModifiers);
        }
        
    }

    [Button(ButtonSizes.Large)]
    public void SetMalus(int _GetMalus)
    {

        if (currentScore > 0)
        { 
        currentScore -= _GetMalus;
        pointsModifiers = _GetMalus;
        Debug.Log(pointsModifiers);

        if (currentScore <= 0)
        { currentScore = 0; }
        }


        GameCanvasManager.Instance.UpdateScoreAmount(currentScore);


        
            var myNewMalus = Instantiate(MalusFeedback, GameCanvasManager.Instance.ScorePoint.position, Quaternion.identity);
            myNewMalus.transform.parent = GameCanvasManager.Instance.transform;
            myNewMalus.GetComponent<PopUpText>().SetPoints(-pointsModifiers);
        
    }

    #endregion
}
