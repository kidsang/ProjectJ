using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class SceneDataProxy
    {
        private static SceneDataProxy instance = new SceneDataProxy();
        public static SceneDataProxy Instance { get { return instance; } }

        public string DataPath { get; private set; }
        public SceneSetting Data { get { return repo.Data; } }
        public MapSetting MapData { get { return Data.Map; } }

        private DataRepository<SceneSetting> repo = new DataRepository<SceneSetting>();

        public bool Recording
        {
            get { return repo.Recording; }
            set { repo.Recording = value; }
        }

        public void Undo()
        {
            repo.Undo();
            UpdateRemoteData();
        }

        public void Redo()
        {
            repo.Redo();
            UpdateRemoteData();
        }

        public void Load(SceneSetting data, string path = null)
        {
            DataPath = path;
            InfoMap infos = new InfoMap();
            infos["path"] = path;
            repo.New(data, EditorEvent.MAP_LOAD, infos);
            UpdateRemoteData();
        }

        private void Modify(string evt, InfoMap infos)
        {
            repo.Modify(evt, infos);
            UpdateRemoteData();
        }

        private void UpdateRemoteData()
        {
            string evt = repo.CurrentEvt;
            InfoMap infos = repo.CurrentInfos;
            string sceneDataJson = SimpleJson.SerializeObject(Data);

            App.Instance.Net.RemoteCallParams("OnSceneDataUpdate", evt, infos, sceneDataJson);
        }

    //    public void AddPath(int startX, int startY, int endX, int endY)
    //    {
    //        // TODO:
    //        //List<MapPathSetting> paths = new List<MapPathSetting>(Data.Paths);
    //        //MapPathSetting path = new MapPathSetting();
    //        //path.StartX = startX;
    //        //path.StartY = startY;
    //        //path.EndX = endX;
    //        //path.EndY = endY;
    //        //path.ColorR = Random.value;
    //        //path.ColorG = Random.value;
    //        //path.ColorB = Random.value;
    //        //paths.Add(path);
    //        //Data.Paths = paths.ToArray();

    //        //Modify(EditorEvent.MAP_UPDATE_PATHS, null);
    //    }

    //    public void RemovePath(int index)
    //    {
    //        List<MapPathSetting> paths = new List<MapPathSetting>(MapData.Paths);
    //        paths.RemoveAt(index);
    //        MapData.Paths = paths.ToArray();

    //       Modify(EditorEvent.MAP_UPDATE_PATHS, null);
    //    }

    //    public void SwapPath(int index1, int index2)
    //    {
    //        List<MapPathSetting> paths = new List<MapPathSetting>(MapData.Paths);
    //        MapPathSetting temp = paths[index1];
    //        paths[index1] = paths[index2];
    //        paths[index2] = temp;
    //        MapData.Paths = paths.ToArray();

    //        Modify(EditorEvent.MAP_UPDATE_PATHS, null);
    //    }

    //    public void SetPathStart(int index, int startX, int startY)
    //    {
    //        // TODO:
    //        //MapPathSetting path = Data.Paths[index];
    //        //path.StartX = startX;
    //        //path.StartY = startY;

    //        //InfoMap infos = new InfoMap();
    //        //infos["index"] = index;
    //        //Modify(EditorEvent.MAP_UPDATE_PATH, infos);
    //    }

    //    public void SetPathEnd(int index, int endX, int endY)
    //    {
    //        // TODO:
    //        //MapPathSetting path = Data.Paths[index];
    //        //path.EndX = endX;
    //        //path.EndY = endY;

    //        //InfoMap infos = new InfoMap();
    //        //infos["index"] = index;
    //        //Modify(EditorEvent.MAP_UPDATE_PATH, infos);
    //    }

    //    public void SetPathColor(int index, float colorR, float colorG, float colorB)
    //    {
    //        MapPathSetting path = MapData.Paths[index];
    //        path.ColorR = colorR;
    //        path.ColorG = colorG;
    //        path.ColorB = colorB;

    //        InfoMap infos = new InfoMap();
    //        infos["index"] = index;
    //        Modify(EditorEvent.MAP_UPDATE_PATH, infos);
    //    }

        public void SetTerrainFlag(int x, int y, int radius, MapCellFlag flag, bool apply)
        {
            Dictionary<int, MapCellSetting> cellSettings = MapUtils.ArrayToDict(MapData.Cells);
            //EditorMap map = GameEditor.Instance.Map;
            Vector2[] locations = MapUtils.Circle(x, y, radius);
            foreach (Vector2 location in locations)
            {
                //MapCell cell = map.GetCell(location);
                //if (cell == null)
                //    continue;

                int cellKey = MapUtils.MakeKey((short)x, (short)y);
                if (apply)
                {
                    MapCellSetting cellSetting;
                    if (!cellSettings.TryGetValue(cellKey, out cellSetting))
                    {
                        cellSetting = new MapCellSetting();
                        cellSetting.X = x;
                        cellSetting.Y = y;
                        cellSettings.Add(cellKey, cellSetting);
                    }
                    EditorUtils.SetFlag(ref cellSetting.Flags, (int)flag, apply);
                }
                else
                {
                    MapCellSetting cellSetting;
                    if (cellSettings.TryGetValue(cellKey, out cellSetting))
                    {
                        EditorUtils.SetFlag(ref cellSetting.Flags, (int)flag, apply);
                        if (cellSetting.Flags == 0)
                            cellSettings.Remove(cellKey);
                    }
                }
            }

            MapData.Cells = MapUtils.DictToArray(cellSettings);

            InfoMap infos = new InfoMap();
            infos["flag"] = flag;
            Modify(EditorEvent.MAP_TERRAIN_UPDATE, infos);
        }
    }
}
