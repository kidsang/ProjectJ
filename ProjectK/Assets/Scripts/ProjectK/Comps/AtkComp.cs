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
        public SceneEntity LockTarget { get; set; }

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
                return;

            if (LockTarget == null)
            {
                if (!CollectTargets())
                    return;
                LockTarget = TargetEntities[0];
            }

            lastAttackTime = Time.time;
            Entity.Scene.FireBullet(Entity.UID, LockTarget.UID);
        }

        public bool Attack()
        {
            if (InCD())
                return false;

            if (!CollectTargets())
                return false;

            return true;
        }

        public bool InCD()
        {
            double atkSpeed = attrComp.AtkSpeed;
            if (atkSpeed == 0)
                return false;

            float atkInterval = (float)(1 / atkSpeed);
            return Time.time - lastAttackTime <= atkInterval;
        }

        public bool CollectTargets()
        {
            TargetEntities.Clear();
            SceneManager.Instance.Scene.CollectEntitiesCircle(naviComp.Position, attrComp.AtkRange, TargetEntities);
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

    }
}
