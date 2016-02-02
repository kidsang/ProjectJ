using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class SkillCompositeSetting : CsvFileObject
    {
        public DamageType Element1;
        public DamageType Element2;
        public DamageType Element3;
        public int Energy;
        public string SkillIDs;
        public string SkillWeights;

        public int[] SkillIDArr;
        public int[] SkillWeightArr;

        public SkillCompositeSetting()
        {

        }

        public override object GetKey()
        {
            return BuildMultiKey((int)Element1, (int)Element2, (int)Element3, Energy);
        }

        public override void OnComplete()
        {
            SkillIDArr = ParseIntArray(SkillIDs);
            SkillWeightArr = ParseIntArray(SkillWeights);
        }

        public override void OnCheck()
        {
            Check(SkillIDArr.Length > 0);
            Check(SkillIDArr.Length == SkillWeightArr.Length);
        }
    }
}
