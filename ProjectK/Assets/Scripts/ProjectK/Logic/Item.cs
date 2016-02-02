using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class Item
    {
        public int ID { get; private set; }
        public int Count { get; set; }
        public ItemSetting Setting { get; private set; }

        public Item(int itemID, int itemCount = 1)
        {
            ID = itemID;
            Count = itemCount;
            Setting = SettingManager.Instance.GetItemSetting(itemID);
        }
    }
}