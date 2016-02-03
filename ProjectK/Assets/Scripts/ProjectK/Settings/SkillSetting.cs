using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class SkillSetting : CsvFileObject
    {
        public int ID;
        public string Name;

        public override object GetKey()
        {
            return ID;
        }
    }
}
