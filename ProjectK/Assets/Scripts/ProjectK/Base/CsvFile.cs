// 是否开启CSV检查
#define CHECK_CSV

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ProjectK.Base
{
    public abstract class CsvFileObject : TextResourceUtils
    {

        public abstract object GetKey();

        public virtual void OnComplete()
        {
        }

        /// <summary>
        /// 仅当 #define CHECK_CSV 时调用
        /// </summary>
        public virtual void OnCheck()
        {
        }

        public void Check(bool condition)
        {
            if (!condition)
                Log.Error("Csv check failed! File:", this, "Key:", GetKey());
        }
    }

    public class CsvFile<T> : TextResource where T : CsvFileObject, new()
    {
        private static char[] LINE_SEPERATOR = new char[] { '\n', '\r' };
        private static char[] VALUE_SEPERATOR = new char[] { ',' };

        private bool parsed = false;
        private string rawData;
        private Dictionary<object, T> datas = new Dictionary<object, T>();

        internal override void Load()
        {
            base.Load();
            if (Text != null)
                LoadFromData(Text);
        }

        internal override void OnLoadAsync()
        {
            base.OnLoadAsync();
            if (Text != null)
                LoadFromData(Text);
        }

        public void LoadFromData(string rawData, bool parseImmediately = true)
        {
            this.rawData = rawData;
            if (parseImmediately)
                Parse();
        }

        public T GetValue(object key)
        {
            if (!parsed)
                Parse();

            T data;
            datas.TryGetValue(key, out data);
            return data;
        }

        public T GetValue(params object[] keys)
        {
            string key = CsvFileObject.BuildMultiKey(keys);
            return GetValue(key);
        }

        private void Parse()
        {
            if (rawData == null)
                return;

            string[] lines = rawData.Split(LINE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
            int numLines = lines.Length;
            if (numLines <= 1)
                return;

            string[] titles = lines[0].Split(VALUE_SEPERATOR);
            int numTitles = titles.Length;
            FieldInfo[] fields = new FieldInfo[numTitles];
            Type type = typeof(T);
            for (int i = 0; i < numTitles; ++i)
                fields[i] = type.GetField(titles[i]);

            for (int i = 1; i < numLines; ++i)
            {
                string line = lines[i];
                // 第一列字段为#的是注释行，跳过。
                if (line.StartsWith("#"))
                    continue;

                string[] values = line.Split(VALUE_SEPERATOR);
                Log.Assert(values.Length == numTitles);

                T obj = new T();
                for (int j = 0; j < numTitles; ++j)
                {
                    FieldInfo field = fields[j];
                    if (field == null)
                        continue;

                    string strVal = values[j];
                    if (String.IsNullOrEmpty(strVal))
                        continue;

                    Type fieldType = field.FieldType;
                    try
                    {
                        // 按照频率排序
                        if (fieldType == typeof(int))
                            field.SetValue(obj, int.Parse(strVal));
                        else if (fieldType == typeof(string))
                            field.SetValue(obj, strVal);
                        else if (fieldType == typeof(bool))
                            field.SetValue(obj, bool.Parse(strVal));
                        else if (fieldType == typeof(float))
                            field.SetValue(obj, float.Parse(strVal));
                        else if (fieldType.IsSubclassOf(typeof(Enum)))
                            field.SetValue(obj, Enum.Parse(fieldType, strVal));
                        else if (fieldType == typeof(uint))
                            field.SetValue(obj, uint.Parse(strVal));
                        else if (fieldType == typeof(double))
                            field.SetValue(obj, double.Parse(strVal));
                        else // for long,short,byte...
                            field.SetValue(obj, int.Parse(strVal));
                    }
                    catch (Exception)
                    {
                        Log.Assert(false, "解析tab表错误! url:", Url, "line:", i, "field:", field.Name, "value:", strVal);
                    }
                }

                object key = obj.GetKey();
                if (datas.ContainsKey(key))
                {
                    Log.Error("tab表键值重复！ url:", Url, "key:", key);
                }
                else
                {
                    datas[key] = obj;
                    obj.OnComplete();
#if CHECK_CSV
                    obj.OnCheck();
#endif
                }
            }

            parsed = true;
            rawData = null;
        }
    }
}
