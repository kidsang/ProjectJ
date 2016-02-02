using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public class Player
    {
        public static Player Me = new Player();

        public List<int> SelectedTowers = new List<int>();

        /// <summary>
        /// 怪物掉落原料的背包
        /// </summary>
        public ItemBag MaterialBag = new ItemBag();

        /// <summary>
        /// 合成技能暂存的背包
        /// </summary>
        public UniqueItemBag SkillItemBag = new UniqueItemBag();
    }
}
