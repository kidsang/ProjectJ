using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    /// <summary>
    /// 怪物掉落的原材料，用于合成技能
    /// </summary>
    public class ItemMaterial : Item
    {
        public ItemMaterial(int itemID, int itemCount)
            : base(itemID, itemCount)
        {

        }

        public DamageType[] ElementTypes
        {
            get { return (Setting as ItemMaterialSetting).ElementTypeArr; }
        }

        public double[] ElementPercents
        {
            get { return (Setting as ItemMaterialSetting).ElementPercentArr; }
        }

        public int Energy
        {
            get { return (Setting as ItemMaterialSetting).Energy; }
        }
    }
}