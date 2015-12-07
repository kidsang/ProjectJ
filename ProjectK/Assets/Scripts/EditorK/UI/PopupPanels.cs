using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorK.UI
{
    public class PopupPanels : MonoBehaviour
    {
        private static PopupPanels instance;
        public static PopupPanels Instance { get { return instance; } }

        public GameObject ModalMask;
        public NewMapPopup NewMapPopup;

        private Dictionary<Type, PopupPanel> popupDict = new Dictionary<Type, PopupPanel>();
        private List<PopupPanel> popupList = new List<PopupPanel>();

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个PopupPanels实例！");

            popupDict[typeof(NewMapPopup)] = NewMapPopup;
        }

        public void ShowPopup<T>() where T: PopupPanel
        {
            PopupPanel popup = popupDict[typeof(T)];
            if (popupList.Contains(popup))
                popupList.Remove(popup);
            popupList.Add(popup);

            popup.gameObject.SetActive(true);
            popup.OnShow();
            LayoutPopups();
        }

        public void HidePopup<T>() where T : PopupPanel
        {
            PopupPanel popup = popupDict[typeof(T)];
            HidePopup(popup);
        }

        public void HidePopup(PopupPanel popup)
        {
            if (popupList.Contains(popup))
                popupList.Remove(popup);

            popup.gameObject.SetActive(false);
            popup.OnHide();
            LayoutPopups();
        }

        private void LayoutPopups()
        {
            for (int i = 0; i < popupList.Count; ++i)
            {
                PopupPanel popup = popupList[i];
                popup.transform.SetSiblingIndex(i);
            }

            if (popupList.Count == 0)
            {
                ModalMask.gameObject.SetActive(false);
            }
            else
            {
                ModalMask.gameObject.SetActive(true);
                ModalMask.transform.SetSiblingIndex(popupList.Count - 1);
            }
        }
    }
}
