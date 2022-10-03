using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIcon : MonoBehaviour
{
    #region UnityInspector

    [Required]
    [SerializeField] private GameObject healthIconActive;
    [Required]
    [SerializeField] private GameObject healthIconInactive;

    #endregion

    #region Behaviour

    public void SetHealthIconActive(bool value)
    {
        healthIconActive.SetActive(value);
        healthIconInactive.SetActive(!value);
    }

    #endregion
}
