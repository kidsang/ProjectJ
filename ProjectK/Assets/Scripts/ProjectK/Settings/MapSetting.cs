﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public enum MapCellFlag
    {
        None = 0,
        CanWalk = 1,
        CanBuild = 2,
    }

    public class MapSetting
    {
        public string Name;
        public int CellCountX;
        public int CellCountY;

        public MapPathSetting[] Paths;
        public MapCellSetting[] Cells;

        public MapSetting()
        {
        }

        public MapSetting(bool initEmpty)
        {
            if (initEmpty)
            {
                Paths = new MapPathSetting[0];
                Cells = new MapCellSetting[0];
            }
        }
    }

    public class MapPathSetting
    {
        public float ColorR;
        public float ColorG;
        public float ColorB;
        public MapWaypointSetting[] Waypoints = new MapWaypointSetting[0];
    }

    public class MapWaypointSetting
    {
        public int X;
        public int Y;
    }

    public class MapCellSetting
    {
        public int X;
        public int Y;
        public int Flags;
    }
}
