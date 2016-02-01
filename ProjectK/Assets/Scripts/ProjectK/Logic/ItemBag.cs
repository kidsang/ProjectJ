using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class ItemBag
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public Item AddItem(int itemID, int itemCount = 1)
        {
            if (itemCount <= 0)
                return null;

            Item item;
            if (!Items.TryGetValue(itemID, out item))
            {
                item = new Item(itemID, itemCount);
                Items[itemID] = item;
            }
            else
            {
                item.Count += itemCount;
            }
            return item;
        }

        public Item RemoveItem(int itemID, int itemCount)
        {
            if (itemCount <= 0)
                return null;

            Item item;
            if (!Items.TryGetValue(itemID, out item))
                return null;

            Log.Assert(itemCount <= item.Count);

            if (itemCount == item.Count)
            {
                Items.Remove(itemID);
                return item;
            }
            else
            {
                item.Count -= itemCount;
                return new Item(itemID, itemCount);
            }
        }

        public int CountItem(int itemID)
        {
            Item item;
            if (!Items.TryGetValue(itemID, out item))
                return 0;
            return item.Count;
        }
    }
}