using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public class TowerEntitySetting : EntitySetting
    {
        public double Atk;
        public double AtkSpeed;
        public float AtkRange;
        public string AtkTypes;
        public string Icon;

        public DamageType[] AtkTypeArr;

        public override void OnComplete()
        {
            AtkTypeArr = ParseEnumArray<DamageType>(AtkTypes);
        }
    }
}
