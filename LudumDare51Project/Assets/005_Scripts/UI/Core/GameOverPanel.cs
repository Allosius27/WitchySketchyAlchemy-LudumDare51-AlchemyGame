using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    #region UnityInspector

    [SerializeField] private TextMeshProUGUI scoreAmount;

    #endregion

    #region Behaviour

    private void Start()
    {
        UpdateScoreAmount(ScoreManager.Instance.CurrentScore);
    }

    public void UpdateScoreAmount(int value)
    {
        scoreAmount.text = value.ToString();
    }

    #endregion
}
