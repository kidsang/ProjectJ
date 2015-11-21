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
        private double hp;
        private double maxHp;
        private double maxHpBase;
        private double maxHpAddRate;

        private double atk;
        private double atkBase;
        private double atkAddRate;

        private double def;
        private double defBase;
        private double defAddRate;

        private List<DamageType> atkTypes = new List<DamageType>();
        private DamageType defType;

        public delegate void PropChangeCallback(double newValue);
        private Dictionary<string, List<PropChangeCallback>> propChangeCallbackDict = new Dictionary<string, List<PropChangeCallback>>();

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
