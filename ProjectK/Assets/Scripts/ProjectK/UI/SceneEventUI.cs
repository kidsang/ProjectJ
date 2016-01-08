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
        private bool downed = false;
        private Vector3 downPosition;
        private Vector3 lastMovePosition;

        protected override void Init()
        {
            detail = GameObject.GetComponent<SceneEventUIDetail>();
            AddPointerClickHandler(detail._Image, OnPointerClick);
            AddPointerDownHandler(detail._Image, OnPointerDown);
            AddPointerUpHandler(detail._Image, OnPointerUp);
        }

        override protected void OnDispose()
        {
            RemovePointerClickHandler(detail._Image, OnPointerClick);
        }

        protected override void OnShow()
        {
            MoveBottom();
            EnableUpdate(true);
        }

        protected override void OnHide()
        {
            MoveBottom();
            EnableUpdate(false);
        }

        public override void OnUpdate()
        {
            if (!downed)
                return;

            Vector3 mousePosition = Input.mousePosition;
            Vector3 deltaPosition = mousePosition - lastMovePosition;

            Vector3 deltaWorldPosition = Camera.main.ScreenToWorldPoint(deltaPosition);
            SceneManager.Instance.Scene.Map.UpdateCameraPosition(deltaWorldPosition);

            lastMovePosition = mousePosition;
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            Map map = SceneManager.Instance.Scene.Map;
            MapCell cell = map.GetCellByMousePosition();
            if (cell == null)
                return;

            if (cell.TowerEntities.Count == 0)
            {
                BuildTowerUI ui = UIManager.Instance.CreateUI<BuildTowerUI>();
                ui.SetPosition(eventData.position);
                ui.Refresh(cell);
            }
            else
            {
                // TODO: 升级、卖塔之类的操作
            }
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            downed = true;
            downPosition = lastMovePosition = Input.mousePosition;
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            downed = false;
        }
    }
}
