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
            return Time.time - lastAttackTime > atkInterval;
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
    }
}
