﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class TowerEntity : SceneEntity
    {
        public AtkComp AtkComp { get; private set; }

        public override void Init(ResourceLoader loader, EntitySetting template)
        {
            base.Init(loader, template);

            AtkComp = AddComp<AtkComp>();

            TowerEntitySetting setting = (TowerEntitySetting)template;
            AttrComp.AtkBase = setting.Atk;
            AttrComp.AtkSpeedBase = setting.AtkSpeed;
            AttrComp.AtkRangeBase = setting.AtkRange;
            AttrComp.AtkTypes.AddRange(setting.AtkTypeArr);
        }
    }
}
