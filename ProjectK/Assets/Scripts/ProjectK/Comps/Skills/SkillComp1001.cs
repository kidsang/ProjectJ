﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    /// <summary>
    /// 净化 附加n点光系伤害，使目标受到伤害增加x%，持续y秒
    /// </summary>
    public class SkillComp1001 : SkillComp
    {
        private const DamageType addDamageType = DamageType.Light;
        private const double addDamage = 10;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            Entity.AttrComp.AddDamageAdd(addDamageType, addDamage);
            RegisterOnBeforeAttackCalc(OnBeforeAttackCalc);
            return true;
        }

        public override void Destroy()
        {
            Entity.AttrComp.AddDamageAdd(addDamageType, -addDamage);
            UnregisterOnBeforeAttackCalc(OnBeforeAttackCalc);
            base.Destroy();
        }

        private void OnBeforeAttackCalc(SceneEntity fromEntity, SceneEntity targetEntity)
        {
            if (fromEntity != Entity)
                return;

            BuffMgrComp buffMgrComp = targetEntity.GetComp<BuffMgrComp>();
            if (buffMgrComp == null)
                return;

            buffMgrComp.AddBuff(fromEntity.UID, 1, 10);
        }
    }
}
