using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK.UI
{
    public class PathPanel : DisposableBehaviour
    {
        public Transform Content;
        public GameObject EntryPrefab;
        public Image AddPathButton;

        private bool initialized = false;
        private bool operating = false;

        void Start()
        {
            EventManager.Instance.Register(this, EditorEvent.MAP_LOAD, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATHS, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATH, OnUpdatePath);

            if (!initialized)
            {
                initialized = true;
                if (SceneDataProxy.Instance.Data != null)
                    OnUpdatePaths(null);
            }
        }

        void OnEnable()
        {
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_CLICK, OnSceneMouseClick);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnSceneMouseRightClick);
        }

        void OnDisable()
        {
            EventManager.Instance.Unregister(this, EditorEvent.SCENE_MOUSE_CLICK, OnSceneMouseClick);
            EventManager.Instance.Unregister(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnSceneMouseRightClick);

            if (operating)
                StopOperate();
        }

        public void OnAddPathButtonClick()
        {
            StartOperate();
        }

        private void OnSceneMouseClick(object[] args)
        {
            if (!operating)
                return;

            EditorMouse mouse = EditorMouse.Instance;
            if (mouse.DataType != EditorMouseDataType.MapPath)
                return;

            InfoMap data = mouse.Data as InfoMap;
            int? pathIndex = (int?)data["pathIndex"];
            int? pointIndex = (int?)data["pointIndex"];

            if (pathIndex == null)
                pathIndex = SceneDataProxy.Instance.AddPath(mouse.SelectedLocationX, mouse.SelectedLocationY);
            else
                SceneDataProxy.Instance.SetPathPoint(pathIndex.Value, pointIndex, mouse.SelectedLocationX, mouse.SelectedLocationY);

            if (pointIndex != null)
                StopOperate();
            else
                OperateSetPoint(pathIndex.Value, null);
        }

        private void OnSceneMouseRightClick(object[] args)
        {
            if (operating)
                StopOperate();
        }

        private void StartOperate()
        {
            if (operating)
                return;

            operating = true;
            AddPathButton.color = EditorUtils.SelectedColor;
            EditorMouse mouse = EditorMouse.Instance;

            InfoMap data = new InfoMap();
            data["pathIndex"] = null;
            data["pointIndex"] = null;
            mouse.SetData(EditorMouseDataType.MapPath, data, "Map/StartMark");
        }

        private void StopOperate()
        {
            if (!operating)
                return;

            operating = false;
            AddPathButton.color = Color.white;
            EditorMouse mouse = EditorMouse.Instance;
            mouse.Clear();
        }

        private void OperateSetPoint(int pathIndex, int? pointIndex)
        {
            operating = true;
            AddPathButton.color = EditorUtils.SelectedColor;
            EditorMouse mouse = EditorMouse.Instance;

            InfoMap data = new InfoMap();
            data["pathIndex"] = pathIndex;
            data["pointIndex"] = pointIndex;
            mouse.SetData(EditorMouseDataType.MapPath, data, pointIndex == 0 ? "Map/StartMark" : "Map/EndMark");
        }

        private void OperateRemovePoint(int pathIndex, int? pointIndex)
        {
            SceneDataProxy.Instance.RemovePathPoint(pathIndex, pointIndex.Value);
        }

        private void OnUpdatePaths(object[] args)
        {
            MapSetting data = SceneDataProxy.Instance.MapData;
            int numPathDatas = data.Paths.Length;
            int numEntryObjs = Content.childCount - 1;

            for (int i = 0; i < numPathDatas; ++i)
            {
                GameObject entryObj;
                if (i < numEntryObjs)
                {
                    entryObj = Content.GetChild(i).gameObject;
                }
                else
                {
                    entryObj = Instantiate(EntryPrefab) as GameObject;
                    entryObj.transform.SetParent(Content);
                    entryObj.transform.SetSiblingIndex(Content.childCount - 2);
                }

                PathEntry entry = entryObj.GetComponent<PathEntry>();
                MapPathSetting pathData = data.Paths[i];
                entry.Load(i, pathData, OperateSetPoint, OperateRemovePoint);
            }

            for (int i = numPathDatas; i < numEntryObjs; ++i)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
        }

        private void OnUpdatePath(object[] args)
        {
            var infos = EditorUtils.GetEventInfos(args);
            int index = (int)infos["index"];

            MapSetting data = SceneDataProxy.Instance.MapData;
            GameObject entryObj = Content.GetChild(index).gameObject;
            PathEntry entry = entryObj.GetComponent<PathEntry>();
            entry.Load(index, data.Paths[index], OperateSetPoint, OperateRemovePoint);
        }
    }
}
