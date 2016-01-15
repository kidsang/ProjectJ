using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class BuffSetting : CsvFileObject
    {
        public int ID;
        public int Level;

        public string AttrNames;
        public string AttrValues;

        public int UpdateInterval;
        public string UpdateAttrNames;
        public string UpdateAttrValues;

        public AttrName[] AttrNameArr;
        public double[] AttrValueArr;
        public AttrName[] UpdateAttrNameArr;
        public double[] UpdateAttrValueArr;

        public override object GetKey()
        {
            return BuildMultiKey(ID, Level);
        }

        public override void OnComplete()
        {
            if (AttrNames != null)
            {
                AttrNameArr = ParseEnumArray<AttrName>(AttrNames);
                AttrValueArr = ParseDoubleArray(AttrValues);
            }

            if (UpdateAttrNames != null)
            {
                UpdateAttrNameArr = ParseEnumArray<AttrName>(UpdateAttrNames);
                UpdateAttrValueArr = ParseDoubleArray(UpdateAttrValues);
            }
        }
    }
}
