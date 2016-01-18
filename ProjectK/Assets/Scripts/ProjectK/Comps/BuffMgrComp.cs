using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class BuffMgrComp : GameComp
    {
        private Dictionary<int, Buff> buffDict = new Dictionary<int, Buff>();
        private List<Buff> buffList = new List<Buff>();

        public Buff AddBuff(uint fromEntityUID, int buffID, float buffDuration)
        {
            BuffSetting buffSetting = SettingManager.Instance.BuffSettings.GetValue(buffID);
            if (buffSetting == null)
            {
                Log.Error("找不到BuffSetting！ buffID：", buffID);
                return null;
            }

            Buff buff = GetBuff(buffID);
            if (buff != null)
            {
                if (buff.LeftTime < buffDuration)
                {
                    buff.ResetBuffTime(buffDuration);
                    buff.OnAdd(false);
                }
                return buff;
            }

            if (buffSetting.Special)
            {
                Type buffClass = Assembly.GetExecutingAssembly().GetType("ProjectK.Buff" + buffID.ToString());
                if (buffClass == null)
                {
                    Log.Error("找不到特殊Buff类！ buffID：", buffID);
                    return null;
                }
                buff = Activator.CreateInstance(buffClass) as Buff;
            }
            else
            {
                buff = new Buff();
            }

            buffDict[buffID] = buff;
            buffList.Add(buff);

            buff.ID = buffSetting.ID;
            buff.Setting = buffSetting;
            buff.BuffMgrComp = this;
            buff.ResetBuffTime(buffDuration);
            buff.OnAdd(true);
            return buff;
        }

        public void RemoveBuff(Buff buff)
        {
            buffDict.Remove(buff.ID);
            buffList.Remove(buff);
            buff.OnRemove();
        }

        public void RemoveBuff(int buffID)
        {
            Buff buff = GetBuff(buffID);
            if (buff != null)
                RemoveBuff(buff);
        }

        public Buff GetBuff(int buffID)
        {
            Buff buff;
            buffDict.TryGetValue(buffID, out buff);
            return buff;
        }

        override public void Activate()
        {
            for (int i = buffList.Count - 1; i >= 0; --i)
                buffList[i].Update();
        }
    }
}
