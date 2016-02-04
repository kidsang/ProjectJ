// 六边形格子尖头向上
#define POINTY_TOPPED

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    /**
     * MapCell使用Axial坐标系，其中x + y + z = 0，因此只需要x和y的就可以确定一个MapCell。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class MapCell : DisposableBehaviour
    {
        public static readonly float Radius = MapUtils.Radius;
        public static readonly float HalfWidth = MapUtils.HalfWidth;
        public static readonly float HalfHeight = MapUtils.HalfHeight;
        public static readonly float Width = MapUtils.Width;
        public static readonly float Height = MapUtils.Height;

        protected GameObject CellObject { get; private set; }
        public Map Map { get; private set; }
        public ResourceLoader Loader { get; private set; }

        public short X { get; private set; }
        public short Y { get; private set; }
        public short Z { get; private set; }
        public int Key { get; private set; }
        public int Flags { get; set; }

        public static readonly int NumNeighbours = 6;
        public MapCell[] Neighbours { get; private set; }

        /// <summary>
        /// 该格子上所有的SceneEntity
        /// </summary>
        public List<SceneEntity> SceneEntities;

        /// <summary>
        /// 该格子上所有的TowerEntity
        /// </summary>
        public List<MonsterEntity> MonsterEntities;

        /// <summary>
        /// 该格子上所有的TowerEntity
        /// </summary>
        public List<TowerEntity> TowerEntities;

        internal void Init(Map map, short x, short y)
        {
            CellObject = gameObject;
            Map = map;
            Loader = map.Loader;

            this.X = x;
            this.Y = y;
            this.Z = (short)(-x - y);
            this.Key = MapUtils.MakeKey(x, y);

            CellObject.transform.localPosition = new Vector3(CenterX, CenterY, X / 2.0f + Y);

            Neighbours = new MapCell[NumNeighbours];
            SceneEntities = new List<SceneEntity>();
            MonsterEntities = new List<MonsterEntity>();
            TowerEntities = new List<TowerEntity>();
        }

        internal void Load(MapCellSetting setting)
        {
            Flags = setting.Flags;
        }

        protected override void OnDispose()
        {
            DestroyObject(CellObject);
            CellObject = null;

            Map = null;
            Loader = null;
            Neighbours = null;
            SceneEntities = null;
            MonsterEntities = null;
            TowerEntities = null;

            base.OnDispose();
        }

        internal void BuildNeighbours()
        {
            Neighbours[0] = Map.GetCell(X + 1, Y);
            Neighbours[1] = Map.GetCell(X + 1, Y - 1);
            Neighbours[2] = Map.GetCell(X, Y - 1);
            Neighbours[3] = Map.GetCell(X - 1, Y);
            Neighbours[4] = Map.GetCell(X - 1, Y + 1);
            Neighbours[5] = Map.GetCell(X, Y + 1);
        }

        public Vector2 Location
        {
            get { return new Vector2(X, Y); }
        }

#if POINTY_TOPPED
        public float CenterX
        {
            get { return Width * (X + 0.5f * Y); }
        }

        public float CenterY
        {
            get { return Radius * 1.5f * Y; }
        }
#else
        public float CenterX
        {
            get { return Radius * 1.5f * X; }
        }

        public float CenterY
        {
            get { return Height * (Y + 0.5f * X); }
        }
#endif

        public Vector3 Position
        {
            get { return new Vector3(CenterX, CenterY); }
        }

        public bool HasFlag(MapCellFlag flag)
        {
            return (Flags & (int)flag) != 0;
        }

        public void SetFlag(MapCellFlag flag, bool value)
        {
            if (value)
                Flags = Flags | (int)flag;
            else
                Flags = Flags & (~(int)flag);
        }

        public bool IsObstacle
        {
            get { return !HasFlag(MapCellFlag.CanWalk); }
        }

        public bool CanWalk
        {
            get { return HasFlag(MapCellFlag.CanWalk); }
            set { SetFlag(MapCellFlag.CanWalk, value); }
        }

        public bool CanBuild
        {
            get { return HasFlag(MapCellFlag.CanBuild); }
            set { SetFlag(MapCellFlag.CanBuild, value); }
        }

        public void AddEntity(SceneEntity entity)
        {
            if (!SceneEntities.Contains(entity))
            {
                SceneEntities.Add(entity);

                if (entity is MonsterEntity)
                    MonsterEntities.Add(entity as MonsterEntity);
                else if (entity is TowerEntity)
                    TowerEntities.Add(entity as TowerEntity);
            }
        }

        public void RemoveEntity(SceneEntity entity)
        {
            SceneEntities.Remove(entity);
            if (entity is MonsterEntity)
                MonsterEntities.Remove(entity as MonsterEntity);
            else if (entity is TowerEntity)
                TowerEntities.Remove(entity as TowerEntity);
        }

        public void ColorTransform(float r = 1, float g = 1, float b = 1, float a = 1)
        {
            (GetComponent<Renderer>() as SpriteRenderer).color = new Color(r, g, b, a);
        }

        public void ToWhite()
        {
            ColorTransform();
        }

        public void ToRed()
        {
            ColorTransform(1, 0, 0);
        }

        public void ToGreen()
        {
            ColorTransform(0, 1, 0);
        }

        public void ToBlue()
        {
            ColorTransform(0, 0, 1);
        }

        public void ShowNeighbours(bool show)
        {
            foreach (MapCell cell in Neighbours)
            {
                if (cell != null)
                {
                    if (show)
                        cell.ColorTransform(0.5f, 0.5f, 0.5f);
                    else
                        cell.ColorTransform();
                }
            }
        }

        public void ShowDebugDraw(bool show)
        {
            HudManager hudManager = HudManager.GetHudManager(gameObject);
            if (show)
            {
                UIBase hud = hudManager.GetHud("loc");
                if (hud == null)
                {
                    hud = UIManager.Instance.CreateHud<TextUI>();
                    hudManager.AddHud("loc", hud, new Vector2());
                }
                hud.Show();
                hud.Refresh(X + "," + Y);
            }
            else
            {
                UIBase hud = hudManager.GetHud("loc");
                if (hud != null)
                    hud.Hide();
            }
        }

        public override string ToString()
        {
            return "MapCell(" + X + "," + Y + ")";
        }
    }
}
