using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IngredientData", menuName = "IngredientData")]
public class IngredientData : SerializedScriptableObject
{
    #region Fields

    protected const string LEFT_VERTICAL_GROUP = "Split/Left";
    protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings";

    protected const string RIGHT_VERTICAL_GROUP = "Split/Right";
    protected const string NOTES_GROUP = "Split/Right/Notes";

    #endregion

    #region UnityInspector

    [HideLabel, PreviewField(55)]
    [VerticalGroup(LEFT_VERTICAL_GROUP)]

    [HorizontalGroup(GENERAL_SETTINGS_VERTICAL_GROUP + "/Split", 55, LabelWidth = 67)]
    [SerializeField] public Sprite ingredientSprite;

    [BoxGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP + "/Split/Right")]
    public string Name;

    [VerticalGroup(RIGHT_VERTICAL_GROUP)]

    #region Notes

    [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
    [BoxGroup(NOTES_GROUP)]
    [HideLabel, TextArea(4, 9)]
    public string Notes;

    #endregion

    #endregion
}
