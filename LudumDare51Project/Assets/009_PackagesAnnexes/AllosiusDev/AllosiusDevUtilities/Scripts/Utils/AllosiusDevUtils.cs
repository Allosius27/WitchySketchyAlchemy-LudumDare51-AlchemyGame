//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevUtilities.Utils
{
    public static class AllosiusDevUtils
    {
        #region Fields

        #region IntUtilFields

        private static System.Random random;

        #endregion

        #endregion

        #region Behaviour

        #region IntUtil

        private static void InitRandom()
        {
            if (random == null) random = new System.Random();
        }

        public static int RandomGeneration(int min, int max)
        {
            InitRandom();
            return random.Next(min, max);
        }

        public static List<T> RandomizeList<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            while (list.Count > 0)
            {
                int index = RandomGeneration(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        #endregion

        #region GameHandler

        private static int HexToDec(string hex)
        {
            int dec = System.Convert.ToInt32(hex, 16);
            return dec;
        }

        private static string DecToHex(int value)
        {
            return value.ToString("X2");
        }

        private static string FloatNormalizedToHex(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255f));
        }

        private static float HexToFloatNormalized(string hex)
        {
            return HexToDec(hex) / 255f;
        }

        public static Color GetColorFromString(string hexString)
        {
            float red = HexToFloatNormalized(hexString.Substring(0, 2));
            float green = HexToFloatNormalized(hexString.Substring(2, 2));
            float blue = HexToFloatNormalized(hexString.Substring(4, 2));
            float alpha = 1f;
            if (hexString.Length >= 8)
            {
                alpha = HexToFloatNormalized(hexString.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }

        public static string GetStringFromColor(Color color, bool useAlpha = false)
        {
            string red = FloatNormalizedToHex(color.r);
            string green = FloatNormalizedToHex(color.g);
            string blue = FloatNormalizedToHex(color.b);
            if (!useAlpha)
            {
                return red + green + blue;
            }
            else
            {
                string alpha = FloatNormalizedToHex(color.a);
                return red + green + blue + alpha;
            }
        }

        #endregion

        #endregion
    }
}