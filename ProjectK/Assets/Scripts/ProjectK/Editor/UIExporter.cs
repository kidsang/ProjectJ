using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading;
using System.Collections;
using Microsoft.CSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using ProjectK;
using ProjectK.Base;

namespace ProjectK.Editor
{
    public class UIExporter
    {
        /// <summary>
        /// 会被导出的变量类型
        /// </summary>
        public static readonly List<Type> ExportUIPriority = new List<Type>{
            typeof(Button),
            typeof(Text),
            typeof(Image),
        };

        /// <summary>
        /// 用以验证GameObject.name是否能成为合法的变量名
        /// </summary>
        public static readonly Regex ValidVaribleNameRegex = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");

        /// <summary>
        /// 导出UIDetail类名
        /// </summary>
        public static string GetUIClassName(string gameObjectName)
        {
            return gameObjectName + "Detail";
        }

        /// <summary>
        /// 导出UIDetail路径
        /// </summary>
        public static string GetUIScriptPath(string gameObjectName)
        {
            return "Assets/Scripts/ProjectK/UI/Details/" + gameObjectName + "Detail.cs";
        }

        /// <summary>
        /// 导出UIPrefab路径
        /// </summary>
        public static string GetUIPrefabName(string gameObjectName)
        {
            return "Assets/Resources/UI/" + gameObjectName + ".prefab";
        }

        public static readonly string TempFilePath = "~UIExportTemp";

        [MenuItem("ProjectK/导出选中UI")]
        [MenuItem("Assets/ProjectK/导出选中UI")]
    	public static void ExportUI()
    	{

            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog("导出UI失败", "请选则需要导出的GameObject", "确定");
                return;
            }

            string gameObjectName = Selection.activeGameObject.name;
            if (!ValidVaribleNameRegex.IsMatch(gameObjectName))
            {
                EditorUtility.DisplayDialog("导出UI失败", String.Format("{0} 不是一个有效的名称", gameObjectName), "确定");
                return;
            }

            EditorUtility.DisplayProgressBar("正在导出UI", "正在生成UI类，请勿随意操作...", 20);
            GameObject gameObject = GameObject.Instantiate(Selection.activeGameObject);

            // 查找所有要导出的变量
            Dictionary<string, UIBehaviour> uiObjectDict = new Dictionary<string, UIBehaviour>();
            FetchUIObjects(gameObject, uiObjectDict);
            List<UIObjectInfo> uiObjectInfos = new List<UIObjectInfo>();
            foreach (var pair in uiObjectDict)
            {
                UIObjectInfo info = new UIObjectInfo();
                info.Name = pair.Key;
                info.UIObject = pair.Value;
                uiObjectInfos.Add(info);
            }
            uiObjectInfos.Sort(new UIObjectInfoSorter());

            // 生成代码
            string detailClassName = GetUIClassName(gameObjectName);
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("// 自动生成，请勿手动修改");
            codeBuilder.AppendLine("using UnityEngine;");
            codeBuilder.AppendLine("using UnityEngine.UI;");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine("namespace ProjectK");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine(String.Format("    public class {0} : MonoBehaviour", detailClassName));
            codeBuilder.AppendLine("    {");
            foreach (UIObjectInfo info in uiObjectInfos)
            {
                string typeName = info.UIObject.GetType().Name;
                codeBuilder.AppendLine(String.Format("        public {0} {1};", typeName, info.Name));
            }
            codeBuilder.AppendLine("    }");
            codeBuilder.AppendLine("}");
            string code = codeBuilder.ToString();

            // 保存代码文件并Import
            string detailScriptPath = GetUIScriptPath(gameObjectName);
            try
            {
                if (File.Exists(detailScriptPath))
                    File.Delete(detailScriptPath);
                File.WriteAllText(detailScriptPath, code);

                if (File.Exists(TempFilePath))
                    File.Delete(TempFilePath);
                string tempInfo = gameObjectName + " " + gameObject.GetInstanceID();
                foreach (UIObjectInfo info in uiObjectInfos)
                    tempInfo += " " + info.Name + ":" + info.UIObject.GetInstanceID();
                File.WriteAllText(TempFilePath, tempInfo);

                AssetDatabase.ImportAsset(detailScriptPath);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("导出UI失败", e.Message, "确定");
                EditorUtility.ClearProgressBar();
                return;
            }

