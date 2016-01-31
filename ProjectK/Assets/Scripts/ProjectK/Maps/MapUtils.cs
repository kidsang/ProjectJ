// 六边形格子尖头向上
#define POINTY_TOPPED

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ProjectK
{
    public static class MapUtils
    {
        public static readonly float PI2 = Mathf.PI * 2;
        public static readonly float Sqrt3 = Mathf.Sqrt(3.0f);

#if POINTY_TOPPED
        public static readonly float Radius = 1.0f;
        public static readonly float HalfWidth = Radius * Sqrt3 / 2.0f;
        public static readonly float HalfHeight = Radius;
#else
        public static readonly float Radius = 1.0f;
        public static readonly float HalfWidth = Radius;
        public static readonly float HalfHeight = Radius * Sqrt3 / 2.0f;
#endif
        public static readonly float Width = HalfWidth * 2;
        public static readonly float Height = HalfHeight * 2;

        public static readonly Vector2 Vector2X = new Vector2(1, 0);
        public static readonly Vector2 Vector2Y = new Vector2(0, 1);

        public static readonly Vector3 Vector3X = new Vector3(1, 0, 0);
        public static readonly Vector3 Vector3Y = new Vector3(0, 1, 0);
        public static readonly Vector3 Vector3Z = new Vector3(0, 0, 1);

        public static readonly Vector2[] Directions = {
            new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
            new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1)
                                                      };
        public static int MakeKey(short x, short y)
        {
            return (y << 16) | (ushort)x;
        }

#if POINTY_TOPPED
        public static Vector3 LocationToPosition(int x, int y)
        {
            return new Vector3(Radius * Sqrt3 * (x + 0.5f * y), Radius * 1.5f * y);
        }

        public static Vector2 PositionToLocation(float x, float y)
        {
            float fx = (Sqrt3 * x - y) / 3.0f / Radius;
            float fy = y * 2 / 3.0f / Radius;
            float fz = -fx - fy;

            float rx = Mathf.Round(fx);
            float ry = Mathf.Round(fy);
            float rz = Mathf.Round(fz);

            float dx = Mathf.Abs(rx - fx);
            float dy = Mathf.Abs(ry - fy);
            float dz = Mathf.Abs(rz - fz);

            if (dx > dy && dx > dz)
                rx = -ry - rz;
            else if (dy > dz)
                ry = -rx - rz;

            return new Vector2((int)rx, (int)ry);
        }
#else
        public static Vector3 LocationToPosition(int x, int y)
        {
            return new Vector3(Radius * 1.5f * x, Radius * Sqrt3 * (y + 0.5f * x));
        }

        public static Vector2 PositionToLocation(float x, float y)
        {
            float fx = x * 2 / 3.0f / Radius;
            float fy = (Sqrt3 * y - x) / 3.0f / Radius;
            float fz = -fx - fy;

            float rx = Mathf.Round(fx);
            float ry = Mathf.Round(fy);
            float rz = Mathf.Round(fz);

            float dx = Mathf.Abs(rx - fx);
            float dy = Mathf.Abs(ry - fy);
            float dz = Mathf.Abs(rz - fz);

            if (dx > dy && dx > dz)
                rx = -ry - rz;
            else if (dy > dz)
                ry = -rx - rz;

            return new Vector2((int)rx, (int)ry);
        }
#endif

        public static Vector3 LocationToPosition(Vector2 location)
        {
            return LocationToPosition((int)location.x, (int)location.y);
        }

        public static Vector2 PositionToLocation(Vector3 position)
        {
            return PositionToLocation(position.x, position.y);
        }

        /// <summary>
        /// 判断点p是否在直线p1p2上
        /// </summary>
        public static bool InLine(Vector2 p, Vector2 p1, Vector2 p2)
        {
            return (p2.x - p1.x) * (p.y - p1.y) == (p2.y - p1.y) * (p.x - p1.x);
        }

        /// <summary>
        /// 判断点p是否在线段p1p2上
        /// </summary>
        public static bool InSegment(Vector2 p, Vector2 p1, Vector2 p2)
        {
            if (!InLine(p, p1, p2))
                return false;

            if (p1.x < p2.x)
            {
                if (p.x < p1.x || p.x > p2.x)
                    return false;
            }
            else
            {
                if (p.x > p1.x || p.x < p2.x)
                    return false;
            }

            if (p1.y < p2.y)
            {
                if (p.y < p1.y || p.y > p2.y)
                    return false;
            }
            else
            {
                if (p.y > p1.y || p.y < p2.y)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 两个六边形格子之间的距离
        /// </summary>
        public static int Distance(int x1, int y1, int x2, int y2)
        {
            int z1 = -x1 - y1;
            int z2 = -x2 - y2;
            return (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) + Mathf.Abs(z1 - z2)) / 2;
        }

        /// <summary>
        /// 两个六边形格子之间的距离
        /// </summary>
        public static int Distance(Vector2 location1, Vector2 location2)
        {
            return Distance((int)location1.x, (int)location1.y, (int)location2.x, (int)location2.y);
        }

        /// <summary>
        /// 六边形格子在某个方向上的邻居位置
        /// </summary>
        public static Vector2 Neighbour(int x, int y, int direction)
        {
            direction %= Directions.Length;
            Vector2 dir = Directions[direction];
            return new Vector2(dir.x + x, dir.y + y);
        }

        /// <summary>
        /// 六边形格子在某个方向上的邻居位置
        /// </summary>
        public static Vector2 Neighbour(Vector2 location, int direction)
        {
            return Neighbour((int)location.x, (int)location.y, direction);
        }

        /// <summary>
        /// 六边形格子在某个方向上的邻居位置
        /// </summary>
        public static Vector2 Neighbour(int x, int y, int direction, int distance)
        {
            direction %= Directions.Length;
            Vector2 dir = Directions[direction];
            return new Vector2(dir.x * distance + x, dir.y * distance + y);
        }

        /// <summary>
        /// 六边形格子在某个方向上的邻居位置
        /// </summary>
        public static Vector2 Neighbour(Vector2 location, int direction, int distance)
        {
            return Neighbour((int)location.x, (int)location.y, direction, distance);
        }

        /// <summary>
        /// 某个位置六边形格子的所有邻居
        /// </summary>
        public static Vector2[] Neighbours(int x, int y)
        {
            int count = Directions.Length;
            Vector2[] ret = new Vector2[count];
            Directions.CopyTo(ret, count);
            for (int i = 0; i < count; ++i)
            {
                ret[i].x += x;
                ret[i].y += y;
            }
            return ret;
        }

        /// <summary>
        /// 返回以某个位置为中心的一环六边形格子
        /// </summary>
        private static void Ring(int x, int y, int radius, Vector2[] ret, ref int index)
        {
            if (radius > 0)
            {
                Vector2 current = Neighbour(x, y, 4, radius);
                for (int i = 0; i < 6; ++i)
                {
                    for (int j = 0; j < radius; ++j)
                    {
                        ret[index++] = current;
                        current = Neighbour(current, i);
                    }
                }
            }
            else
            {
                ret[index++] = new Vector2(x, y);
            }
        }

        /// <summary>
        /// 返回以某个位置为中心的一环六边形格子
        /// </summary>
        public static Vector2[] Ring(int x, int y, int radius)
        {
            int count = radius <= 0 ? 1 : radius * 6;
            Vector2[] ret = new Vector2[count];

            int index = 0;
            Ring(x, y, radius, ret, ref index);

            return ret;
        }

        /// <summary>
        /// 返回以某个位置为中心，范围内所有六边形格子
        /// </summary>
        public static Vector2[] Circle(int x, int y, int radius)
        {
            int count = 1;
            for (int i = 1; i <= radius; ++i)
                count += 6 * i;
            Vector2[] ret = new Vector2[count];

            int index = 0;
            for (int i = 0; i <= radius; ++i)
                Ring(x, y, i, ret, ref index);

            return ret;
        }

        /// <summary>
        /// 求v1与v2的夹角，返回角度范围[0, 2PI)
        /// </summary>
        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            return AngleBetween(v1.x, v1.y, v2.x, v2.y);
        }

        /// <summary>
        /// 求v1(x1, y1)与v2(x2, y2)的夹角，返回角度范围[0, 2PI)
        /// </summary>
        public static float AngleBetween(float x1, float y1, float x2, float y2)
        {
            float dot = x1 * x2 + y1 * y2;
            float det = x1 * y2 + x2 * y1;
            return Angle(dot, det);
        }

        /// <summary>
        /// 求v与x轴正方向的夹角，返回角度范围[0, 2PI)
        /// </summary>
        public static float Angle(Vector2 v)
        {
            return Angle(v.x, v.y);
        }

        /// <summary>
        /// 求v(x, y)与x轴正方向的夹角，返回角度范围[0, 2PI)
        /// </summary>
        public static float Angle(float x, float y)
        {
            float angle = Mathf.Atan2(y, x);
            angle = NormalizeAngle(angle);
            return angle;
        }

        /// <summary>
        /// 将angle规范至[0, 2PI)内
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            while (angle < 0) angle += PI2;
            while (angle >= PI2) angle -= PI2;
            return angle;
        }

        /// <summary>
        /// 返回两夹角之间的差值，范围[0, PI)
        /// </summary>
        public static float DeltaAngle(float angle1, float angle2)
        {
            float diff = angle1 - angle2;
            while (diff <= -Mathf.PI) diff += PI2;
            while (diff > Mathf.PI) diff -= PI2;
            return Mathf.Abs(diff);
        }

        /// <summary>
        /// 在一个大小为cellCountX,cellCountY的地图中随机一个位置
        /// </summary>
        public static Vector2 RandomInMap(int cellCountX, int cellCountY)
        {
            int x = Random.Range(0, cellCountX);
            int minY = -x / 2;
            int maxY = cellCountY + minY - 1;
            int y = Random.Range(minY, maxY);
            return new Vector2(x, y);
        }

        /// <summary>
        /// 返回地图的中心点
        /// </summary>
        public static Vector2 CenterOfMap(int cellCountX, int cellCountY)
        {
            int x = cellCountX / 2;
            int y = -x / 2 + cellCountY / 2;
            return new Vector2(x, y);
        }

        public static Dictionary<int, MapCellSetting> ArrayToDict(MapCellSetting[] cellSettings)
        {
            Dictionary<int, MapCellSetting> ret = new Dictionary<int, MapCellSetting>();
            foreach (MapCellSetting cellSetting in cellSettings)
                ret[MapUtils.MakeKey((short)cellSetting.X, (short)cellSetting.Y)] = cellSetting;
            return ret;
        }

        public static MapCellSetting[] DictToArray(Dictionary<int, MapCellSetting> cellSettings)
        {
            return cellSettings.Values.ToArray();
        }
    }
}
