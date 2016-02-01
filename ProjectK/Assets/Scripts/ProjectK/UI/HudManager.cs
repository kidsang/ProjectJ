using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectK.Base;

namespace ProjectK
{
    public class HudManager : MonoBehaviour
    {
        private Dictionary<string, HudInfo> hudInfoDict = new Dictionary<string, HudInfo>();
        private Transform hudTransform;

        private void Start()
        {
            hudTransform = gameObject.transform;
            int childCount = hudTransform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                if (hudTransform.GetChild(i).gameObject.name == "Hud")
                {
                    hudTransform = hudTransform.GetChild(i);
                    break;
                }
            }
        }

        private void Update()
        {
            Vector3 position = Camera.main.WorldToScreenPoint(hudTransform.position);
            foreach (HudInfo hudInfo in hudInfoDict.Values)
            {
                UIBase hud = hudInfo.Hud;
                Vector2 offset = hudInfo.Offset;
                hud.SetPosition(new Vector2(position.x + offset.x, position.y + offset.y));
            }
        }

        private void OnDestroy()
        {
            if (hudInfoDict != null)
            {
                foreach (HudInfo hudInfo in hudInfoDict.Values)
                    hudInfo.Hud.Remove();
                hudInfoDict = null;
            }
        }

        /// <summary>
        /// 添加一个Hud
        /// </summary>
        public void AddHud(string name, UIBase hud, Vector2 offset)
        {
            if (hudInfoDict.ContainsKey(name))
            {
                Log.Error("重复添加的Hud！ name:", name, "hud:", hud);
                return;
            }

            HudInfo hudInfo = new HudInfo();
            hudInfo.Hud = hud;
            hudInfo.Offset = offset;
            hudInfoDict[name] = hudInfo;
        }

        /// <summary>
        /// 删除指定名称的Hud
        /// </summary>
        public void RemoveHud(string name)
        {
            HudInfo hudInfo;
            if (!hudInfoDict.TryGetValue(name, out hudInfo))
                return;

            hudInfo.Hud.Remove();
            hudInfoDict.Remove(name);
        }

        /// <summary>
        /// 获取指定名称的Hud
        /// </summary>
        public UIBase GetHud(string name)
        {
            HudInfo hudInfo;
            hudInfoDict.TryGetValue(name, out hudInfo);
            if (hudInfo == null)
                return null;
            return hudInfo.Hud;
        }

        /// <summary>
        /// 显示指定名称的Hud
        /// </summary>
        public void ShowHud(string name)
        {
            HudInfo hudInfo;
            if (!hudInfoDict.TryGetValue(name, out hudInfo))
                return;
            hudInfo.Hud.Show();
        }

        /// <summary>
        /// 隐藏名称的Hud
        /// </summary>
        public void HideHud(string name)
        {
            HudInfo hudInfo;
            if (!hudInfoDict.TryGetValue(name, out hudInfo))
                return;
            hudInfo.Hud.Hide();
        }

        /// <summary>
        /// 获取GameObject上的HudManager，如果没有则创建
        /// </summary>
        public static HudManager GetHudManager(GameObject gameObject)
        {
            HudManager hudManager = gameObject.GetComponent<HudManager>();
            if (hudManager == null)
                hudManager = gameObject.AddComponent<HudManager>();
            return hudManager;
        }

        class HudInfo
        {
            public UIBase Hud;
            public Vector2 Offset;
        }
    }
}