            EditorUtility.DisplayProgressBar("正在导出UI", "正在等待编辑器编译UI类，请勿随意操作...", 40);
    	}

        private static void FetchUIObjects(GameObject gameObject, Dictionary<string, UIBehaviour> uiObjectDict)
        {
            string name = gameObject.name;

            if (!uiObjectDict.ContainsKey(name))
            {
                if (name.StartsWith("_") && ValidVaribleNameRegex.IsMatch(name))
                {
                    for (int i = 0; i < ExportUIPriority.Count; ++i)
                    {
                        Type type = ExportUIPriority[i];
                        UIBehaviour uiObject = UIBase.FindUIObject(gameObject, type, name, false);
                        if (uiObject != null)
                        {
                            uiObjectDict[name] = uiObject;
                            break;
                        }
                    }
                }
            }

            Transform transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; ++i)
                FetchUIObjects(transform.GetChild(i).gameObject, uiObjectDict);
        }

        internal struct UIObjectInfo
        {
            public string Name;
            public UIBehaviour UIObject;
        }

        internal class UIObjectInfoSorter : IComparer<UIObjectInfo>
        {
            private AlphabeticalSort alphabeticalSort = new AlphabeticalSort();

            public int Compare(UIObjectInfo info1, UIObjectInfo info2)
            {
                Type type1 = info1.UIObject.GetType();
                Type type2 = info2.UIObject.GetType();
                if (type1 == type2)
                    return alphabeticalSort.Compare(info1.UIObject.gameObject, info2.UIObject.gameObject);
                return ExportUIPriority.IndexOf(type1) - ExportUIPriority.IndexOf(type2);
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            if (!File.Exists(TempFilePath))
                return;

            EditorUtility.DisplayProgressBar("正在导出UI", "正在绑定UI变量，请勿随意操作...", 80);

            string gameObjectName;
            int instanceID;
            List<string> fieldInfos;
            try
            {
                string text = File.ReadAllText(TempFilePath);
                File.Delete(TempFilePath);

                string[] infos = text.Split(' ');
                gameObjectName = infos[0];
                instanceID = int.Parse(infos[1]);

                fieldInfos = new List<string>(infos);
                fieldInfos.RemoveRange(0, 2);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("导出UI失败", e.Message, "确定");
                EditorUtility.ClearProgressBar();
                return;
            }

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null)
            {
                EditorUtility.DisplayDialog("导出UI失败", "找不到GameObject " + gameObjectName, "确定");
                EditorUtility.ClearProgressBar();
                return;
            }

            Type detailType = typeof(UIBase).Assembly.GetType("ProjectK." + GetUIClassName(gameObjectName));

            // 绑定变量
            var detailComp = gameObject.GetComponent(detailType);
            if (detailComp != null)
                GameObject.DestroyImmediate(detailComp);
            detailComp = gameObject.AddComponent(detailType);

            foreach (string fieldInfo in fieldInfos)
            {
                string[] infos = fieldInfo.Split(':');
                string fieldName = infos[0];
                object uiObject = EditorUtility.InstanceIDToObject(int.Parse(infos[1]));

                FieldInfo field = detailType.GetField(fieldName);
                field.SetValue(detailComp, uiObject);
            }

            // 保存Prefab
            string prefabPath = GetUIPrefabName(gameObjectName);
            PrefabUtility.CreatePrefab(prefabPath, gameObject);
            AssetDatabase.SaveAssets();

            GameObject.DestroyImmediate(gameObject);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("导出UI", "导出完成！", "确定");
        }
    }
}
