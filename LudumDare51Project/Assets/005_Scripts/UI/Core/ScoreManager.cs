using AllosiusDevUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    #region Fields

    private int currentScore;
    private RecipeData recipedate;
    public GameObject ScoreFeedback;
    public GameObject MalusFeedback;
    //public GameObject ParentUI;
    public int GetPoints = 50;
    public int GetMalus = 300;

    #endregion

    #region Properties

    public int pointsModifiers { get; set; }

   public int CurrentScore => currentScore;

    #endregion

    #region Behaviour

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SetCurrentScore(0, 1);
    }

    public void SetCurrentScore(int _GetPoints, int _multiplierPoints = 1)
    {
        Debug.Log(_GetPoints);
        int timeScore = 10 - (int)Math.Round(GameCore.Instance.CurrentTimer);
        Debug.Log(timeScore);
        pointsModifiers = (_GetPoints * (1 + timeScore)) *_multiplierPoints;
        currentScore += pointsModifiers;
        Debug.Log(pointsModifiers);
        GameCanvasManager.Instance.UpdateScoreAmount(currentScore);

        if (currentScore > 0)
        {
            var myNewScore = Instantiate(ScoreFeedback, new Vector3(1500, 800,0), Quaternion.identity);
            myNewScore.transform.parent = GameCanvasManager.Instance.transform;
        }
        
    }

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


        if (currentScore > 0)
        {
            var myNewMalus = Instantiate(MalusFeedback, new Vector3(1500, 800, 0), Quaternion.identity);
            myNewMalus.transform.parent = GameCanvasManager.Instance.transform;
        }
    }

    #endregion
}
