using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Condition : MonoBehaviour
{
    public UnityEvent conditionsevent;
    public GameObject conditionsObjTrue;
    public GameObject conditionsObjFalse;
    public TypeCondition typeCondition;
}

public enum TypeCondition
{
    Default,
    SelectOnUp,
    SelectOnDown,
    SelectOnLeft,
    SelectOnRight,
}
