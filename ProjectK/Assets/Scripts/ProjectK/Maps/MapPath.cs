using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class MapPath
    {
        private Map map;
        private List<MapWaypoint> waypoints = new List<MapWaypoint>();

        public void Init(Map map)
        {
            this.map = map;
        }

        public void AddWaypoints(List<Vector2> locations)
        {
            foreach (var location in locations)
            {
                MapWaypoint waypoint = new MapWaypoint();
                waypoint.Location = location;
                waypoint.Position = MapUtils.LocationToPosition(location);
                waypoints.Add(waypoint);
            }
        }

        public void ClearWaypoints()
        {
            waypoints.Clear();
        }

        public void CalculatePathMaps()
        {
            for (int i = 1; i < waypoints.Count; ++i)
                CalculatePathMap(i);
        }

        private void CalculatePathMap(int toWaypointIndex)
        {
            MapWaypoint toWaypoint = waypoints[toWaypointIndex];
            Vector2 toLocation = toWaypoint.Location;
            Dictionary<MapCell, MapCell> pathMap = toWaypoint.PathMap;
            DoCalculatePathMap(toLocation, pathMap);
        }

        private void DoCalculatePathMap(Vector2 toLocation, Dictionary<MapCell, MapCell> pathMap)
        {
            pathMap.Clear();
            MapCell toCell = map.GetCell(toLocation);
            if (toCell == null)
                return;

            Queue<MapCell> frontier = new Queue<MapCell>();
            frontier.Enqueue(toCell);
            pathMap[toCell] = toCell;

            while (frontier.Count > 0)
            {
                MapCell current = frontier.Dequeue();
                foreach (MapCell neighbour in current.Neighbours)
                {
                    if (neighbour == null || neighbour.IsObstacle)
                        continue;

                    if (pathMap.ContainsKey(neighbour))
                        continue;

                    frontier.Enqueue(neighbour);
                    pathMap[neighbour] = current;
                }
            }

            pathMap.Remove(toCell);
        }

        public bool GetNextLocation(Vector2 fromLocation, int toWaypointIndex, out Vector2 nextLocation)
        {
            if (toWaypointIndex >= waypoints.Count)
            {
                nextLocation = Vector2.zero;
                return false;
            }

            MapCell fromCell = map.GetCell(fromLocation);
            if (fromCell == null)
            {
                nextLocation = Vector2.zero;
                return false;
            }

            MapWaypoint waypoint = waypoints[toWaypointIndex];
            MapCell nextCell;
            if (!waypoint.PathMap.TryGetValue(fromCell, out nextCell))
            {
                nextLocation = Vector2.zero;
                return false;
            }

            nextLocation = nextCell.Location;
            return true;
        }

        public List<Vector2> FindPathLocation(int toWaypointIndex)
        {
            List<Vector2> locations = new List<Vector2>();
            FindPathLocation(waypoints[toWaypointIndex - 1].Location, toWaypointIndex, locations);
            return locations;
        }

        public List<Vector3> FindPathPosition(int toWaypointIndex)
        {
            List<Vector3> positions = new List<Vector3>();
            FindPathPosition(waypoints[toWaypointIndex - 1].Location, toWaypointIndex, positions);
            return positions;
        }

        public void FindPathLocation(Vector2 fromLocation, int toWaypointIndex, List<Vector2> locations)
        {
            locations.Clear();
            MapCell fromCell = map.GetCell(fromLocation);
            if (fromCell == null)
                return;

            MapWaypoint waypoint = waypoints[toWaypointIndex];
            MapCell toCell = map.GetCell(waypoint.Location);
            if (toCell == null)
                return;

             // build path
            Dictionary<MapCell, MapCell> PathMap = waypoint.PathMap;
            while (fromCell != toCell)
            {
                locations.Add(fromCell.Location);
                if (!PathMap.TryGetValue(fromCell, out fromCell))
                    break;
            }
            if (fromCell == toCell)
                locations.Add(toCell.Location);

            // merge path
            for (int i = locations.Count - 2; i >= 1; --i)
            {
                Vector2 p1 = locations[i + 1];
                Vector2 p2 = locations[i];
                Vector2 p3 = locations[i - 1];
                if (MapUtils.InLine(p2, p1, p3))
                    locations.RemoveAt(i);
            }
        }

        public void FindPathPosition(Vector2 fromLocation, int toWaypointIndex, List<Vector3> positions)
        {
            List<Vector2> locations = new List<Vector2>();
            FindPathLocation(fromLocation, toWaypointIndex, locations);

            positions.Clear();
            for (int i = 0; i < locations.Count; ++i)
                positions.Add(MapUtils.LocationToPosition(locations[i]));
        }

        public int WaypointCount
        {
            get { return waypoints.Count; }
        }

        public Vector2 GetLocation(int waypointIndex)
        {
            MapWaypoint waypoint = waypoints[waypointIndex];
            return waypoint.Location;
        }

        public Vector2 GetPosition(int waypointIndex)
        {
            MapWaypoint waypoint = waypoints[waypointIndex];
            return waypoint.Position;
        }

        public Vector2 StartLocation
        {
            get { return GetLocation(0); }
        }

        public Vector2 EndLocation
        {
            get { return GetLocation(waypoints.Count - 1); }
        }

        public Vector2 StartPosition
        {
            get { return GetPosition(0); }
        }

        public Vector2 EndPosition
        {
            get { return GetPosition(waypoints.Count - 1); }
        }

        class MapWaypoint
        {
            public Vector2 Location;
            public Vector3 Position;

            public Dictionary<MapCell, MapCell> PathMap = new Dictionary<MapCell, MapCell>();
        }
    }
}
