using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class DamageTypeSetting : CsvFileObject
    {
        public DamageType DefType;
        public double DamageFactor0;
        public double DamageFactor1;
        public double DamageFactor2;
        public double DamageFactor3;
        public double DamageFactor4;
        public double DamageFactor5;
        public double DamageFactor6;
        public double DamageFactor7;
        public double[] DamageFactors;

        public override string GetKey()
        {
            return DefType.ToString();
        }

        public override void OnComplete()
        {
            DamageFactors = FieldsToArray<double>("DamageFactor", 0, 7);
        }

        public double GetDamageFactor(DamageType atkType)
        {
            return DamageFactors[(int)atkType];
        }
    }
}
