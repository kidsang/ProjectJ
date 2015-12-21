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
        LayerLow = 0,
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

        public T CreateUI<T>(UILayer layer = UILayer.LayerMid, bool showAfterCreate = true, params object[] args) where T : UIBase, new()
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
                ui.Show(args);
                ui.MoveTop();
            }
            return (T)ui;
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
    }
}
