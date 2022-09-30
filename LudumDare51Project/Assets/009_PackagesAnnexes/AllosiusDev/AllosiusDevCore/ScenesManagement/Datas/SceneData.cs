using AllosiusDevCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SceneData", menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    #region UnityInspector

    public Scenes sceneToLoad;

    #endregion
}
