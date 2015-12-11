using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public enum UILayer
    {
        Bottom = 0,
        Middle,
        Top,
        Count,
    }

    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance { get { return instance; } }

        private ResourceLoader loader = new ResourceLoader();
        private Dictionary<Type, UIBase> createdUIs = new Dictionary<Type, UIBase>();
        private Dictionary<UILayer, GameObject> layers = new Dictionary<UILayer, GameObject>();

        public static void Init()
        {
            if (instance != null)
                return;

            GameObject gameObject = new GameObject("UIManager");
            instance = gameObject.AddComponent<UIManager>();

            for (int i = 0; i < (int)UILayer.Count; ++i)
            {
                UILayer layer = (UILayer)i;
                GameObject layerObject = new GameObject(layer.ToString());
                layerObject.transform.SetParent(gameObject.transform);
                instance.layers[layer] = layerObject;
            }
        }

        public T CreateUI<T>(UILayer layer = UILayer.Middle, params object[] args) where T : UIBase
        {
            Type type = typeof(T);
            UIBase ui;

            if (!createdUIs.TryGetValue(type, out ui))
            {
                GameObject uiObject = loader.LoadPrefab("").Instantiate();
                ui = uiObject.GetComponent<T>();
                createdUIs.Add(type, ui);
                uiObject.transform.SetParent(layers[layer].gameObject.transform);
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
