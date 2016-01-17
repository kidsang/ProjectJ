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

        /// <summary>
        /// 如果需要特殊的BuffComp，则设置该属性
        /// </summary>
        public bool Special;

        /// <summary>
        /// 在Buff开始时应用，Buff结束时去除的属性
        /// </summary>
        public string AttrNames;
        public string AttrValues;
        public AttrName[] AttrNameArr;
        public double[] AttrValueArr;

        /// <summary>
        /// 每隔一段时间应用一次的属性
        /// </summary>
        public float UpdateInterval;
        public string UpdateAttrNames;
        public string UpdateAttrValues;
        public AttrName[] UpdateAttrNameArr;
        public double[] UpdateAttrValueArr;

        public override object GetKey()
        {
            return ID;
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

        public override void OnCheck()
        {
            if (AttrNameArr != null)
            {
                Check(AttrNameArr.Length == AttrValueArr.Length);
            }

            if (UpdateAttrNameArr != null)
            {
                Check(UpdateAttrNameArr.Length == UpdateAttrValueArr.Length);
            }
        }
    }
}
