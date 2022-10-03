using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIconsCtrl : MonoBehaviour
{
    #region Properties

    public List<HealthIcon> HealthIcons => healthIcons;

    #endregion

    #region UnityInspector

    [SerializeField] private List<HealthIcon> healthIcons;

    #endregion

    #region Behaviour

    public void SetHealthIconsStates(int currentHealthCount)
    {
        for (int i = 0; i < healthIcons.Count; i++)
        {
            if(i < currentHealthCount)
            {
                healthIcons[i].SetHealthIconActive(true);
            }
            else
            {
                healthIcons[i].SetHealthIconActive(false);
            }
        }
    }

    #endregion
}
