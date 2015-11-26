using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    /// <summary>
    /// 通用属性组件
    /// </summary>
    public class AttrComp : GameComp
    {
        #region 生命值 hp maxHp maxHpBase maxHpAddRate
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
                OnPropChange("Hp", value);
            }
        }

        /// <summary>
        /// 最大血量
        /// 如果最大血量小于当前血量，当前血量会调整为最大血量
        /// </summary>
        public double MaxHp
        {
            get { return maxHp; }
            set
            {
                if (value == maxHp)
                    return;
                maxHp = value;

                if (maxHp < hp)
                    Hp = maxHp;

                OnPropChange("MaxHp", value);
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
            set
            {
                if (value == atk)
                    return;
                atk = value;
                OnPropChange("Atk", value);
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
            set
            {
                if (value == def)
                    return;
                def = value;
                OnPropChange("Def", value);
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
            set
            {
                if (value < 0)
                    value = 0;
                if (value == atkSpeed)
                    return;
                atkSpeed = value;
                OnPropChange("AtkSpeed", value);
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
            set
            {
                if (value < 0)
                    value = 0;
                if (value == atkRange)
                    return;
                atkRange = value;
                OnPropChange("AtkRange", value);
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

        public delegate void PropChangeCallback(double newValue);
        private Dictionary<string, List<PropChangeCallback>> propChangeCallbackDict = new Dictionary<string, List<PropChangeCallback>>();

        /// <summary>
        /// 注册属性变化时的回调函数
        /// </summary>
        public void RegisterPropChangeCallback(string propName, PropChangeCallback callback)
        {
            List<PropChangeCallback> callbackList;
            if (propChangeCallbackDict.TryGetValue(propName, out callbackList))
            {
                if (callbackList.Contains(callback))
                    return;
            }
            else
            {
                callbackList = new List<PropChangeCallback>();
                propChangeCallbackDict[propName] = callbackList;
            }
            callbackList.Add(callback);
        }

        /// <summary>
        /// 反注册属性变化时的回调函数
        /// </summary>
        public void UnrigisterPropChangeCallback(string propName, PropChangeCallback callback)
        {
            List<PropChangeCallback> callbackList;
            if (propChangeCallbackDict.TryGetValue(propName, out callbackList))
                callbackList.Remove(callback);
        }

        /// <summary>
        /// 通知属性变化
        /// </summary>
        private void OnPropChange(string propName, double newValue)
        {
            List<PropChangeCallback> callbackList;
            if (propChangeCallbackDict.TryGetValue(propName, out callbackList))
            {
                callbackList = new List<PropChangeCallback>(callbackList);
                int count = callbackList.Count;
                for (int i = 0; i < count; ++i)
                {
                    PropChangeCallback callback = callbackList[i];
                    callback(newValue);
                }
            }
        }

    }
}
