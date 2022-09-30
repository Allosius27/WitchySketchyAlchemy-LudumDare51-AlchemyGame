using UnityEngine;
using System.Globalization;

public static class StringExtensions
{
    /// <summary>
    /// Converts the specified string from hex format ("FFFFFF") to <see cref="Color"/>. 
    /// </summary>
    public static Color ToColor(this string str)
    {
        int r = int.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
        int g = int.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
        int b = int.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}