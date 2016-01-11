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
        private ResourceLoader loader;
        private Dictionary<string, HudInfo> hudInfoDict = new Dictionary<string, HudInfo>();

        private void Start()
        {
            loader = new ResourceLoader();
        }

        private void Update()
        {
            Vector3 position = gameObject.transform.position;
            position = Camera.main.WorldToScreenPoint(position);

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

            if (loader != null)
            {
                loader = new ResourceLoader();
                loader = null;
            }
        }

        public HudInfo AddHud(string name, UIBase hud, Vector2 offset)
        {
            if (hudInfoDict.ContainsKey(name))
            {
                Log.Error("重复添加的Hud！ name:", name, "hud:", hud);
                return null;
            }

            HudInfo hudInfo = new HudInfo();
            hudInfo.Hud = hud;
            hudInfo.Offset = offset;
            hudInfoDict[name] = hudInfo;
            return hudInfo;
        }

        public void RemoveHud(string name)
        {
            HudInfo hudInfo;
            if (!hudInfoDict.TryGetValue(name, out hudInfo))
                return;

            hudInfo.Hud.Remove();
            hudInfoDict.Remove(name);
        }

        public HudInfo GetHud(string name)
        {
            HudInfo hudInfo;
            hudInfoDict.TryGetValue(name, out hudInfo);
            return hudInfo;
        }

        public void ShowHud(string name)
        {
            HudInfo hudInfo;
            if (!hudInfoDict.TryGetValue(name, out hudInfo))
                return;
            hudInfo.Hud.Show();
        }

        public void HideHud(string name)
        {
            HudInfo hudInfo;
            if (!hudInfoDict.TryGetValue(name, out hudInfo))
                return;
            hudInfo.Hud.Hide();
        }

        public void ShowHpBar(float hpPercent)
        {
            HudInfo hudInfo = GetHud("hp_bar");
            if (hudInfo == null)
            {
                UIBase hud = UIManager.Instance.CreateHud<HpBarUI>();
                hudInfo = AddHud("hp_bar", hud, new Vector2());
            }
            hudInfo.Hud.Show();
            hudInfo.Hud.Refresh(hpPercent);
        }

        public void HideHpBar()
        {
            HideHud("hp_bar");
        }

        public static HudManager GetHudManager(GameObject gameObject)
        {
            HudManager hudManager = gameObject.GetComponent<HudManager>();
            if (hudManager == null)
                hudManager = gameObject.AddComponent<HudManager>();
            return hudManager;
        }

        public class HudInfo
        {
            public UIBase Hud;
            public Vector2 Offset;
        }
    }
}