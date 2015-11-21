using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    /// <summary>
    /// 用以存放伤害计算结果
    /// </summary>
    public struct SkillCalcResult
    {
        public double[] Damages;

        /// <summary>
        /// 请使用这个函数创建SkillCalcResult，而不是new SkillCalcResult()
        /// </summary>
        public static SkillCalcResult New()
        {
            SkillCalcResult result = new SkillCalcResult();
            result.Damages = new double[(int)DamageType.Total];
            return result;
        }
    }

    /// <summary>
    /// 用以进行伤害计算
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// 减伤系数
        /// </summary>
        public static readonly double DEF_COEF = 0.01;

        //public static void skillCalc()

        public static SkillCalcResult TestCalc(SceneEntity fromEntity, SceneEntity targetEntity)
        {
            SkillCalcResult result = SkillCalcResult.New();
            AttrComp fromAttrComp = fromEntity.GetComp<AttrComp>();
            AttrComp targetAttrComp = targetEntity.GetComp<AttrComp>();

            // 计算护甲减伤，公式为：
            // 1 - (Def * ARMOR_COEF) / (1 + Def * ARMOR_COEF)
            double defValue = targetAttrComp.Def * DEF_COEF;
            double damageRate = 1 - defValue / (1 + defValue);

            // 计算总伤害
            double damage = fromAttrComp.Atk * damageRate;

            // 根据攻击者的伤害类型和目标的护甲类型计算伤害值
            List<DamageType> atkTypes = fromAttrComp.AtkTypes;
            DamageType defType = targetAttrComp.DefType;
            int numAtkTypes = atkTypes.Count;
            damage /= numAtkTypes; // 按照伤害类型的数量平分伤害
            for (int i = 0; i < numAtkTypes; ++i)
            {
                DamageType atkType = atkTypes[i];
                damageRate = GetDamageRate(atkType, defType);
                result.Damages[(int)atkType] = damage * damageRate;
            }

            return result;
        }

        /// <summary>
        /// 根据伤害类型和护甲类型返回伤害系数
        /// </summary>
        public static double GetDamageRate(DamageType atkType, DamageType defType)
        {
            // TODO:
            return 1;
        }
    }
}
