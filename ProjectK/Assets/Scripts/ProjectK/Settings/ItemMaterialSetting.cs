using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class ItemMaterialSetting : ItemSetting
    {
        public string ElementTypes;
        public string ElementWeights;
        public int Energy;

        public DamageType[] ElementTypeArr;
        public double[] ElementPercentArr;

        public override void OnComplete()
        {
            base.OnComplete();
            ElementTypeArr = ParseEnumArray<DamageType>(ElementTypes);
            ElementPercentArr = ParseDoubleArray(ElementWeights);
            int numElements = ElementPercentArr.Length;
            if (numElements > 0)
            {
                double factor = 1.0 / ElementPercentArr.Sum();
                for (int i = 0; i < numElements; ++i)
                    ElementPercentArr[i] *= factor;
            }
        }

        public override void OnCheck()
        {
            Check(ElementTypeArr.Length == ElementPercentArr.Length);
            Check(Energy >= 0);
        }
    }
}
