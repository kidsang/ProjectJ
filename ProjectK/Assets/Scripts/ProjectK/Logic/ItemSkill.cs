using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class ItemSkill : Item
    {
        public SkillSetting SkillSetting;
        public int Energy;

        public ItemSkill()
            : base(0, 1)
        {
        }
    }
}