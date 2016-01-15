using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    /// <summary>
    /// 用以注册属性变化回调函数
    /// </summary>
    public enum AttrName
    {
        Hp,
        MaxHp,
        MaxHpBase,
        MaxHpAddRate,
        Atk,
        AtkBase,
        AtkAddRate,
        Def,
        DefBase,
        DefAddRate,
        AtkSpeed,
        AtkSpeedBase,
        AtkSpeedAddRate,
        AtkRange,
        AtkRangeBase,
        AtkRangeAddRate,
        MoveSpeed,
        MoveSpeedBase,
        MoveSpeedAddRate,
    }

    /// <summary>
    /// 通用属性组件
    /// </summary>
    public class AttrComp : GameComp
    {
        #region 生命值 hp maxHp maxHpBase maxHpAddRate hpPercent
        private double hp;
        private double maxHp;
        private double maxHpBase;
        private double maxHpAddRate;

        /// <summary>
        /// 当前血量
        /// </summary>
        public double Hp 
        {
            get { return hp; }
            set 
            {
                if (value == hp)
                    return;
                hp = value; 
                OnAttrChange(AttrName.Hp, value);
            }
        }

        /// <summary>
        /// 最大血量
        /// 如果最大血量小于当前血量，当前血量会调整为最大血量
        /// </summary>
        public double MaxHp
        {
            get { return maxHp; }
            private set
            {
                if (value == maxHp)
                    return;
                maxHp = value;

                if (maxHp < hp)
                    Hp = maxHp;

                OnAttrChange(AttrName.MaxHp, value);
            }
        }

        /// <summary>
        /// 最大血量基础值
        /// </summary>
        public double MaxHpBase
        {
            get { return maxHpBase; }
            set
            {
                if (value == maxHpBase)
                    return;
                maxHpBase = value;
                MaxHp = maxHpBase * (1 + maxHpAddRate);
            }
        }

        /// <summary>
        /// 最大血量百分比加成
        /// </summary>
        public double MaxHpAddRate
        {
            get { return maxHpAddRate; }
            set
            {
                if (value == maxHpAddRate)
                    return;
                maxHpAddRate = value;
                MaxHp = maxHpBase * (1 + maxHpAddRate);
            }
        }

        /// <summary>
        /// 当前血量百分比
        /// </summary>
        public double HpPercent
        {
            get { return Hp / MaxHp; }
        }
        #endregion

        # region 攻击力 atk atkBase atkAddRate
        private double atk;
        private double atkBase;
        private double atkAddRate;

        /// <summary>
        /// 攻击力
        /// </summary>
        public double Atk
        {
            get { return atk; }
            private set
            {
                if (value == atk)
                    return;
                atk = value;
                OnAttrChange(AttrName.Atk, value);
            }
        }

        /// <summary>
        /// 攻击力基础值
        /// </summary>
        public double AtkBase
        {
            get { return atkBase; }
            set
            {
                if (value == atkBase)
                    return;
                atkBase = value;
                Atk = atkBase * (1 + atkAddRate);
            }
        }

        /// <summary>
        /// 攻击力百分比加成
        /// </summary>
        public double AtkAddRate
        {
            get { return atkAddRate; }
            set
            {
                if (value == atkAddRate)
                    return;
                atkAddRate = value;
                Atk = atkBase * (1 + atkAddRate);
            }
        }
        # endregion

        # region 防御力 def defBase defAddRate
        private double def;
        private double defBase;
        private double defAddRate;

        /// <summary>
        /// 防御力
        /// </summary>
        public double Def
        {
            get { return def; }
            private set
            {
                if (value == def)
                    return;
                def = value;
                OnAttrChange(AttrName.Def, value);
            }
        }

        /// <summary>
        /// 防御力基础值
        /// </summary>
        public double DefBase
        {
            get { return defBase; }
            set
            {
                if (value == defBase)
                    return;
                defBase = value;
                Def = defBase * (1 + defAddRate);
            }
        }

        /// <summary>
        /// 防御力百分比加成
        /// </summary>
        public double DefAddRate
        {
            get { return defAddRate; }
            set
            {
                if (value == defAddRate)
                    return;
                defAddRate = value;
                Def = defBase * (1 + defAddRate);
            }
        }
        # endregion

        # region 攻速 atkSpeed atkSpeedBase atkSpeedAddRate
        private double atkSpeed;
        private double atkSpeedBase;
        private double atkSpeedAddRate;

        /// <summary>
        /// 攻速
        /// 攻速最低为0
        /// </summary>
        public double AtkSpeed
        {
            get { return atkSpeed; }
            private set
            {
                if (value < 0)
                    value = 0;
                if (value == atkSpeed)
                    return;
                atkSpeed = value;
                OnAttrChange(AttrName.AtkSpeed, value);
            }
        }

        /// <summary>
        /// 攻速基础值
        /// </summary>
        public double AtkSpeedBase
        {
            get { return atkSpeedBase; }
            set
            {
                if (value == atkSpeedBase)
                    return;
                atkSpeedBase = value;
                AtkSpeed = atkSpeedBase * (1 + atkSpeedAddRate);
            }
        }

        /// <summary>
        /// 攻速百分比加成
        /// </summary>
        public double AtkSpeedAddRate
        {
            get { return atkSpeedAddRate; }
            set
            {
                if (value == atkSpeedAddRate)
                    return;
                atkSpeedAddRate = value;
                AtkSpeed = atkSpeedBase * (1 + atkSpeedAddRate);
            }
        }
        # endregion

        # region 攻击范围 atkRange atkRangeBase atkRangeAddRate
        private float atkRange;
        private float atkRangeBase;
        private float atkRangeAddRate;

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float AtkRange
        {
            get { return atkRange; }
            private set
            {
                if (value < 0)
                    value = 0;
                if (value == atkRange)
                    return;
                atkRange = value;
                OnAttrChange(AttrName.AtkRange, value);
            }
        }

        /// <summary>
        /// 攻击范围基础值
        /// </summary>
        public float AtkRangeBase
        {
            get { return atkRangeBase; }
            set
            {
                if (value == atkRangeBase)
                    return;
                atkRangeBase = value;
                AtkRange = atkRangeBase * (1 + atkRangeAddRate);
            }
        }

        /// <summary>
        /// 攻击范围百分比加成
        /// </summary>
        public float AtkRangeAddRate
        {
            get { return atkRangeAddRate; }
            set
            {
                if (value == atkRangeAddRate)
                    return;
                atkRangeAddRate = value;
                AtkRange = atkRangeBase * (1 + atkRangeAddRate);
            }
        }
        # endregion

        # region 移动速度 moveSpeed moveSpeedBase moveSpeedAddRate
        private float moveSpeed;
        private float moveSpeedBase;
        private float moveSpeedAddRate;

        /// <summary>
        /// 移动速度
        /// 移动速度最低为0
        /// </summary>
        public float MoveSpeed
        {
            get { return moveSpeed; }
            private set
            {
                if (value < 0)
                    value = 0;
                if (value == moveSpeed)
                    return;
                moveSpeed = value;
                OnAttrChange(AttrName.MoveSpeed, value);
            }
        }

        /// <summary>
        /// 移动速度基础值
        /// </summary>
        public float MoveSpeedBase
        {
            get { return moveSpeedBase; }
            set
            {
                if (value == moveSpeedBase)
                    return;
                moveSpeedBase = value;
                MoveSpeed = moveSpeedBase * (1 + moveSpeedAddRate);
            }
        }

        /// <summary>
        /// 移动速度百分比加成
        /// </summary>
        public float MoveSpeedAddRate
        {
            get { return moveSpeedAddRate; }
            set
            {
                if (value == moveSpeedAddRate)
                    return;
                moveSpeedAddRate = value;
                MoveSpeed = moveSpeedBase * (1 + moveSpeedAddRate);
            }
        }
        # endregion

        # region 伤害类型 atkTypes defType
        private List<DamageType> atkTypes = new List<DamageType>();
        private DamageType defType;

        /// <summary>
        /// 伤害类型
        /// 可以具有多个伤害类型，伤害平分
        /// </summary>
        public List<DamageType> AtkTypes
        {
            get { return atkTypes; }
        }

        /// <summary>
        /// 护甲类型
        /// 只能有一种护甲类型
        /// </summary>
        public DamageType DefType
        {
            get { return defType; }
            set { defType = value; }
        }
        # endregion

        #region 属性附加伤害 damageAdds damageAddRates
        private double[] damageAdds = new double[(int)DamageType.Total];
        private double[] damageAddRates = new double[(int)DamageType.Total];

        /// <summary>
        /// 属性附加伤害
        /// </summary>
        public double GetDamageAdd(DamageType type)
        {
            return damageAdds[(int)type];
        }

        /// <summary>
        /// 属性附加伤害
        /// </summary>
        public void SetDamageAdd(DamageType type, double value)
        {
            damageAdds[(int)type] = value;
        }

        /// <summary>
        /// 属性附加伤害
        /// </summary>
        public void AddDamageAdd(DamageType type, double value)
        {
            SetDamageAdd(type, GetDamageAdd(type) + value);
        }

        /// <summary>
        /// 属性附加伤害
        /// </summary>
        public double GetDamageAddRate(DamageType type)
        {
            return damageAddRates[(int)type];
        }

        /// <summary>
        /// 属性附加伤害
        /// </summary>
        public void SetDamageAddRate(DamageType type, double value)
        {
            damageAddRates[(int)type] = value;
        }

        /// <summary>
        /// 属性附加伤害
        /// </summary>
        public void AddDamageAddRate(DamageType type, double value)
        {
            SetDamageAddRate(type, GetDamageAddRate(type) + value);
        }
        #endregion

        #region 总伤害增加/减免 damageAddRate beDamageAddRate
        /// <summary>
        /// 造成伤害加成百分比
        /// </summary>
        public double DamageAddRate { get; set; }

        /// <summary>
        /// 受到伤害加成百分比
        /// </summary>
        public double BeDamageAddRate { get; set; }
        #endregion


        public delegate void AttrChangeCallback(double newValue);
        private Dictionary<AttrName, List<AttrChangeCallback>> attrChangeCallbackDict = new Dictionary<AttrName, List<AttrChangeCallback>>();

        /// <summary>
        /// 注册属性变化时的回调函数
        /// </summary>
        public void RegisterAttrChangeCallback(AttrName attrName, AttrChangeCallback callback)
        {
            List<AttrChangeCallback> callbackList;
            if (attrChangeCallbackDict.TryGetValue(attrName, out callbackList))
            {
                if (callbackList.Contains(callback))
                    return;
            }
            else
            {
                callbackList = new List<AttrChangeCallback>();
                attrChangeCallbackDict[attrName] = callbackList;
            }
            callbackList.Add(callback);
        }

        /// <summary>
        /// 反注册属性变化时的回调函数
        /// </summary>
        public void UnrigisterAttrChangeCallback(AttrName attrName, AttrChangeCallback callback)
        {
            List<AttrChangeCallback> callbackList;
            if (attrChangeCallbackDict.TryGetValue(attrName, out callbackList))
                callbackList.Remove(callback);
        }

        /// <summary>
        /// 通知属性变化
        /// </summary>
        private void OnAttrChange(AttrName attrName, double newValue)
        {
            List<AttrChangeCallback> callbackList;
            if (attrChangeCallbackDict.TryGetValue(attrName, out callbackList))
            {
                callbackList = new List<AttrChangeCallback>(callbackList);
                int count = callbackList.Count;
                for (int i = 0; i < count; ++i)
                {
                    AttrChangeCallback callback = callbackList[i];
                    callback(newValue);
                }
            }
        }

        /// <summary>
        /// 根据AttrName返回属性值
        /// </summary>
        public double GetValue(AttrName attrName)
        {

            switch (attrName)
            {
                case AttrName.Hp:
                    return Hp;
                case AttrName.MaxHp:
                    return MaxHp;
                case AttrName.MaxHpBase:
                    return MaxHpBase;
                case AttrName.MaxHpAddRate:
                    return MaxHpAddRate;
                case AttrName.Atk:
                    return Atk;
                case AttrName.AtkBase:
                    return AtkBase;
                case AttrName.AtkAddRate:
                    return AtkAddRate;
                case AttrName.Def:
                    return Def;
                case AttrName.DefBase:
                    return DefBase;
                case AttrName.DefAddRate:
                    return DefAddRate;
                case AttrName.AtkSpeed:
                    return AtkSpeed;
                case AttrName.AtkSpeedBase:
                    return AtkSpeedBase;
                case AttrName.AtkSpeedAddRate:
                    return AtkSpeedAddRate;
                case AttrName.AtkRange:
                    return AtkRange;
                case AttrName.AtkRangeBase:
                    return AtkRangeBase;
                case AttrName.AtkRangeAddRate:
                    return AtkRangeAddRate;
                case AttrName.MoveSpeed:
                    return MoveSpeed;
                case AttrName.MoveSpeedBase:
                    return MoveSpeedBase;
                case AttrName.MoveSpeedAddRate:
                    return MoveSpeedAddRate;
                default:
                    Log.Error("非法的AttrName！", attrName);
                    return 0.0;
            }
        }

        /// <summary>
        /// 根据AttrName设置属性值
        /// </summary>
        public void SetValue(AttrName attrName, double value)
        {
            switch (attrName)
            {
                case AttrName.Hp:
                    Hp = value;
                    break;
                case AttrName.MaxHpBase:
                    MaxHpBase = value;
                    break;
                case AttrName.MaxHpAddRate:
                    MaxHpAddRate = value;
                    break;
                case AttrName.AtkBase:
                    AtkBase = value;
                    break;
                case AttrName.AtkAddRate:
                    AtkAddRate = value;
                    break;
                case AttrName.DefBase:
                    DefBase = value;
                    break;
                case AttrName.DefAddRate:
                    DefAddRate = value;
                    break;
                case AttrName.AtkSpeedBase:
                    AtkSpeedBase = value;
                    break;
                case AttrName.AtkSpeedAddRate:
                    AtkSpeedAddRate = value;
                    break;
                case AttrName.AtkRangeBase:
                    AtkRangeBase = (float)value;
                    break;
                case AttrName.AtkRangeAddRate:
                    AtkRangeAddRate = (float)value;
                    break;
                case AttrName.MoveSpeedBase:
                    MoveSpeedBase = (float)value;
                    break;
                case AttrName.MoveSpeedAddRate:
                    MoveSpeedAddRate = (float)value;
                    break;
                default:
                    Log.Error("非法的AttrName！", attrName);
                    break;
            }
        }

        /// <summary>
        /// 根据AttrName设置属性值
        /// </summary>
        public void AddValue(AttrName attrName, double value)
        {
            SetValue(attrName, GetValue(attrName) + value);
        }
    }
}
