using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public enum EntityType
    {
        Invalid = 0,
        Monster,
        Turret,
        Bullet,
    }

    public class SceneEntity : DisposableBehaviour
    {
        public ResourceLoader Loader { get; private set; }
        public EntitySetting Template { get; private set; }
        public int TemplateID { get; private set; }
        public EntityType Type { get; protected set; }

        public Dictionary<Type, GameComp> CompDict { get; private set; }
        public List<GameComp> CompList { get; private set; }
        public AttrComp AttrComp { get; private set; }
        public NaviComp NaviComp { get; private set; }

        public virtual void Init(ResourceLoader loader, EntitySetting template)
        {
            Loader = loader;
            Template = template;
            TemplateID = template.ID;

            Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
            body.isKinematic = true;

            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(Template.Width, Template.Height);
            collider.isTrigger = true;
        }

        public virtual void Start()
        {
            AttrComp = GetComp<AttrComp>();
            NaviComp = GetComp<NaviComp>();

            int numComps = CompList.Count;
            for (int i = 0; i < numComps; ++i)
            {
                GameComp comp = CompList[i];
                comp.Start();
            }
        }

        public virtual void Activate(Scene scene)
        {
        }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        public void AddComp<T>(bool start = false) where T: GameComp, new()
        {
            Type type = typeof(T);
            if (CompDict.ContainsKey(type))
            {
                Log.Error("重复添加组件！ Entity:", this, "CompType:", type);
                return;
            }

            GameComp comp = new T();
            CompDict[type] = comp;
            CompList.Add(comp);
            comp.Entity = this;

            if (start)
                comp.Start();
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        public T GetComp<T>() where T: GameComp
        {
            Type type = typeof(T);
            GameComp comp;
            CompDict.TryGetValue(type, out comp);
            return (T)comp;
        }

        /// <summary>
        /// 删除一个组件，并自动调用comp.destroy()
        /// </summary>
        public void DelComp<T>() where T : GameComp
        {
            Type type = typeof(T);
            GameComp comp;
            if (!CompDict.TryGetValue(type, out comp))
                return;

            CompDict.Remove(type);
            CompList.Remove(comp);
            comp.Destroy();
        }

        public Vector2 Location
        {
            get { return MapUtils.PositionToLocation(gameObject.transform.position); }
        }
    }
}
