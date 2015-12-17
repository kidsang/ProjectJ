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

        private ResourceLoader loader;
        private Dictionary<Type, UIBase> createdUIs = new Dictionary<Type, UIBase>();
        private Dictionary<UILayer, GameObject> layers = new Dictionary<UILayer, GameObject>();

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
            instance.loader = new ResourceLoader();

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
                layerObject.transform.SetParent(gameObject.transform, false);
                instance.layers[layer] = layerObject;
            }

            // init event system
            gameObject = new GameObject("UIEventSystem");
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
            gameObject.AddComponent<TouchInputModule>();
        }

        public T CreateUI<T>(UILayer layer = UILayer.LayerMid, params object[] args) where T : UIBase
        {
            Type type = typeof(T);
            UIBase ui;

            if (!createdUIs.TryGetValue(type, out ui))
            {
                GameObject uiObject = loader.LoadPrefab("UI/" + type.Name).Instantiate();
                ui = uiObject.GetComponent<T>();
                createdUIs.Add(type, ui);
                uiObject.transform.SetParent(layers[layer].gameObject.transform, false);
            }

            ui.MoveTop();
            ui.OnShow(args);
            return (T)ui;
        }

        public void RemoveUI<T>() where T : UIBase
        {
            Type type = typeof(T);
            UIBase ui;
            if (!createdUIs.TryGetValue(type, out ui))
                return;
            createdUIs.Remove(type);

            if (ui.gameObject.activeSelf)
                ui.OnHide();
            ui.Dispose();
        }

        public void RemoveUI(UIBase ui)
        {
            Type type = ui.GetType();
            if (createdUIs.ContainsKey(type))
                createdUIs.Remove(type);

            if (ui.gameObject.activeSelf)
                ui.OnHide();
            ui.Dispose();
        }
    }
}
