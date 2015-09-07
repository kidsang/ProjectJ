using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class RemoteDataProxy
    {
        private static RemoteDataProxy instance = new RemoteDataProxy();
        public static RemoteDataProxy Instance { get { return instance; } }

        public SceneSetting SceneData { get; private set; }
        public MapSetting MapData { get { return SceneData.Map; } }

        //--------------------
        // send

        public void Load(SceneSetting data, string path = null)
        {
            string jsonData = SimpleJson.SerializeObject(data);
            RemoteCallParams("Load", jsonData, path);
        }

        public void Undo()
        {
            RemoteCall("Undo");
        }

        public void Redo()
        {
            RemoteCall("Redo");
        }

        private void RemoteCall(string funcName, object[] args = null)
        {
            App.Instance.Net.RemoteCall(funcName, args);
        }

        private void RemoteCallParams(string funcName, params object[] args)
        {
            App.Instance.Net.RemoteCall(funcName, args);
        }

        //--------------------
        // recv
        public void OnSceneDataUpdate(string sceneDataJson, string evt, RemoteTable infos)
        {
            Log.Info("OnDataUpdate! event:", evt);
            SceneData = SimpleJson.DeserializeObject<SceneSetting>(sceneDataJson);
            EventManager.Instance.FireEvent(evt, infos);
        }
    }
}
