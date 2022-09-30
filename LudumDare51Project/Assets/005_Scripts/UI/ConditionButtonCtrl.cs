using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ConditionButtonCtrl : ButtonCtrl
{
    public bool selectOnUpCondition;
    [ShowIfGroup("selectOnUpCondition")]
    public Condition onUpCondition;

    public bool selectOnDownCondition;
    [ShowIfGroup("selectOnDownCondition")]
    public Condition onDownCondition;

    public bool selectOnLeftCondition;
    [ShowIfGroup("selectOnLeftCondition")]
    public Condition onLeftCondition;

    public bool selectOnRightCondition;
    [ShowIfGroup("selectOnRightCondition")]
    public Condition onRightCondition;


    public override void Start()
    {
        base.Start();

        if(onUpCondition != null)
        {
            onUpCondition.conditionsevent.Invoke();
        }

        if (onDownCondition != null)
        {
            onDownCondition.conditionsevent.Invoke();
        }

        if (onLeftCondition != null)
        {
            onLeftCondition.conditionsevent.Invoke();
        }

        if (onRightCondition != null)
        {
            onRightCondition.conditionsevent.Invoke();
        }
    }

    public void GetButtonIsInteractive(Condition condition)
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnUp = button.navigation.selectOnUp;
        newNav.selectOnDown = button.navigation.selectOnDown;
        newNav.selectOnLeft = button.navigation.selectOnLeft;
        newNav.selectOnRight = button.navigation.selectOnRight;

        Selectable conditionButtonTrue = condition.conditionsObjTrue.GetComponent<Selectable>();
        if (conditionButtonTrue.IsInteractable())
        {
            if (condition.typeCondition == TypeCondition.SelectOnUp)
            {
                newNav.selectOnUp = conditionButtonTrue;
                button.navigation = newNav;
            }
            else if (condition.typeCondition == TypeCondition.SelectOnDown)
            {
                newNav.selectOnDown = conditionButtonTrue;
                button.navigation = newNav;
            }
            else if (condition.typeCondition == TypeCondition.SelectOnLeft)
            {
                newNav.selectOnLeft = conditionButtonTrue;
                button.navigation = newNav;
            }
            else if (condition.typeCondition == TypeCondition.SelectOnRight)
            {
                newNav.selectOnRight = conditionButtonTrue;
                button.navigation = newNav;
            }
        }
        else
        {
            Selectable conditionButtonFalse = condition.conditionsObjFalse.GetComponent<Selectable>();
            if (condition.typeCondition == TypeCondition.SelectOnUp)
            {
                newNav.selectOnUp = conditionButtonFalse;
                button.navigation = newNav;
            }
            else if (condition.typeCondition == TypeCondition.SelectOnDown)
            {
                newNav.selectOnDown = conditionButtonFalse;
                button.navigation = newNav;
            }
            else if (condition.typeCondition == TypeCondition.SelectOnLeft)
            {
                newNav.selectOnLeft = conditionButtonFalse;
                button.navigation = newNav;
            }
            else if (condition.typeCondition == TypeCondition.SelectOnRight)
            {
                newNav.selectOnRight = conditionButtonFalse;
                button.navigation = newNav;
            }
        }
    }
}
