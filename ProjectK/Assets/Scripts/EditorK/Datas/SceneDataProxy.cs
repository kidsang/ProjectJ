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
        }

        public void Redo()
        {
            repo.Redo();
        }

        public void Load(SceneSetting data, string path = null)
        {
            DataPath = path;
            repo.New(data, EditorEvent.MAP_LOAD, null);
        }

        private void Modify(string evt, InfoMap infos)
        {
            repo.Modify(evt, infos);
            GameEditor.Instance.FileModified = true;
        }

        public int AddPath(int locationX, int locationY)
        {
            List<MapPathSetting> paths = new List<MapPathSetting>(MapData.Paths);
            MapPathSetting path = new MapPathSetting();
            path.ColorR = Random.value;
            path.ColorG = Random.value;
            path.ColorB = Random.value;
            MapWaypointSetting waypoint = new MapWaypointSetting();
            waypoint.X = locationX;
            waypoint.Y = locationY;
            path.Waypoints = new MapWaypointSetting[] { waypoint };
            paths.Add(path);
            MapData.Paths = paths.ToArray();

            Modify(EditorEvent.MAP_UPDATE_PATHS, null);
            return paths.Count - 1;
        }

        public void RemovePath(int index)
        {
            List<MapPathSetting> paths = new List<MapPathSetting>(MapData.Paths);
            paths.RemoveAt(index);
            MapData.Paths = paths.ToArray();

           Modify(EditorEvent.MAP_UPDATE_PATHS, null);
        }

        public void SwapPath(int index1, int index2)
        {
            List<MapPathSetting> paths = new List<MapPathSetting>(MapData.Paths);
            MapPathSetting temp = paths[index1];
            paths[index1] = paths[index2];
            paths[index2] = temp;
            MapData.Paths = paths.ToArray();

            Modify(EditorEvent.MAP_UPDATE_PATHS, null);
        }

        public void SetPathPoint(int pathIndex, int? pointIndex, int locationX, int locationY)
        {
            MapPathSetting path = MapData.Paths[pathIndex];
            MapWaypointSetting waypoint = new MapWaypointSetting();
            waypoint.X = locationX;
            waypoint.Y = locationY;

            if (pointIndex != null)
            {
                path.Waypoints[pointIndex.Value] = waypoint;
            }
            else
            {
                var waypoints = path.Waypoints.ToList<MapWaypointSetting>();
                waypoints.Add(waypoint);
                path.Waypoints = waypoints.ToArray();
            }

            InfoMap infos = new InfoMap();
            infos["index"] = pathIndex;
            Modify(EditorEvent.MAP_UPDATE_PATH, infos);
        }

        public void RemovePathPoint(int pathIndex, int pointIndex)
        {
            MapPathSetting path = MapData.Paths[pathIndex];
            if (path.Waypoints.Length == 1)
            {
                RemovePath(pathIndex);
                return;
            }

            var waypoints = path.Waypoints.ToList<MapWaypointSetting>();
            waypoints.RemoveAt(pointIndex);
            path.Waypoints = waypoints.ToArray();

            InfoMap infos = new InfoMap();
            infos["index"] = pathIndex;
            Modify(EditorEvent.MAP_UPDATE_PATH, infos);
        }

        public void SetPathColor(int index, float colorR, float colorG, float colorB)
        {
            MapPathSetting path = MapData.Paths[index];
            path.ColorR = colorR;
            path.ColorG = colorG;
            path.ColorB = colorB;

            InfoMap infos = new InfoMap();
            infos["index"] = index;
            Modify(EditorEvent.MAP_UPDATE_PATH, infos);
        }

        public void SetTerrainFlag(int x, int y, int radius, MapCellFlag flag, bool apply)
        {
            Dictionary<int, MapCellSetting> cellSettings = MapUtils.ArrayToDict(MapData.Cells);
            EditorMap map = GameEditor.Instance.Map;
            Vector2[] locations = MapUtils.Circle(x, y, radius);
            foreach (Vector2 location in locations)
            {
                MapCell cell = map.GetCell(location);
                if (cell == null)
                    continue;

                if (apply)
                {
                    MapCellSetting cellSetting;
                    if (!cellSettings.TryGetValue(cell.Key, out cellSetting))
                    {
                        cellSetting = new MapCellSetting();
                        cellSetting.X = cell.X;
                        cellSetting.Y = cell.Y;
                        cellSettings.Add(cell.Key, cellSetting);
                    }
                    EditorUtils.SetFlag(ref cellSetting.Flags, (int)flag, apply);
                }
                else
                {
                    MapCellSetting cellSetting;
                    if (cellSettings.TryGetValue(cell.Key, out cellSetting))
                    {
                        EditorUtils.SetFlag(ref cellSetting.Flags, (int)flag, apply);
                        if (cellSetting.Flags == 0)
                            cellSettings.Remove(cell.Key);
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
