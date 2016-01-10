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
        public bool UpdateEnabled { get; private set; }

        public delegate void OnLoadCompleteCallback();
        private event OnLoadCompleteCallback onLoadCompleteCallbacks;

        public UIBase()
        {
            Name = GetType().Name;
            Loaded = false;
            Showing = false;
            UpdateEnabled = false;

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
            GameObject.name = Name;
            Loaded = true;

            if (!Showing)
                GameObject.SetActive(false);

            Init();

            if (onLoadCompleteCallbacks != null)
            {
                onLoadCompleteCallbacks();
                onLoadCompleteCallbacks = null;
            }
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

            if (UpdateEnabled)
            {
                UIManager.Instance.RemoveUpdateUI(this);
                UpdateEnabled = false;
            }

            onLoadCompleteCallbacks = null;
            Loaded = false;
            Showing = false;

            base.OnDispose();
        }

        /// <summary>
        /// 在窗口加载完成后被调用
        /// </summary>
        protected virtual void Init()
        {

        }

        /// <summary>
        /// 在窗口显示的时候被调用
        /// </summary>
        protected virtual void OnShow()
        {

        }

        /// <summary>
        /// 在窗口隐藏的时候被调用
        /// </summary>
        protected virtual void OnHide()
        {

        }

        /// <summary>
        /// 刷新窗口内容
        /// </summary>
        protected virtual void OnRefresh(params object[] args)
        {

        }

        /// <summary>
        /// 每帧调用，设置EnableUpdate()启用
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        public void AddOnLoadCompleteCallback(OnLoadCompleteCallback callback)
        {
            if (!Loaded)
                onLoadCompleteCallbacks += callback;
            else
                callback();
        }

        #region 各种异步接口

        /// <summary>
        /// 销毁窗口
        /// </summary>
        public void Remove()
        {
            UIManager.Instance.RemoveUI(this);
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
        public void Show()
        {
            if (Loaded)
            {
                if (!GameObject.activeSelf)
                {
                    GameObject.SetActive(true);
                    OnShow();
                }
            }
            else
            {
                AddOnLoadCompleteCallback(() => Show());
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
        /// 刷新窗口内容
        /// </summary>
        public void Refresh(params object[] args)
        {
            if (Loaded)
            {
                OnRefresh(args);
            }
            else
            {
                AddOnLoadCompleteCallback(() => Refresh(args));
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
        /// 将窗口移动到本层的最底端
        /// </summary>
        public void MoveBottom()
        {
            if (Loaded)
            {
                GameObject.transform.SetAsFirstSibling();
            }
            else
            {
                AddOnLoadCompleteCallback(() => MoveBottom());
            }
        }

        /// <summary>
        /// 设置窗口位置
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            if (Loaded)
            {
                GameObject.transform.position = position;
            }
            else
            {
                AddOnLoadCompleteCallback(() => SetPosition(position));
            }
        }

        /// <summary>
        /// 启用OnUpdate
        /// </summary>
        public void EnableUpdate(bool enable)
        {
            if (UpdateEnabled == enable)
                return;
            UpdateEnabled = enable;

            if (enable)
            {
                if (Loaded)
                    UIManager.Instance.AddUpdateUI(this);
                else
                    AddOnLoadCompleteCallback(() => UIManager.Instance.AddUpdateUI(this));
            }
            else
            {
                if (Loaded)
                    UIManager.Instance.RemoveUpdateUI(this);
                else
                    AddOnLoadCompleteCallback(() => UIManager.Instance.RemoveUpdateUI(this));
            }
        }

        #endregion

        #region 查找UI元件helpers

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

        #endregion

        #region 添加鼠标事件监听helpers

        public static void AddPointerClickHandler(UIBehaviour uiBehaviour, OnPointerEvent callback)
        {

            PointerHandler handler = GetPointerHandler(uiBehaviour);
            handler.PointerClickEvent += callback;
        }

        public static void RemovePointerClickHandler(UIBehaviour uiBehaviour, OnPointerEvent callback)
        {

            PointerHandler handler = GetPointerHandler(uiBehaviour);
            handler.PointerClickEvent -= callback;
        }

        public static void AddPointerDownHandler(UIBehaviour uiBehaviour, OnPointerEvent callback)
        {

            PointerHandler handler = GetPointerHandler(uiBehaviour);
            handler.PointerDownEvent += callback;
        }

        public static void RemovePointerDownHandler(UIBehaviour uiBehaviour, OnPointerEvent callback)
        {

            PointerHandler handler = GetPointerHandler(uiBehaviour);
            handler.PointerDownEvent -= callback;
        }

        public static void AddPointerUpHandler(UIBehaviour uiBehaviour, OnPointerEvent callback)
        {

            PointerHandler handler = GetPointerHandler(uiBehaviour);
            handler.PointerUpEvent += callback;
        }

        public static void RemovePointerUpHandler(UIBehaviour uiBehaviour, OnPointerEvent callback)
        {

            PointerHandler handler = GetPointerHandler(uiBehaviour);
            handler.PointerUpEvent -= callback;
        }

        private static PointerHandler GetPointerHandler(UIBehaviour uiBehaviour)
        {
            GameObject gameObject = uiBehaviour.gameObject;
            PointerHandler handler = gameObject.GetComponent<PointerHandler>();
            if (handler == null)
                handler = gameObject.AddComponent<PointerHandler>();
            return handler;
        }

        public delegate void OnPointerEvent(PointerEventData eventData);

        public class PointerHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
        {
            public event OnPointerEvent PointerClickEvent;
            public event OnPointerEvent PointerDownEvent;
            public event OnPointerEvent PointerUpEvent;

            public void OnPointerClick(PointerEventData eventData)
            {
                if (PointerClickEvent != null)
                    PointerClickEvent(eventData);
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                if (PointerDownEvent != null)
                    PointerDownEvent(eventData);
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                if (PointerUpEvent != null)
                    PointerUpEvent(eventData);
            }
        }

        #endregion
    }
}
