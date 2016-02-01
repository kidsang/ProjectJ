using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class ItemTypeSetting : CsvFileObject
    {
        public ItemType ID;
        public string Name;
        public int StartItemID;
        public int EndItemID;

        public override object GetKey()
        {
            return (int)ID;
        }
    }
}
