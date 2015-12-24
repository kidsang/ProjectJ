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
    public class PathEntry : MonoBehaviour
    {
        public Image ColorField;
        public RectTransform WaypointGroup;
        public GameObject WaypointEntryPrefab;

        private int pathIndex;

        public delegate void Operate(int pathIndex, int? pointIndex);
        private Operate operateSet;
        private Operate operateRemove;

        public void Load(int pathIndex, MapPathSetting data, Operate operateSet, Operate operateRemove)
        {
            this.pathIndex = pathIndex;
            this.operateSet = operateSet;
            this.operateRemove = operateRemove;

            ColorField.color = new Color(data.ColorR, data.ColorG, data.ColorB);

            for (int i = WaypointGroup.transform.childCount - 1; i >= 0; --i)
                GameObject.DestroyImmediate(WaypointGroup.transform.GetChild(i).gameObject);

            int lineHeight = 24;
            for (int i = 0; i < data.Waypoints.Length; ++i)
            {
                MapWaypointSetting waypointSetting = data.Waypoints[i];
                GameObject WaypointEntry = GameObject.Instantiate(WaypointEntryPrefab);
                WaypointEntry.transform.SetParent(WaypointGroup.transform, false);
                WaypointEntry.transform.localPosition = new Vector3(0, -i * lineHeight);
                UIBase.FindUIObject<Text>(WaypointEntry, "Text").text = string.Format("{0}：({1}, {2})", i, waypointSetting.X, waypointSetting.Y);
                UIBase.FindUIObject<Button>(WaypointEntry, "ResetButton").onClick.AddListener(OnResetPoint(i));
                UIBase.FindUIObject<Button>(WaypointEntry, "RemoveButton").onClick.AddListener(OnRemovePoint(i));
            }
            gameObject.GetComponent<LayoutElement>().minHeight = lineHeight * data.Waypoints.Length + 12;
        }

        public UnityEngine.Events.UnityAction OnResetPoint(int pointIndex)
        {
            return () => operateSet(pathIndex, pointIndex);
        }

        public UnityEngine.Events.UnityAction OnRemovePoint(int pointIndex)
        {
            return () => operateRemove(pathIndex, pointIndex);
        }

    }
}
