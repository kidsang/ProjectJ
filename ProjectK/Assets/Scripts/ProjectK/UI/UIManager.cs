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
    public enum UILayer
    {
        /// <summary>
        /// 怪物血条，数字等
        /// </summary>
        LayerHud = 0,

        LayerLow,
        LayerMid,
        LayerTop,
        Count,
    }

    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance { get { return instance; } }

        private Dictionary<Type, UIBase> createdUIs = new Dictionary<Type, UIBase>();
        private Dictionary<UILayer, RectTransform> layers = new Dictionary<UILayer, RectTransform>();
        private List<UIBase> updatingUIs = new List<UIBase>();

        public static void Init()
        {
            if (instance != null)
                return;

            // init canvas
            GameObject gameObject = new GameObject("UIManager");
            DontDestroyOnLoad(gameObject);
            gameObject.layer = LayerMask.NameToLayer("UI");
            gameObject.AddComponent<RectTransform>();
            gameObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
            instance = gameObject.AddComponent<UIManager>();

            // init layers
            for (int i = 0; i < (int)UILayer.Count; ++i)
            {
                UILayer layer = (UILayer)i;
                GameObject layerObject = new GameObject(layer.ToString());
                RectTransform transform = layerObject.AddComponent<RectTransform>();
                transform.anchorMin = new Vector2(0, 0);
                transform.anchorMax = new Vector2(1, 1);
                transform.offsetMin = Vector2.zero;
                transform.offsetMax = Vector2.zero;
                transform.SetParent(gameObject.transform, false);
                instance.layers[layer] = transform;
            }

            // init event system
            gameObject = new GameObject("UIEventSystem");
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
            gameObject.AddComponent<TouchInputModule>();
        }

        public T CreateUI<T>(UILayer layer = UILayer.LayerMid, bool showAfterCreate = true) where T : UIBase, new()
        {
            Type type = typeof(T);
            UIBase ui;

            if (!createdUIs.TryGetValue(type, out ui))
            {
                ui = new T();
                createdUIs.Add(type, ui);
            }

            ui.SetUILayer(layer);
            if (showAfterCreate)
            {
                ui.MoveTop();
                ui.Show();
            }
            return (T)ui;
        }

        public T CreateHud<T>(bool showAfterCreate = true) where T : UIBase, new()
        {
            UIBase hud = new T();
            hud.SetUILayer(UILayer.LayerHud);

            if (showAfterCreate)
                hud.Show();

            return (T)hud;
        }

        public void RemoveUI<T>() where T : UIBase
        {
            Type type = typeof(T);
            UIBase ui;
            if (!createdUIs.TryGetValue(type, out ui))
                return;
            createdUIs.Remove(type);

            ui.Hide();
            ui.Dispose();
        }

        public void RemoveUI(UIBase ui)
        {
            Type type = ui.GetType();
            if (createdUIs.ContainsKey(type))
                createdUIs.Remove(type);

            ui.Hide();
            ui.Dispose();
        }

        public void AddUIToLayer(UIBase ui, UILayer uiLayer)
        {
            RectTransform transform = layers[uiLayer];
            ui.GameObject.transform.SetParent(transform, false);
        }

        public void AddUIToLayer(RectTransform uiTransform, UILayer uiLayer)
        {
            RectTransform transform = layers[uiLayer];
            uiTransform.SetParent(transform, false);
        }

        public void AddUpdateUI(UIBase ui)
        {
            if (!updatingUIs.Contains(ui))
                updatingUIs.Add(ui);
        }

        public void RemoveUpdateUI(UIBase ui)
        {
            updatingUIs.Remove(ui);
        }

        private void Update()
        {
            foreach (UIBase ui in updatingUIs)
            {
                ui.OnUpdate();
            }
        }
    }
}
