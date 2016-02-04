﻿// 六边形格子尖头向上
#define POINTY_TOPPED

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;
using Vectrosity;

namespace ProjectK
{

    /**
     * 左上角为地图的起始点，其中x = 0，y = 0。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class Map : DisposableBehaviour
    {
        protected Transform MapRoot { get; private set; }
        protected Transform CellRoot { get; private set; }
        protected Transform PathRoot { get; private set; }
        public ResourceLoader Loader { get; private set; }

        public string Name { get; protected set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public int CellCountX { get; protected set; }
        public int CellCountY { get; protected set; }

        public Dictionary<int, MapCell> Cells { get; protected set; }
        public List<Color> PathColors { get; protected set; }
        public List<GameObject> PathObjectRoots { get; protected set; }
        public List<MapPath> Paths { get; protected set; }
        protected HashSet<int> cantBlockCells = new HashSet<int>(); // 这些位置不能放置阻挡，否则就会把路堵死

        internal void Init(ResourceLoader loader)
        {
            MapRoot = gameObject.transform;
            CellRoot = new GameObject("CellRoot").transform;
            CellRoot.SetParent(MapRoot, false);
            PathRoot = new GameObject("PathRoot").transform;
            PathRoot.SetParent(MapRoot, false);

            Loader = loader;
            Cells = new Dictionary<int, MapCell>();
            PathColors = new List<Color>();
            PathObjectRoots = new List<GameObject>();
            Paths = new List<MapPath>();
        }

        protected override void OnDispose()
        {
            if (Cells != null)
            {
                foreach (MapCell cell in Cells.Values)
                    cell.Dispose();
                Cells = null;
            }

            PathColors = null;
            Paths = null;
            MapRoot = null;
            CellRoot = null;
            PathRoot = null;
            Loader = null;

            base.OnDispose();
        }

        public void Load(string url)
        {
            var res = Loader.LoadJsonFile<MapSetting>(url);
            Load(res.Data);
        }

        public void Load(MapSetting setting)
        {
            CellCountX = setting.CellCountX;
            CellCountY = setting.CellCountY;
            UpdateMapSize();

            // Load map cells
            Cells = new Dictionary<int, MapCell>();
            foreach (MapCellSetting cellSetting in setting.Cells)
            {
                GameObject cellObject;
                if ((cellSetting.Flags & (int)MapCellFlag.Highland) != 0)
                    cellObject = Loader.LoadPrefab("Map/GrassHigh2").Instantiate();
                else
                    cellObject = Loader.LoadPrefab("Map/GrassLow2").Instantiate();
                cellObject.transform.SetParent(CellRoot, false);
                MapCell cell = cellObject.AddComponent<MapCell>();
                cell.Init(this, (short)cellSetting.X, (short)cellSetting.Y);
                cell.Load(cellSetting);
                Cells.Add(cell.Key, cell);
            }
            BuildNeighbours();

            // Load paths
            foreach (MapPathSetting pathSetting in setting.Paths)
            {
                List<Vector2> locations = new List<Vector2>(pathSetting.Waypoints.Length);
                foreach (var point in pathSetting.Waypoints)
                    locations.Add(new Vector2(point.X, point.Y));
                AddPath(locations, new Color(pathSetting.ColorR, pathSetting.ColorG, pathSetting.ColorB));
            }
            CalculatePaths();
            ToggleShowPaths(true);
        }

        protected void UpdateMapSize()
        {
#if POINTY_TOPPED
            Width = Mathf.Max(0, CellCountX - 1) * MapCell.Width;
            Height = Mathf.Max(0, CellCountY - 1) * MapCell.HalfHeight * 1.5f;
#else
            Width = Mathf.Max(0, CellCountX - 1) * MapCell.HalfWidth * 1.5f;
            Height = Mathf.Max(0, CellCountY - 1) * MapCell.Height;
#endif
        }

        protected void BuildNeighbours()
        {
            foreach (MapCell cell in Cells.Values)
                cell.BuildNeighbours();
        }

        public MapCell GetCell(short x, short y)
        {
            int key = MapUtils.MakeKey(x, y);
            MapCell cell = null;
            Cells.TryGetValue(key, out cell);
            return cell;
        }

        public MapCell GetCell(int x, int y)
        {
            return GetCell((short)x, (short)y);
        }

        public MapCell GetCell(Vector2 location)
        {
            return GetCell((short)location.x, (short)location.y);
        }

        public MapCell GetCellByWorldXY(float worldX, float worldY)
        {
            Vector2 location = MapUtils.PositionToLocation(worldX, worldY);
            return GetCell(location);
        }

        public MapCell GetCellByWorldXY(Vector3 worldPoint)
        {
            return GetCellByWorldXY(worldPoint.x, worldPoint.y);
        }

        public MapCell GetCellByMousePosition()
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return GetCellByWorldXY(worldPoint);
        }

        public int GetDistance(int x1, int y1, int x2, int y2)
        {
            return MapUtils.Distance(x1, y1, x2, y2);
        }

        public int GetDistance(Vector2 location1, Vector2 location2)
        {
            return GetDistance((int)location1.x, (int)location1.y, (int)location2.x, (int)location2.y);
        }

        public int GetDistance(MapCell cell1, MapCell cell2)
        {
            return GetDistance(cell1.X, cell1.Y, cell2.X, cell2.Y);
        }

        public void AddPath(List<Vector2> locations, Color color)
        {
            MapPath path = new MapPath();
            path.Init(this);
            path.AddWaypoints(locations);
            Paths.Add(path);

            PathColors.Add(color);
            PathObjectRoots.Add(null);
        }

        public void RemovePath(int index)
        {
            PathColors.RemoveAt(index);
            Paths.RemoveAt(index);

            if (PathObjectRoots[index] != null)
                Destroy(PathObjectRoots[index]);
            PathObjectRoots.RemoveAt(index);
        }

        public void UpdatePath(int index, List<Vector2> locations, Color color)
        {
            PathColors[index] = color;
            Paths[index].ClearWaypoints();
            Paths[index].AddWaypoints(locations);

            if (PathObjectRoots[index] != null)
                Destroy(PathObjectRoots[index]);
            PathObjectRoots[index] = null;
        }

        public void CalculatePath(int index)
        {
            MapPath path = Paths[index];
            path.CalculatePathMaps();
        }

        public void CalculatePaths()
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
                CalculatePath(i);
        }

        public bool CanBlockLocation(Vector2 location)
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
            {
                if (!Paths[i].CanBlockLocation(location))
                    return false;
            }
            return true;
        }

        public void ToggleShowPath(int index, bool show, bool update = true)
        {
            GameObject pathObjectRoot = PathObjectRoots[index];
            if (pathObjectRoot && (!show || update))
            {
                Object.Destroy(pathObjectRoot);
                PathObjectRoots[index] = null;
                pathObjectRoot = null;
            }

            if (show && pathObjectRoot == null)
            {
                Color color = PathColors[index];
                MapPath path = Paths[index];

                pathObjectRoot = new GameObject("PathObjectRoot");
                pathObjectRoot.transform.SetParent(PathRoot.transform, false);
                PathObjectRoots[index] = pathObjectRoot;

                GameObject obj = Loader.LoadPrefab("Map/StartMark").Instantiate();
                obj.transform.SetParent(pathObjectRoot.transform, false);
                obj.transform.position = path.StartPosition;
                obj.GetComponent<SpriteRenderer>().color = color;

                obj = Loader.LoadPrefab("Map/EndMark").Instantiate();
                obj.transform.SetParent(pathObjectRoot.transform, false);
                obj.transform.position = path.EndPosition;
                obj.GetComponent<SpriteRenderer>().color = color;

                int count = path.WaypointCount;
                for (int i = 1; i < count; ++i)
                {
                    List<Vector3> positions = path.FindPathPosition(i);
                    if (positions.Count >= 2)
                    {
                        VectorLine line = new VectorLine("PathLine", positions, null, 2, LineType.Continuous);
                        line.color = color;
                        obj = new GameObject("PathLine");
                        obj.transform.SetParent(pathObjectRoot.transform, false);
                        VectorManager.useDraw3D = true;
                        VectorManager.ObjectSetup(obj, line, Visibility.Dynamic, Brightness.None);
                    }

                    if (i < count - 1)
                    {
                        obj = Loader.LoadPrefab("Map/StartMark").Instantiate();
                        obj.transform.SetParent(pathObjectRoot.transform, false);
                        obj.transform.position = path.GetPosition(i);
                        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        obj.GetComponent<SpriteRenderer>().color = color;
                    }
                }
            }
        }

        public void ToggleShowPaths(bool show, bool update = true)
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
                ToggleShowPath(i, show, update);
        }

        /// <summary>
        /// 计算哪些位置会堵死迷宫
        /// </summary>
        public void CalculateCantBlockCells()
        {
            cantBlockCells.Clear();

            int pathCount = Paths.Count;
            for (int i = 0; i < pathCount; ++i)
            {
                MapPath path = Paths[i];
                int waypointCount = path.WaypointCount;
                for (int j = 1; j < waypointCount; ++j)
                {
                    MapCell start = GetCell(path.GetLocation(j - 1));
                    MapCell end = GetCell(path.GetLocation(j));
                    DoCalculateCantBlockCells(start, end, 0, new HashSet<int>(), new Dictionary<int, int>(), new Dictionary<int, int>(), new Dictionary<int, int>());
                }
            }
        }

        private bool DoCalculateCantBlockCells(MapCell start, MapCell end, int depth, HashSet<int> visited, Dictionary<int, int> depths, Dictionary<int, int> lows, Dictionary<int, int> parents)
        {
            int startKey = start.Key;
            visited.Add(startKey);
            depths[startKey] = depth;
            lows[startKey] = depth;
            int childCount = 0;
            bool isCutPoint = false;
            bool reachedEnd = false;
            int parentKey;

            foreach (MapCell neighbour in start.Neighbours)
            {
                if (neighbour == null || neighbour.IsObstacle)
                    continue;

                int neighbourKey = neighbour.Key;
                if (!visited.Contains(neighbourKey))
                {
                    parents[neighbourKey] = startKey;
                    reachedEnd = DoCalculateCantBlockCells(neighbour, end, depth + 1, visited, depths, lows, parents);
                    childCount += 1;
                    if (lows[neighbourKey] >= depths[startKey])
                        isCutPoint = true;
                    lows[startKey] = Mathf.Min(lows[startKey], lows[neighbourKey]);
                }
                else
                {
                    parentKey = -1;
                    if (parents.ContainsKey(startKey))
                        parentKey = parents[startKey];
                    if (neighbourKey != parentKey)
                        lows[startKey] = Mathf.Min(lows[startKey], depths[neighbourKey]);
                }
            }

            reachedEnd = reachedEnd || (start == end);

            parentKey = -1;
            if (parents.ContainsKey(startKey))
                parentKey = parents[startKey];
            if (parentKey != -1 && isCutPoint || parentKey == -1 && childCount > 1)
            {
                if (reachedEnd)
                    cantBlockCells.Add(startKey);
            }

            return reachedEnd;
        }

        /// <summary>
        /// 更新SceneEntity所属的MapCell信息
        /// </summary>
        public void UpdateSceneEntityCell(SceneEntity sceneEntity)
        {
            MapCell cell = GetCell(sceneEntity.Location);
            if (sceneEntity.Cell != cell)
            {
                if (sceneEntity.Cell != null)
                    sceneEntity.Cell.RemoveEntity(sceneEntity);
                if (cell != null)
                    cell.AddEntity(sceneEntity);
                sceneEntity.Cell = cell;
            }
        }

        public void Update()
        {
            UpdateCameraPosition();
        }

        public void UpdateCameraPosition()
        {
            Camera camera = Camera.main;
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * camera.aspect;
            float left = cameraHalfWidth;
            float right = Width - cameraHalfWidth;
            float bottom = cameraHalfHeight;
            float top = Height - cameraHalfHeight;

            Vector3 cameraPosition = camera.transform.position;
            if (left > right)
                cameraPosition.x = (left + right) / 2;
            else if (cameraPosition.x < left)
                cameraPosition.x = left;
            else if (cameraPosition.x > right)
                cameraPosition.x = right;
            if (bottom > top)
                cameraPosition.y = (bottom + top) / 2;
            else if (cameraPosition.y < bottom)
                cameraPosition.y = bottom;
            else if (cameraPosition.y > top)
                cameraPosition.y = top;
            camera.transform.position = cameraPosition;
        }

        public void UpdateCameraPosition(Vector2 deltaPosition)
        {
            Camera camera = Camera.main;
            camera.transform.position += new Vector3(deltaPosition.x, deltaPosition.y);
            UpdateCameraPosition();
        }

        public void ShowDebugDraw(bool show)
        {
            foreach (MapCell cell in Cells.Values)
            {
                cell.ShowDebugDraw(show);
            }
        }
    }
}
