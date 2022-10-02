using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    #region UnityInspector

    [Required]
    [SerializeField] private CharacterController characterController;

    #endregion

    #region Behaviour

    public void ActivePotionObj()
    {
        SetActivePotionObj(true);
    }

    public void DeactivePotionObj()
    {
        SetActivePotionObj(false);
    }

    private void SetActivePotionObj(bool value)
    {
        characterController.PotionObj.SetActive(value);
    }

    public void ChangeFacialExpression()
    {

    }

    #endregion
}
