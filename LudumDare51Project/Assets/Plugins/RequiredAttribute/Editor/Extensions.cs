using UnityEngine;
using UnityEditor;

public static class SerializedPropertyExtensions
{
    public static bool IsDefaultValue(this SerializedProperty property)
    {
        // See https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/SerializedProperty.cs
        // for more info on this.

        switch (property.propertyType)
        {
            case SerializedPropertyType.AnimationCurve: return property.animationCurveValue == default(AnimationCurve);
            case SerializedPropertyType.ArraySize: return property.arraySize == default(int);
            case SerializedPropertyType.Boolean: return property.boolValue == default(bool);
            case SerializedPropertyType.Bounds: return property.boundsValue == default(Bounds);
            case SerializedPropertyType.BoundsInt: return property.boundsIntValue == default(BoundsInt);
            case SerializedPropertyType.Character: return property.intValue == default(char);
            case SerializedPropertyType.Color: return property.colorValue == default(Color);
            case SerializedPropertyType.Enum: return property.enumValueIndex == default(int);
            case SerializedPropertyType.ExposedReference: return property.exposedReferenceValue == default(Object);
            case SerializedPropertyType.FixedBufferSize: return property.fixedBufferSize == default(int);
            case SerializedPropertyType.Float: return property.floatValue == default(float);
            case SerializedPropertyType.Generic: break;
            case SerializedPropertyType.Gradient: break; // Couldn't find a property.gradientValue.
            case SerializedPropertyType.Integer: return property.intValue == default(int);
            case SerializedPropertyType.LayerMask: return property.intValue == default(int);
            case SerializedPropertyType.ObjectReference: return property.objectReferenceValue == default(Object);
            case SerializedPropertyType.Quaternion: return property.quaternionValue == default(Quaternion);
            case SerializedPropertyType.Rect: return property.rectValue == default(Rect);
            case SerializedPropertyType.RectInt: break; // Since RectInt doesn't implement IEquatable, we can do nothing here.
            case SerializedPropertyType.String: return property.stringValue == default(string);
            case SerializedPropertyType.Vector2: return property.vector2Value == default(Vector2);
            case SerializedPropertyType.Vector2Int: return property.vector2IntValue == default(Vector2Int);
            case SerializedPropertyType.Vector3: return property.vector3Value == default(Vector3);
            case SerializedPropertyType.Vector3Int: return property.vector3IntValue == default(Vector3Int);
            case SerializedPropertyType.Vector4: return property.vector4Value == default(Vector4);
            default: return true;
        }

        return true;
    }
}