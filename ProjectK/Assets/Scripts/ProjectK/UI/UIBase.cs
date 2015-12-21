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
    public class UIBase : Disposable
    {
        public ResourceLoader Loader { get; private set; }
        public GameObject GameObject { get; private set; }

        public string Name { get; private set; }
        public bool Loaded { get; private set; }
        public bool Showing { get; private set; }

        public delegate void OnLoadCompleteCallback();
        private event OnLoadCompleteCallback onLoadCompleteCallbacks;

        public UIBase()
        {
            Name = GetType().Name;
            Loaded = false;
            Showing = false;

            Loader = new ResourceLoader();
            Loader.LoadPrefabAsync("UI/" + Name, OnLoadComplete);
        }

        private void OnLoadComplete(Resource res)
        {
            if (Disposed)
                return;

            if (res.LoadFailed)
            {
                UIManager.Instance.RemoveUI(this);
                return;
            }

            GameObject = (res as PrefabResource).Instantiate();
            Loaded = true;

            if (!Showing)
                GameObject.SetActive(false);

            onLoadCompleteCallbacks();
            onLoadCompleteCallbacks = null;
        }

        override protected void OnDispose()
        {
            if (Loader != null)
            {
                Loader.Dispose();
                Loader = null;
            }

            if (GameObject != null)
            {
                GameObject.Destroy(GameObject);
                GameObject = null;
            }

            onLoadCompleteCallbacks = null;
            Loaded = false;
            Showing = false;

            base.OnDispose();
        }

        /// <summary>
        /// 设置UI的层级
        /// </summary>
        public void SetUILayer(UILayer uiLayer)
        {
            if (Loaded)
            {
                UIManager.Instance.AddUIToLayer(this, uiLayer);
            }
            else
            {
                AddOnLoadCompleteCallback(() => SetUILayer(uiLayer));
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void Show(params object[] args)
        {
            if (Loaded)
            {
                GameObject.SetActive(true);
                OnShow(args);
            }
            else
            {
                AddOnLoadCompleteCallback(() => Show(args));
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public void Hide()
        {
            if (Loaded)
            {
                if (GameObject.activeSelf)
                {
                    GameObject.SetActive(false);
                    OnHide();
                }
            }
            else
            {
                AddOnLoadCompleteCallback(() => Hide());
            }
        }

        /// <summary>
        /// 将窗口移动到本层的最顶端
        /// </summary>
        public void MoveTop()
        {
            if (Loaded)
            {
                GameObject.transform.SetAsLastSibling();
            }
            else
            {
                AddOnLoadCompleteCallback(() => MoveTop());
            }
        }

        /// <summary>
        /// 在窗口显示的时候被调用
        /// </summary>
        public virtual void OnShow(object[] args)
        {

        }

        /// <summary>
        /// 在窗口隐藏的时候被调用
        /// </summary>
        public virtual void OnHide()
        {

        }

        public void AddOnLoadCompleteCallback(OnLoadCompleteCallback callback)
        {
            if (!Loaded)
                onLoadCompleteCallbacks += callback;
            else
                callback();
        }

        /// <summary>
        /// 从自身开始查找是否存在指定名字的UI
        /// </summary>
        public T FindUIObject<T>(string name, bool includeChildren = true, int maxDeep = -1) where T : UIBehaviour
        {
            return FindUIObject<T>(GameObject, name, includeChildren, maxDeep);
        }

        /// <summary>
        /// 从指定GameObject开始查找是否存在指定名字的UI
        /// </summary>
        public static T FindUIObject<T>(GameObject gameObject, string name, bool includeChildren = true, int maxDeep = -1) where T : UIBehaviour
        {
            if (gameObject.name == name)
            {
                T element = gameObject.GetComponent<T>();
                if (element != null)
                    return element;
            }

            if (includeChildren && maxDeep != 0)
            {
                Transform transform = gameObject.transform;
                int childCount = transform.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    T element = FindUIObject<T>(transform.GetChild(i).gameObject, name, includeChildren, maxDeep - 1);
                    if (element != null)
                        return element;
                }
            }

            return null;
        }

        /// <summary>
        /// 从指定GameObject开始查找是否存在指定名字的UI
        /// </summary>
        public static UIBehaviour FindUIObject(GameObject gameObject, Type type, string name, bool includeChildren = true, int maxDeep = -1)
        {
            if (gameObject.name == name)
            {
                UIBehaviour element = gameObject.GetComponent(type) as UIBehaviour;
                if (element != null)
                    return element;
            }

            if (includeChildren && maxDeep != 0)
            {
                Transform transform = gameObject.transform;
                int childCount = transform.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    UIBehaviour element = FindUIObject(transform.GetChild(i).gameObject, type, name, includeChildren, maxDeep - 1);
                    if (element != null)
                        return element;
                }
            }

            return null;
        }

        /// <summary>
        /// 从自身开始查找是否存在指定名字的GameObject
        /// </summary>
        public GameObject FindGameObject(string name)
        {
            return FindGameObject(GameObject, name);
        }

        /// <summary>
        /// 从指定GameObject开始查找是否存在指定名字的GameObject
        /// </summary>
        public static GameObject FindGameObject(GameObject parent, string name)
        {
            Transform transform = parent.transform;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                GameObject gameObject = transform.GetChild(i).gameObject;
                if (gameObject.name == name)
                    return gameObject;

                gameObject = FindGameObject(gameObject, name);
                if (gameObject.name != null)
                    return gameObject;
            }
            return null;
        }
    }
}
