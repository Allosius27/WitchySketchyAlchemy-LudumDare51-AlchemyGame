using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Fields

    private int currentScore;
    private RecipeData recipedate;
    public GameObject ScoreFeedback;
    public GameObject MalusFeedback;
    public GameObject ParentUI;
    public int GetPoints = 50;
    public int GetMalus = 300;
    #endregion

    #region Properties

   public int CurrentScore => currentScore;

    #endregion

    #region Behaviour

    private void Start()
    {
        SetCurrentScore(0);
    }

    public void SetCurrentScore(int GetPoints)
    {
        int timeScore = 10 - (int)Math.Round(GameCore.Instance.CurrentTimer);
        currentScore += GetPoints * (1 + timeScore);
        GameCanvasManager.Instance.UpdateScoreAmount(currentScore);

        if (currentScore > 0)
        {
            var myNewScore = Instantiate(ScoreFeedback, new Vector3(1500, 800,0), Quaternion.identity);
            myNewScore.transform.parent = ParentUI.transform;
        }
        
    }

    public void SetMalus(int GetMalus)
    {
        if (currentScore > 0)
        { 
        currentScore -= GetMalus;
        if (currentScore <= 0)
        { currentScore = 0; }
        }


        GameCanvasManager.Instance.UpdateScoreAmount(currentScore);


        if (currentScore > 0)
        {
            var myNewMalus = Instantiate(MalusFeedback, new Vector3(1500, 800, 0), Quaternion.identity);
            myNewMalus.transform.parent = ParentUI.transform;
        }
    }

    private int Round(float currentTimer)
    {
        throw new NotImplementedException();
    }

    #endregion
}
