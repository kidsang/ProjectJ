using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ProjectK.Base
{
    public class TextResource : UnityResource<TextAsset>
    {
        public string Text
        {
            get 
            {
                if (_Data == null)
                    return null;
                return _Data.text;
            }
        }
    }

    public class TextResourceUtils
    {
        private static StringBuilder keyBuilder = new StringBuilder();

        public static string BuildMultiKey(params object[] keys)
        {
            keyBuilder.Remove(0, keyBuilder.Length);

            int end = keys.Length - 1;
            for (int i = 0; i <= end; ++i)
            {
                keyBuilder.Append(keys[i]);
                if (i < end)
                    keyBuilder.Append("-");
            }

            return keyBuilder.ToString();
        }

        #region 从字符串中解析出数组

        public static string[] ParseStrArray(string data, char seperator = ';')
        {
            if (data == null)
                return new string[0];

            return data.Split(seperator);
        }

        public static int[] ParseIntArray(string data, char seperator = ';')
        {
            if (data == null)
                return new int[0];

            string[] strArr = data.Split(seperator);
            int length = strArr.Length;

            int[] valArr = new int[length];
            for (int i = 0; i < length; ++i)
                valArr[i] = int.Parse(strArr[i]);

            return valArr;
        }

        public static float[] ParseFloatArray(string data, char seperator = ';')
        {
            if (data == null)
                return new float[0];

            string[] strArr = data.Split(seperator);
            int length = strArr.Length;

            float[] valArr = new float[length];
            for (int i = 0; i < length; ++i)
                valArr[i] = float.Parse(strArr[i]);

            return valArr;
        }

        public static double[] ParseDoubleArray(string data, char seperator = ';')
        {
            if (data == null)
                return new double[0];

            string[] strArr = data.Split(seperator);
            int length = strArr.Length;

            double[] valArr = new double[length];
            for (int i = 0; i < length; ++i)
                valArr[i] = double.Parse(strArr[i]);

            return valArr;
        }

        public static bool[] ParseBoolArray(string data, char seperator = ';')
        {
            if (data == null)
                return new bool[0];

            string[] strArr = data.Split(seperator);
            int length = strArr.Length;

            bool[] valArr = new bool[length];
            for (int i = 0; i < length; ++i)
                valArr[i] = bool.Parse(strArr[i]);

            return valArr;
        }

        public static T[] ParseEnumArray<T>(string data, char seperator = ';')
        {
            if (data == null)
                return new T[0];

            string[] strArr = data.Split(seperator);
            int length = strArr.Length;

            Type type = typeof(T);
            T[] valArr = new T[length];
            for (int i = 0; i < length; ++i)
                valArr[i] = (T)Enum.Parse(type, strArr[i]);

            return valArr;
        }

        #endregion

        #region 把具有连续Key的字段变成数组

        public T[] FieldsToArray<T>(string baseKey, int startIndex, int endIndex)
        {
            Type type = GetType();
            T[] retArr = new T[endIndex - startIndex + 1];

            for (int i = startIndex; i <= endIndex; ++i)
            {
                FieldInfo fieldInfo = type.GetField(baseKey + i);
                retArr[i] = (T)fieldInfo.GetValue(this);
            }

            return retArr;
        }

        #endregion
    }
}
