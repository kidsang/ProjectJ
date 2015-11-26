using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class Scene : DisposableBehaviour
    {
        private GameObject sceneRoot;
        private ResourceLoader loader;

        public Map Map { get; private set; }
        public SpawnManager SpawnManager { get; private set; }

        public bool Playing { get; private set; }
        private float startTime;
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }

        private List<MonsterEntity> monsters = new List<MonsterEntity>();

        private uint nextEntityUid = 1;
        private Dictionary<uint, SceneEntity> entities = new Dictionary<uint, SceneEntity>();

        public void Init()
        {
            sceneRoot = gameObject;
            loader = new ResourceLoader();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform, false);

            Map = mapRoot.AddComponent<Map>();
            Map.Init(loader);

            SpawnManager = new SpawnManager();
        }

        protected override void OnDispose()
        {
            if (monsters != null)
            {
                foreach (var monster in monsters)
                    monster.Dispose();
                monsters = null;
            }

            if (Map != null)
            {
                Map.Dispose();
                Map = null;
            }

            if (loader != null)
            {
                loader.Dispose();
                loader = null;
            }

            if (sceneRoot != null)
            {
                DestroyObject(sceneRoot);
                sceneRoot = null;
            }

            base.OnDispose();
        }

        public void Load(SceneSetting setting)
        {
            Map.Load(setting.Map);
            SpawnManager.Load(setting.Spawn);
        }

        public void Load(string url)
        {
            SceneSetting setting = loader.Load<JsonFile<SceneSetting>>(url).Data;
            Load(setting);
        }

        public void StartScene()
        {
            Playing = true;
            startTime = UnityEngine.Time.fixedTime;

            SpawnManager.Start();
        }

        private void FixedUpdate()
        {
            if (!Playing)
                return;

            Time = UnityEngine.Time.fixedTime - startTime;
            DeltaTime = UnityEngine.Time.fixedDeltaTime;

            SpawnManager.Activate(this, Time);

            foreach (var monster in monsters)
                monster.Activate(this);
        }

        public void CreateMonster(int pathIndex, int templateID, int count)
        {
            MapPath path = Map.Paths[pathIndex];
            for (int i = 0; i < count; ++i)
            {
                MonsterEntity monster = SceneEntityManager.Create<MonsterEntity>(loader, templateID);
                monster.gameObject.transform.SetParent(sceneRoot.transform);
                monster.gameObject.transform.position = path.StartPosition;
                monster.SetPath(path);
                monsters.Add(monster);
            }
        }

        #region 范围找怪接口

        /// <summary>
        /// 收集圆形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesCircle(Vector2 origin, float radius,
            List<SceneEntity> resultEntities = null, bool sortByDistance = false)
        {
            return CollectEntities(origin, radius, 0, 0, CollectEntityCircleFunc, resultEntities, sortByDistance);
        }

        /// <summary>
        /// 收集扇形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesFan(Vector2 origin, float radius, float rangeAngle, float rotateAngle,
            List<SceneEntity> resultEntities = null, bool sortByDistance = false)
        {
            return CollectEntities(origin, radius, rangeAngle, rotateAngle, CollectEntityFanFunc, resultEntities, sortByDistance);
        }

        /// <summary>
        /// 收集矩形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesRectangle(Vector2 origin, float width, float height, float rotateAngle,
            List<SceneEntity> resultEntities = null, bool sortByDistance = false)
        {
            return CollectEntities(origin, width, height, rotateAngle, CollectEntityRectangleFunc, resultEntities, sortByDistance);
        }

        private List<SceneEntity> CollectEntities(Vector2 origin, float param1, float param2, float param3, CollectEntityFunc collectFunc,
            List<SceneEntity> resultEntities = null, bool sortByDistance = false)
        {
            if (resultEntities == null)
                resultEntities = new List<SceneEntity>();

            collectFunc(origin, param1, param2, param3, resultEntities);

            if (sortByDistance)
            {
                EntityDistanceSorter sorter = new EntityDistanceSorter(origin);
                resultEntities.Sort(sorter);
            }

            return resultEntities;
        }

        delegate void CollectEntityFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities);

        public void CollectEntityCircleFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float radius2 = param1 * param1;
            foreach (SceneEntity entity in entities.Values)
            {
                Vector2 position = entity.NaviComp.Position;
                Vector2 direction = position - origin;
                if (direction.sqrMagnitude <= radius2)
                    resultEntities.Add(entity);
            }
        }

        public void CollectEntityFanFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float radius2 = param1 * param1;
            float rangeAngle = param2;
            float rotateAngle = param3;
            foreach (SceneEntity entity in entities.Values)
            {
                Vector2 position = entity.NaviComp.Position;
                Vector2 direction = position - origin;
                if (direction.sqrMagnitude <= radius2)
                {
                    float angle = MapUtils.Angle(direction);
                    float deltaAngle = MapUtils.DeltaAngle(angle, rotateAngle);
                    if (deltaAngle <= rangeAngle)
                        resultEntities.Add(entity);
                }
            }
        }

        public void CollectEntityRectangleFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float halfWidth = param1 * 0.5f;
            float halfHeight = param2 * 0.5f;
            float rotateAngle = param3;
            Vector2 center = new Vector2(origin.x + Mathf.Cos(rotateAngle) * halfWidth, origin.y + Mathf.Sin(rotateAngle) * halfHeight);
            Quaternion rotate = Quaternion.Euler(0, 0, -rotateAngle);

            foreach (SceneEntity entity in entities.Values)
            {
                Vector2 position = entity.NaviComp.Position;
                Vector2 direction = rotate * (position - center);
                if (direction.x <= halfWidth && direction.y <= halfHeight)
                    resultEntities.Add(entity);
            }
        }

        struct EntityDistanceSorter : IComparer<SceneEntity>
        {
            Vector2 origin;

            public EntityDistanceSorter(Vector2 origin)
            {
                this.origin = origin;
            }

            public int Compare(SceneEntity x, SceneEntity y)
            {
                Vector2 p1 = x.NaviComp.Position;
                Vector2 p2 = y.NaviComp.Position;
                return (int)((p1 - origin).sqrMagnitude - (p2 - origin).sqrMagnitude);
            }
        }

        #endregion
    }
}
