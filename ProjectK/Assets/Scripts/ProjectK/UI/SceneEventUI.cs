using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using ProjectK.Base;

namespace ProjectK
{
    public class SceneEventUI : UIBase
    {
        private SceneEventUIDetail detail;

        protected override void Init()
        {
            detail = GameObject.GetComponent<SceneEventUIDetail>();
            AddPointerClickHandler(detail._Image, OnPointerClick);
        }

        override protected void OnDispose()
        {
            RemovePointerClickHandler(detail._Image, OnPointerClick);
        }

        protected override void OnShow()
        {
            MoveBottom();
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            BuildTowerUI ui = UIManager.Instance.CreateUI<BuildTowerUI>();
            ui.SetPosition(eventData.position);
        }
    }
}
