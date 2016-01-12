using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class AtkComp : GameComp
    {
        private AttrComp attrComp;
        private NaviComp naviComp;

        private float lastAttackTime;
        public List<SceneEntity> TargetEntities { get; private set; }
        public SceneEntity AimTargetEntity { get; set; }

        private DebugDraw rangeDebugDraw;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            attrComp = Entity.AttrComp;
            naviComp = Entity.NaviComp;
            TargetEntities = new List<SceneEntity>();
            return true;
        }

        public override void Destroy()
        {
            attrComp = null;
            naviComp = null;
            base.Destroy();
        }

        public override void Activate()
        {
            base.Activate();

            if (InCD())
            {
                return;
            }

            if (!CollectTargets())
            {
                AimTargetEntity = null;
                return;
            }

            if (AimTargetEntity == null || !TargetEntities.Contains(AimTargetEntity))
            {
                AimTargetEntity = TargetEntities[0];
            }

            lastAttackTime = Time.time;

            List<Bullet> bullets = new List<Bullet> { Entity.Scene.FireBullet(Entity.UID, AimTargetEntity.UID) };

            if (OnBeforeFireBullet != null)
                OnBeforeFireBullet(this, bullets);

            if (OnAfterFireBullet != null)
                OnAfterFireBullet(this, bullets);
        }

        public bool InCD()
        {
            double atkSpeed = attrComp.AtkSpeed;
            if (atkSpeed == 0)
                return true;

            float atkInterval = (float)(1 / atkSpeed);
            return Time.time - lastAttackTime <= atkInterval;
        }

        public bool CollectTargets()
        {
            TargetEntities.Clear();
            SceneManager.Instance.Scene.CollectEntitiesCircle(naviComp.Position, attrComp.AtkRange, TargetEntities, CampType.Enemy);
            return TargetEntities.Count > 0;
        }

        public void ClearCD()
        {
            lastAttackTime = 0;
        }

        override public void UpdateDebugDraw()
        {
            if (rangeDebugDraw == null)
                rangeDebugDraw = DebugDraw.Create("AtkRangeDebug", Entity.gameObject, Color.yellow);

            rangeDebugDraw.DrawCircle(attrComp.AtkRange);
        }

        #region 战斗计算相关回调

        public delegate void BeforeFireBulletCallback(AtkComp atkComp, List<Bullet> bullets);
        public event BeforeFireBulletCallback OnBeforeFireBullet;

        public delegate void AfterFireBulletCallback(AtkComp atkComp, List<Bullet> bullets);
        public event AfterFireBulletCallback OnAfterFireBullet;

        #endregion

    }
}
