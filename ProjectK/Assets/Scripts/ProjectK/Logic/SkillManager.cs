using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class SkillManager
    {
        /// <summary>
        /// 技能合成的元素数量
        /// </summary>
        public const int ComposeElementCount = 3;

        /// <summary>
        /// 用于合成技能暂存
        /// </summary>
        private double[] elementWeights = new double[(int)DamageType.TotalElements];

        /// <summary>
        /// 用于合成技能暂存
        /// </summary>
        private int[] elements = new int[ComposeElementCount];

        /// <summary>
        /// 使用原材料合成技能
        /// </summary>
        public void ComposeSkill(List<ItemMaterial> materials)
        {
            // 统计所有原料的能量值和元素占比权重
            int totalEnergy = 0;
            Array.Clear(elementWeights, 0, elementWeights.Length);
            foreach (ItemMaterial material in materials)
            {
                DamageType[] types = material.ElementTypes;
                double[] percents = material.ElementPercents;
                for (int i = 0; i < types.Length; ++i)
                    elementWeights[(int)types[i]] += percents[i] * material.Energy;
                totalEnergy += material.Energy;
            }

            // 根据权重随机选取3个元素
            for (int i = 0; i < ComposeElementCount; ++i)
                elements[i] = Helpers.RandomWeights(elementWeights);

            // 根据 光-暗-金-木-水-火-土 的顺序排序
            Array.Sort(elements);

            // 根据能量获取能级
            // TODO:
            int energyLevel = 1;

            // 查询技能合成表并合成技能
            SkillCompositeSetting skillCompositeSetting = SettingManager.Instance.SkillCompositeSettings.GetValue(elements[0], elements[1], elements[2], energyLevel);
            int skillIndex = Helpers.RandomWeights(skillCompositeSetting.SkillWeightArr);
            int skillID = skillCompositeSetting.SkillIDArr[skillIndex];

        }
    }
}
