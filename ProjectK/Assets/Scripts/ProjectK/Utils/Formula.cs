using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    /// <summary>
    /// 用以存放伤害计算结果
    /// </summary>
    public class AttackCalcResult
    {
        /// <summary>
        /// 按伤害类型分的伤害值
        /// </summary>
        public double[] Damages = new double[(int)DamageType.Total];

        /// <summary>
        /// 伤害值总数
        /// </summary>
        public double TotalDamage
        {
            get { return Damages.Sum(); }
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

        public static AttackCalcResult AttackCalc(SceneEntity fromEntity, SceneEntity targetEntity, bool doAttack = true)
        {
            AttackCalcResult result = new AttackCalcResult();
            AttrComp fromAttrComp = fromEntity.GetComp<AttrComp>();
            AttrComp targetAttrComp = targetEntity.GetComp<AttrComp>();

            // 计算护甲减伤，公式为：
            // 1 - (Def * ARMOR_COEF) / (1 + Def * ARMOR_COEF)
            double defValue = targetAttrComp.Def * DEF_COEF;
            double damageRate = 1 - defValue / (1 + defValue);

            // 计算总伤害
            double damage = fromAttrComp.Atk * damageRate;

            // 按照伤害类型的数量平分伤害
            List<DamageType> atkTypes = fromAttrComp.AtkTypes;
            int numAtkTypes = atkTypes.Count;
            damage /= numAtkTypes;
            for (int i = 0; i < numAtkTypes; ++i)
            {
                DamageType atkType = atkTypes[i];
                result.Damages[(int)atkType] = damage;
            }

            // 1. 计算伤害加成和免伤
            // 2. 按攻击类型和护甲类型调整伤害值
            DamageType defType = targetAttrComp.DefType;
            int numDamageTypes = (int)DamageType.Total;
            for (int i = 0; i < numDamageTypes; ++i)
            {
                DamageType atkType = (DamageType)i;
                double singleDamage = result.Damages[i];

                // 百分比伤害加成减免
                singleDamage *= 1 + fromAttrComp.GetDamageAddRate(atkType) + fromAttrComp.DamageAddRate + targetAttrComp.BeDamageAddRate;
                // 伤害附加
                singleDamage += fromAttrComp.GetDamageAdd(atkType);
                // 属性相克伤害加成
                singleDamage *= DamageTypeSetting.GetDamageFactor(atkType, defType);

                result.Damages[i] = singleDamage;
            }

            if (doAttack)
                DoAttack(fromEntity, targetEntity, result);

            return result;
        }

        public static void DoAttack(SceneEntity fromEntity, SceneEntity targetEntity, AttackCalcResult result)
        {
            double damage = result.TotalDamage;
            // 应用伤害
            targetEntity.AttrComp.Hp -= damage;
            // 伤害数字和血条
            Helpers.ShowHpBar(targetEntity.gameObject, (float)targetEntity.AttrComp.HpPercent);
            Helpers.ShowHpText(targetEntity.gameObject, (int)-damage);
        }
    }
}
