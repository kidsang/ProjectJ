﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class EntitySetting : CsvFileObject
    {
        public int ID;
        public string Name;
        public float Width;
        public float Height;
        public string Prefab;

        public override object GetKey()
        {
            return ID;
        }
    }
}
