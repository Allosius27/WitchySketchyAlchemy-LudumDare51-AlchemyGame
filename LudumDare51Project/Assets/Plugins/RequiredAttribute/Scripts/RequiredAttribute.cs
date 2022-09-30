using System;
using UnityEngine;

/// <summary>
/// If this field has its default value, its label will be coloured in red (or another color if you specify it).
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class RequiredAttribute : PropertyAttribute
{
    private Color color = new Color(1f, 0.25f, 0.25f); // Default color is pale red
    public Color Color { get { return color; } }

    public RequiredAttribute() { }

    /// <summary>
    /// Don't write the '#'
    /// </summary>
    public RequiredAttribute(string hexColor)
    {
        color = hexColor.ToColor();
    }

    // Since Color is a nullable type, it's only possible to give the color as an hex string. Sorry!

}