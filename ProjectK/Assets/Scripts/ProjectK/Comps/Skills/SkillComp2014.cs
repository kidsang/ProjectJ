using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    /// <summary>
    /// 攻击有几率对目标造成x次伤害
    /// </summary>
    public class SkillComp2014 : SkillComp
    {
        private const float chance = 1;
        private const int atkCount = 1;

        //public override bool Start()
        //{
        //    if (!base.Start())
        //        return false;

        //    Entity.AttrComp.AddDamageAdd(addDamageType, addDamage);
        //    return true;
        //}

        //public override void Destroy()
        //{
        //    Entity.AttrComp.AddDamageAdd(addDamageType, -addDamage);
        //    base.Destroy();
        //}

        //private void OnBeforeFireBullet(AtkComp atkComp, List<Bullet> bullets)
        //{
        //}
    }
}
