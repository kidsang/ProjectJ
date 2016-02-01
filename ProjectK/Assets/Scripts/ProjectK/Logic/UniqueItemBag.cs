using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class UniqueItemBag
    {
        public Dictionary<Guid, UniqueItem> Items = new Dictionary<Guid, UniqueItem>();

        public void AddItem(UniqueItem item)
        {
            Items[item.Guid] = item;
        }

        public UniqueItem GetItem(Guid guid)
        {
            UniqueItem item;
            Items.TryGetValue(guid, out item);
            return item;
        }

        public void RemoveItem(UniqueItem item)
        {
            Items.Remove(item.Guid);
        }

        public int CountItem(int itemID)
        {
            int count = 0;
            foreach (UniqueItem item in Items.Values)
            {
                if (item.ID == itemID)
                    count += 1;
            }
            return count;
        }
    }
}