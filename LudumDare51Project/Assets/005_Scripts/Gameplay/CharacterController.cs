using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Fields

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] public MemberCtrl headSlot, armsSlot, legsSlot, torsoSlot;

    [Required]
    [SerializeField] private Animator anim;

    #endregion

    #region Behaviour

    public void Fail()
    {
        anim.SetTrigger("isWrong");
    }

    public void Drink()
    {
        anim.SetTrigger("isRight");
    }

    public void Die()
    {
        anim.SetTrigger("isDead");
    }

    public bool CheckStateCharacter()
    {
        if(headSlot.NaturalState)
        {
            return true;
        }
        else if (armsSlot.NaturalState)
        {
            return true;
        }
        else if (legsSlot.NaturalState)
        {
            return true;
        }
        else if (torsoSlot.NaturalState)
        {
            return true;
        }

        return false;
    }

    #endregion
}
