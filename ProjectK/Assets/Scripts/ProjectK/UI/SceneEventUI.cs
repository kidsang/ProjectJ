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
        private Vector3 downMousePosition;
        private Vector3 lastMousePosition;
        private float clickMoveDelta = 10.0f;

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
            RemovePointerDownHandler(detail._Image, OnPointerDown);
            RemovePointerUpHandler(detail._Image, OnPointerUp);
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

            Camera camera = Camera.main;
            Vector3 mousePosition = Input.mousePosition;
            Vector3 deltaWorldPosition = camera.ScreenToWorldPoint(mousePosition) - camera.ScreenToWorldPoint(lastMousePosition);
            deltaWorldPosition *= -1;
            SceneManager.Instance.Scene.Map.UpdateCameraPosition(deltaWorldPosition);

            lastMousePosition = mousePosition;
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseDelta = mousePosition - downMousePosition;
            if (Mathf.Abs(mouseDelta.x) > clickMoveDelta || Mathf.Abs(mouseDelta.y) > clickMoveDelta)
                return;

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
            downMousePosition = lastMousePosition = Input.mousePosition;
            lastMousePosition = downMousePosition;
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            downed = false;
        }
    }
}
