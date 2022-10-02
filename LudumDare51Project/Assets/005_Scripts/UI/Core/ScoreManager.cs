using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Fields

    private int currentScore;

    #endregion

    #region Properties

    public int CurrentScore => currentScore;

    #endregion

    #region Behaviour

    private void Start()
    {
        SetCurrentScore(0);
    }

    public void SetCurrentScore(int value)
    {
        currentScore = value;
        GameCanvasManager.Instance.UpdateScoreAmount(currentScore);
    }

    #endregion
}
