using System;
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
        private readonly DamageType addDamageType = DamageType.Light;
        private readonly double addDamage = 10;
        private readonly int buffID = 0;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            Entity.AttrComp.AddDamageAdd(addDamageType, addDamage);
            return true;
        }

        public override void Destroy()
        {
            Entity.AttrComp.AddDamageAdd(addDamageType, -addDamage);
            base.Destroy();
        }
    }
}
