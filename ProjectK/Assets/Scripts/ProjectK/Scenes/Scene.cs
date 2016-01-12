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

        private uint nextEntityUid = 1;
        public Dictionary<uint, SceneEntity> EntityDict { get; private set; }
        public List<SceneEntity> EntityList { get; private set; }

        private uint nextBulletUID = 1;
        public Dictionary<uint, Bullet> BulletDict { get; private set; }
        public List<Bullet> BulletList { get; private set; }

        public void Init()
        {
            sceneRoot = gameObject;
            loader = new ResourceLoader();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform, false);

            Map = mapRoot.AddComponent<Map>();
            Map.Init(loader);

            EntityDict = new Dictionary<uint, SceneEntity>();
            EntityList = new List<SceneEntity>();
            BulletDict = new Dictionary<uint, Bullet>();
            BulletList = new List<Bullet>();
            SpawnManager = new SpawnManager();
        }

        protected override void OnDispose()
        {
            if (BulletDict != null)
            {
                foreach (var bullet in BulletDict.Values)
                    bullet.Dispose();
                BulletDict = null;
                BulletList = null;
            }

            if (EntityDict != null)
            {
                foreach (var sceneEntity in EntityDict.Values)
                    sceneEntity.Dispose();
                EntityDict = null;
                EntityList = null;
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

            //SpawnManager.Start();
        }

        private void FixedUpdate()
        {
            if (!Playing)
                return;

            Time = UnityEngine.Time.fixedTime - startTime;
            DeltaTime = UnityEngine.Time.fixedDeltaTime;

            SpawnManager.Activate(this, Time);

            for (int i = EntityList.Count - 1; i >= 0; --i)
            {
                SceneEntity entity = EntityList[i];
                entity.Activate();
            }

            for (int i = BulletList.Count - 1; i >= 0; --i)
            {
                Bullet bullet = BulletList[i];
                bullet.Activate();
            }
        }

        #region 创建SceneEntity接口

        public void CreateMonster(int pathIndex, int templateID, int count)
        {
            MapPath path = Map.Paths[pathIndex];
            for (int i = 0; i < count; ++i)
            {
                MonsterEntity monster = CreateMonsterEntity(templateID);
                AddEntityToScene(monster, path.StartPosition);
                monster.SetPath(path);
            }
        }

        public void AddTowerEntity(TowerEntity towerEntity, Vector3 position)
        {
            AddEntityToScene(towerEntity, position);

            MapCell cell = Map.GetCellByWorldXY(position);
            cell.SetFlag(MapCellFlag.CanWalk, false);
            Map.CalculatePaths();
            Map.ToggleShowPaths(true);

            for (int i = EntityList.Count - 1; i >= 0; --i)
            {
                SceneEntity entity = EntityList[i];
                if (entity is MonsterEntity)
                    (entity as MonsterEntity).InvalidWayPositions();
            }
        }

        public MonsterEntity CreateMonsterEntity(int templateID)
        {
            MonsterEntity monsterEntity = CreateSceneEntity<MonsterEntity>(templateID);
            monsterEntity.Camp = CampType.Enemy;
            return monsterEntity;
        }

        public TowerEntity CreateTowerEntity(int templateID)
        {
            TowerEntity towerEntity = CreateSceneEntity<TowerEntity>(templateID);
            towerEntity.Camp = CampType.Friend;
            return towerEntity;
        }

        public T CreateSceneEntity<T>(int templateID) where T : SceneEntity
        {
            T sceneEntity = SceneEntityManager.Create<T>(loader, templateID);
            sceneEntity.UID = nextEntityUid++;
            EntityDict[sceneEntity.UID] = sceneEntity;
            EntityList.Add(sceneEntity);
            return sceneEntity;
        }

        public void AddEntityToScene(SceneEntity sceneEntity, Vector3? position = null)
        {
            sceneEntity.gameObject.transform.SetParent(sceneRoot.transform, false);

            if (position != null)
                sceneEntity.NaviComp.Position = position.Value;

            Map.UpdateSceneEntityCell(sceneEntity);
            sceneEntity.Scene = this;
        }

        public void DestroyEntity(SceneEntity sceneEntity)
        {
            if (EntityDict.ContainsKey(sceneEntity.UID))
            {
                EntityDict.Remove(sceneEntity.UID);
                EntityList.Remove(sceneEntity);
            }
            sceneEntity.Dispose();
        }

        public void DestroyEntity(uint entityUID)
        {
            SceneEntity sceneEntity;
            if (EntityDict.TryGetValue(entityUID, out sceneEntity))
            {
                EntityDict.Remove(entityUID);
                EntityList.Remove(sceneEntity);
                sceneEntity.Dispose();
            }
        }

        public SceneEntity GetEntity(uint entityUID)
        {
            SceneEntity sceneEntity;
            EntityDict.TryGetValue(entityUID, out sceneEntity);
            return sceneEntity;
        }

        #endregion

        #region 创建子弹接口

        // TODO: test
        public Bullet FireBullet(uint fromEntityUID, uint targetEntityUID)
        {
            SceneEntity fromEntity = GetEntity(fromEntityUID);
            if (fromEntity == null)
                Log.Error("FireBullet错误，找不到来源。fromEntityUID:", fromEntityUID);

            SceneEntity targetEntity = GetEntity(targetEntityUID);
            if (targetEntity == null)
                Log.Error("FireBullet错误，找不到目标。targetEntityUID:", targetEntityUID);

            GameObject gameObject = loader.LoadPrefab("Bullet/Bullet").Instantiate();
            Bullet bullet = gameObject.AddComponent<Bullet>();
            bullet.UID = nextBulletUID++;
            bullet.FromEntityUID = fromEntityUID;
            bullet.TargetEntityUID = targetEntityUID;
            BulletDict[bullet.UID] = bullet;
            BulletList.Add(bullet);

            gameObject.transform.position = fromEntity.Position;

            return bullet;
        }

        public void DestroyBullet(Bullet bullet)
        {
            if (BulletDict.ContainsKey(bullet.UID))
            {
                BulletDict.Remove(bullet.UID);
                BulletList.Remove(bullet);
            }
            bullet.Dispose();
        }

        public void DestroyBullet(uint bulletUID)
        {
            Bullet bullet;
            if (BulletDict.TryGetValue(bulletUID, out bullet))
            {
                BulletDict.Remove(bulletUID);
                BulletList.Remove(bullet);
                bullet.Dispose();
            }
        }

        #endregion

        #region 范围找怪接口

        /// <summary>
        /// 收集圆形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesCircle(Vector2 origin, float radius,
            List<SceneEntity> resultEntities = null, CampType campType = CampType.All, bool sortByDistance = false)
        {
            return CollectEntities(origin, radius, 0, 0, CollectEntityCircleFunc, resultEntities, campType, sortByDistance);
        }

        /// <summary>
        /// 收集扇形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesFan(Vector2 origin, float radius, float rangeAngle, float rotateAngle,
            List<SceneEntity> resultEntities = null, CampType campType = CampType.All, bool sortByDistance = false)
        {
            return CollectEntities(origin, radius, rangeAngle, rotateAngle, CollectEntityFanFunc, resultEntities, campType, sortByDistance);
        }

        /// <summary>
        /// 收集矩形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesRectangle(Vector2 origin, float width, float height, float rotateAngle,
            List<SceneEntity> resultEntities = null, CampType campType = CampType.All, bool sortByDistance = false)
        {
            return CollectEntities(origin, width, height, rotateAngle, CollectEntityRectangleFunc, resultEntities, campType, sortByDistance);
        }

        /// <summary>
        /// 收集环形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesRing(Vector2 origin, float radius, float innerRadius,
            List<SceneEntity> resultEntities = null, CampType campType = CampType.All, bool sortByDistance = false)
        {
            return CollectEntities(origin, radius, innerRadius, 0, CollectEntityRingFunc, resultEntities, campType, sortByDistance);
        }

        /// <summary>
        /// 收集中心矩形范围内的目标
        /// </summary>
        public List<SceneEntity> CollectEntitiesCenterRect(Vector2 origin, float width, float height, float rotateAngle,
            List<SceneEntity> resultEntities = null, CampType campType = CampType.All, bool sortByDistance = false)
        {
            return CollectEntities(origin, width, height, rotateAngle, CollectEntityCenterRectFunc, resultEntities, campType, sortByDistance);
        }

        private List<SceneEntity> CollectEntities(Vector2 origin, float param1, float param2, float param3, CollectEntityFunc collectFunc,
            List<SceneEntity> resultEntities = null, CampType campType = CampType.All, bool sortByDistance = false)
        {
            if (resultEntities == null)
                resultEntities = new List<SceneEntity>();

            collectFunc(origin, param1, param2, param3, resultEntities);

            if (campType != CampType.All)
            {
                for (int i = resultEntities.Count - 1; i >= 0; --i)
                {
                    if (resultEntities[i].Camp != campType)
                        resultEntities.RemoveAt(i);
                }
            }

            if (sortByDistance)
            {
                EntityDistanceSorter sorter = new EntityDistanceSorter(origin);
                resultEntities.Sort(sorter);
            }

            return resultEntities;
        }

        delegate void CollectEntityFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities);

        private void CollectEntityCircleFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float radius2 = param1 * param1;
            foreach (SceneEntity entity in EntityDict.Values)
            {
                Vector2 position = entity.Position;
                Vector2 direction = position - origin;
                if (direction.sqrMagnitude <= radius2)
                    resultEntities.Add(entity);
            }
        }

        private void CollectEntityFanFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float radius2 = param1 * param1;
            float rangeAngleDiv2 = param2 / 2;
            float rotateAngle = param3;
            foreach (SceneEntity entity in EntityDict.Values)
            {
                Vector2 position = entity.Position;
                Vector2 direction = position - origin;
                if (direction.sqrMagnitude <= radius2)
                {
                    float angle = MapUtils.Angle(direction);
                    float deltaAngle = MapUtils.DeltaAngle(angle, rotateAngle);
                    if (deltaAngle <= rangeAngleDiv2)
                        resultEntities.Add(entity);
                }
            }
        }

        private void CollectEntityRectangleFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float halfWidth = param1 * 0.5f;
            float halfHeight = param2 * 0.5f;
            float rotateAngle = param3;
            Vector2 center = new Vector2(origin.x + Mathf.Cos(rotateAngle) * halfWidth, origin.y + Mathf.Sin(rotateAngle) * halfWidth);
            Quaternion rotate = Quaternion.AngleAxis(-rotateAngle * Mathf.Rad2Deg, MapUtils.Vector3Z);

            foreach (SceneEntity entity in EntityDict.Values)
            {
                Vector2 position = entity.Position;
                Vector2 delta = rotate * (position - center);
                if (Mathf.Abs(delta.x) <= halfWidth && Mathf.Abs(delta.y) <= halfHeight)
                    resultEntities.Add(entity);
            }
        }

        private void CollectEntityRingFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float radius2 = param1 * param1;
            float innerRadius2 = param2 * param2;
            foreach (SceneEntity entity in EntityDict.Values)
            {
                Vector2 position = entity.Position;
                Vector2 direction = position - origin;
                float sqrtLength = direction.sqrMagnitude;
                if (sqrtLength <= radius2 && sqrtLength >= innerRadius2)
                    resultEntities.Add(entity);
            }
        }

        private void CollectEntityCenterRectFunc(Vector2 origin, float param1, float param2, float param3, List<SceneEntity> resultEntities)
        {
            float halfWidth = param1 * 0.5f;
            float halfHeight = param2 * 0.5f;
            float rotateAngle = param3;
            Vector2 center = new Vector2(origin.x, origin.y);
            Quaternion rotate = Quaternion.AngleAxis(-rotateAngle * Mathf.Rad2Deg, MapUtils.Vector3Z);

            foreach (SceneEntity entity in EntityDict.Values)
            {
                Vector2 position = entity.Position;
                Vector2 delta = rotate * (position - center);
                if (Mathf.Abs(delta.x) <= halfWidth && Mathf.Abs(delta.y) <= halfHeight)
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
                Vector2 p1 = x.Position;
                Vector2 p2 = y.Position;
                return (int)((p1 - origin).sqrMagnitude - (p2 - origin).sqrMagnitude);
            }
        }

        #endregion
    }
}
