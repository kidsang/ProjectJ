using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class RemoteCall
    {
        private static RemoteCall instance = new RemoteCall();
        public static RemoteCall Instance { get { return instance; } }

        public string DataPath;
        public SceneSetting Data;
        public MapSetting MapData { get { return Data.Map; } }

        public void OnSceneDataUpdate(string jsonData, string evt, InfoMap infos)
        {
            Data = SimpleJson.DeserializeObject<SceneSetting>(jsonData);
            DataPath = (string)infos["path"];

            if (evt == EditorEvent.MAP_LOAD)
                GameEditor.Instance.LoadMap(Data, DataPath);

            EventManager.Instance.FireEvent(evt, infos);
        }
    }
}
