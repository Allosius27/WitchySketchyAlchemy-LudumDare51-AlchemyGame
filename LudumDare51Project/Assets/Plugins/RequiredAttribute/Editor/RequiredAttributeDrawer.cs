using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredAttributeDrawer : PropertyDrawer
{
    private const int margin = 15;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Color guiColor = GUI.color;

        if (property.IsDefaultValue())
        {
            RequiredAttribute attribute = fieldInfo.GetCustomAttributes(typeof(RequiredAttribute), false)[0] as RequiredAttribute;
            GUI.color = attribute.Color;
        }

        EditorGUI.PropertyField(position, property, label, true);

        GUI.color = guiColor;

        EditorGUI.EndProperty();
    }
}