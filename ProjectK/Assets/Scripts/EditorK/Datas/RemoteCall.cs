using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK;
using ProjectK.Base;
using UnityEngine;

namespace EditorK
{
    public class RemoteCall
    {
        private static RemoteCall instance = new RemoteCall();
        public static RemoteCall Instance { get { return instance; } }

        public string DataPath;
        public SceneSetting Data;
        public MapSetting MapData { get { return Data.Map; } }

        private ResourceLoader loader = new ResourceLoader();

        public void OnSceneDataUpdate(string jsonData, string evt, InfoMap infos)
        {
            Data = SimpleJson.DeserializeObject<SceneSetting>(jsonData);
            DataPath = (string)infos["path"];

            if (evt == EditorEvent.MAP_LOAD)
                GameEditor.Instance.LoadMap(Data, DataPath);

            EventManager.Instance.FireEvent(evt, infos);
        }

        public void OnClearMouseData()
        {
            EditorMouse.Instance.Clear();
        }

        public void OnSetTerrainMouseData(MapCellFlag flag, int size, bool erase)
        {
            TerrainFlagInfo info = TerrainFlagInfo.GetInfoByFlag(flag);
            GameObject preview = loader.LoadPrefab("Map/TerrainFillMark").Instantiate();
            preview.GetComponent<SpriteRenderer>().color = info.Color;
            float scale = (size - 0.5f) * Mathf.Sqrt(3);
            preview.transform.localScale = new Vector3(scale, scale);

            InfoMap infos = new InfoMap();
            infos["flag"] = flag;
            infos["size"] = size;
            infos["erase"] = erase;
            EditorMouse.Instance.Clear();
            EditorMouse.Instance.SetData(EditorMouseDataType.TerrainFill, infos, preview);
        }
    }
}
