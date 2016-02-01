using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class UniqueItem : Item
    {
        public Guid Guid { get; private set; }

        public UniqueItem(int itemID, int itemCount = 1)
            : base(itemID, itemCount)
        {
            Guid = Guid.NewGuid();
        } 
    }
}