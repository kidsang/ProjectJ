using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class Buff
    {
        public int ID;
        public BuffSetting Setting;
        public BuffMgrComp BuffMgrComp;

        /// <summary>
        /// buff已经生效的时间
        /// </summary>
        public float BuffTime { get; private set; }

        /// <summary>
        /// buff总持续时间，-1为永存
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// buff剩余时间
        /// </summary>
        public float LeftTime { get { return Duration - BuffTime; } }

        /// <summary>
        /// 上次Buff生效时间
        /// </summary>
        private float lastUpdateTime;

        /// <summary>
        /// 重置Buff时间
        /// </summary>
        public void ResetBuffTime(float duration)
        {
            BuffTime = 0;
            Duration = duration;
            lastUpdateTime = 0;
        }

        public void Update()
        {
            float nowTime = BuffMgrComp.Entity.Scene.Time;
            if (BuffTime >= Duration)
            {
                Remove();
                return;
            }

            BuffTime += BuffMgrComp.Entity.Scene.DeltaTime;

            if (Setting.UpdateInterval > 0 && nowTime - lastUpdateTime >= Setting.UpdateInterval)
            {
                lastUpdateTime = nowTime;
                OnUpdate();
            }
        }

        /// <summary>
        /// 把自身从Entity上移除
        /// </summary>
        public void Remove()
        {
            if (BuffMgrComp != null)
                BuffMgrComp.RemoveBuff(this);
        }

        /// <summary>
        /// 在buff添加的时候调用一次
        /// </summary>
        public virtual void OnAdd(bool firstTime)
        {
            if (firstTime)
                ApplyAttrs(Setting.AttrNameArr, Setting.AttrValueArr);
        }

        /// <summary>
        /// 在buff移除的时候调用一次
        /// </summary>
        public virtual void OnRemove()
        {
            UnapplyAttrs(Setting.AttrNameArr, Setting.AttrValueArr);
            Setting = null;
            BuffMgrComp = null;
        }

        /// <summary>
        /// 如果设置了UpdateInterval > 0，则每到指定间隔调用一次
        /// </summary>
        public virtual void OnUpdate()
        {
            ApplyAttrs(Setting.UpdateAttrNameArr, Setting.UpdateAttrValueArr);
        }

        protected void ApplyAttr(AttrName attrName, double attrValue)
        {
            BuffMgrComp.Entity.AttrComp.AddValue(attrName, attrValue);
        }

        protected void UnapplyAttr(AttrName attrName, double attrValue)
        {
            BuffMgrComp.Entity.AttrComp.AddValue(attrName, -attrValue);
        }

        protected void ApplyAttrs(AttrName[] attrNames, double[] attrValues)
        {
            int length = attrNames.Length;
            AttrComp attrComp = BuffMgrComp.Entity.AttrComp;
            for (int i = 0; i < length; ++i)
                attrComp.AddValue(attrNames[i], attrValues[i]);
        }

        protected void UnapplyAttrs(AttrName[] attrNames, double[] attrValues)
        {
            int length = attrNames.Length;
            AttrComp attrComp = BuffMgrComp.Entity.AttrComp;
            for (int i = 0; i < length; ++i)
                attrComp.AddValue(attrNames[i], -attrValues[i]);
        }
    }
}
