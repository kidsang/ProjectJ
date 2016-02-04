using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class SceneEntity : DisposableBehaviour
    {
        public ResourceLoader Loader { get; private set; }

        /// <summary>
        /// 对应的配置文件
        /// </summary>
        public EntitySetting Template { get; private set; }

        /// <summary>
        /// 对应的模板ID
        /// </summary>
        public int TemplateID { get; private set; }

        /// <summary>
        /// 场景中的唯一ID
        /// </summary>
        public uint UID { get; set; }

        /// <summary>
        /// 所属阵营
        /// </summary>
        public CampType Camp { get; set; }

        /// <summary>
        /// 该Entity所在场景，由Scene.AddEntityToScene()设置
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// 该Entity所在的格子，由Map.UpdateSceneEntityCell()设置
        /// </summary>
        public MapCell Cell { get; set; }

        public Dictionary<Type, GameComp> CompDict { get; private set; }
        public List<GameComp> CompList { get; private set; }
        public AttrComp AttrComp { get; private set; }
        public NaviComp NaviComp { get; private set; }

        /// <summary>
        /// 是否显示debugDraw
        /// </summary>
        public bool ShowDebugDraw;

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

            CompDict = new Dictionary<Type, GameComp>();
            CompList = new List<GameComp>();
            // 所有人都有AttrComp
            AttrComp = AddComp<AttrComp>();
            // 所有人都有NaviComp
            NaviComp = AddComp<NaviComp>();
        }

        override protected void OnDispose()
        {
            if (CompList != null)
            {
                for (int i = CompList.Count - 1; i >= 0; --i)
                    CompList[i].Destroy();
                CompList = null;
            }

            base.OnDispose();
        }

        public virtual void Start()
        {
            int numComps = CompList.Count;
            for (int i = 0; i < numComps; ++i)
            {
                GameComp comp = CompList[i];
                comp.Start();
            }
        }

        public virtual void Activate()
        {
            int numComps = CompList.Count;
            for (int i = 0; i < numComps; ++i)
            {
                GameComp comp = CompList[i];
                if (comp.Started)
                {
                    comp.Activate();
                    if (ShowDebugDraw)
                        comp.UpdateDebugDraw();
                }
            }
        }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        public T AddComp<T>(bool start = false) where T: GameComp, new()
        {
            Type type = typeof(T);
            if (CompDict.ContainsKey(type))
            {
                Log.Error("重复添加组件！ Entity:", this, "CompType:", type);
                return null;
            }

            GameComp comp = new T();
            CompDict[type] = comp;
            CompList.Add(comp);
            comp.Entity = this;

            if (start)
                comp.Start();
            return (T)comp;
        }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        public GameComp AddComp(Type type, bool start = false)
        {
            if (CompDict.ContainsKey(type))
            {
                Log.Error("重复添加组件！ Entity:", this, "CompType:", type);
                return null;
            }

            GameComp comp = Activator.CreateInstance(type) as GameComp;
            CompDict[type] = comp;
            CompList.Add(comp);
            comp.Entity = this;

            if (start)
                comp.Start();
            return comp;
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        public T GetComp<T>() where T: GameComp
        {
            Type type = typeof(T);
            return (T)GetComp(type);
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        public GameComp GetComp(Type type)
        {
            GameComp comp;
            CompDict.TryGetValue(type, out comp);
            return comp;
        }

        /// <summary>
        /// 删除一个组件，并自动调用comp.destroy()
        /// </summary>
        public void DelComp<T>() where T : GameComp
        {
            Type type = typeof(T);
            DelComp(type);
        }

        /// <summary>
        /// 删除一个组件，并自动调用comp.destroy()
        /// </summary>
        public void DelComp(Type type)
        {
            GameComp comp;
            if (!CompDict.TryGetValue(type, out comp))
                return;

            CompDict.Remove(type);
            CompList.Remove(comp);
            comp.Destroy();
        }

        public Vector3 Position
        {
            get { return NaviComp.Position; }
            set { NaviComp.Position = value; }
        }

        public Vector2 Location
        {
            get { return NaviComp.Location; }
            set { NaviComp.Location = value; }
        }

        #region 战斗计算相关回调
        /// <summary>
        /// 在Fromula.AttackCalc前调用
        /// </summary>
        public delegate void BeforeAttackCalcCallback(SceneEntity fromEntity, SceneEntity targetEntity);
        public event BeforeAttackCalcCallback OnBeforeAttackCalc;

        /// <summary>
        /// 在Fromula.AttackCalc后调用
        /// </summary>
        public delegate void AfterAttackCalcCallback(SceneEntity fromEntity, SceneEntity targetEntity, AttackCalcResult result);
        public event AfterAttackCalcCallback OnAfterAttackCalc;

        /// <summary>
        /// 仅攻击者在Formula.DoAttack后调用
        /// </summary>
        public delegate void AttackCallback(SceneEntity targetEntity, AttackCalcResult result);
        public event AttackCallback OnAttack;

        /// <summary>
        /// 仅被击者在Formula.DoAttack后调用
        /// </summary>
        public delegate void AttackedCallback(SceneEntity fromEntity, AttackCalcResult result);
        public event AttackedCallback OnAttacked;

        public void TryFireOnBeforeAttackCalcEvent(SceneEntity fromEntity, SceneEntity targetEntity)
        {
            if (OnBeforeAttackCalc != null)
                OnBeforeAttackCalc(fromEntity, targetEntity);
        }

        public void TryFireOnAfterAttackCalcEvent(SceneEntity fromEntity, SceneEntity targetEntity, AttackCalcResult result)
        {
            if (OnAfterAttackCalc != null)
                OnAfterAttackCalc(fromEntity, targetEntity, result);
        }

        public void TryFireOnAttackEvent(SceneEntity targetEntity, AttackCalcResult result)
        {
            if (OnAttack != null)
                OnAttack(targetEntity, result);
        }

        public void TryFireOnAttackedEvent(SceneEntity fromEntity, AttackCalcResult result)
        {
            if (OnAttacked != null)
                OnAttacked(fromEntity, result);
        }
        #endregion
    }
}
