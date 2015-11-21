using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    /// <summary>
    /// 伤害和护甲类型
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// 神圣伤害/神圣护甲
        /// </summary>
        Holy,

        /// <summary>
        /// 光明伤害/光明护甲
        /// </summary>
        Light,

        /// <summary>
        /// 黑暗伤害/黑暗护甲
        /// </summary>
        Dark,

        /// <summary>
        /// 物理伤害/物理护甲
        /// </summary>
        Physics,

        /// <summary>
        /// 自然伤害/自然护甲
        /// </summary>
        Nature,

        /// <summary>
        /// 寒冰伤害/寒冰护甲
        /// </summary>
        Ice,

        /// <summary>
        /// 火焰伤害/火焰护甲
        /// </summary>
        Fire,

        /// <summary>
        /// 大地伤害/大地护甲
        /// </summary>
        Earth,

        /// <summary>
        /// 伤害/护甲类型总计
        /// </summary>
        Total,
    }
}
