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
    public class UIBase : DisposableBehaviour
    {
        /// <summary>
        /// 从自身开始查找是否存在指定名字的UI
        /// </summary>
        public T FindUIObject<T>(string name, bool includeChildren = true, int maxDeep = -1) where T : UIBehaviour
        {
            return FindUIObject<T>(gameObject, name, includeChildren, maxDeep);
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
        /// 显示窗口
        /// </summary>
        public void Show(params object[] args)
        {
            gameObject.SetActive(true);
            OnShow(args);
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                OnHide();
            }
        }

        /// <summary>
        /// 将窗口移动到本层的最顶端
        /// </summary>
        public void MoveTop()
        {
            gameObject.transform.SetAsLastSibling();
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
    }
}
