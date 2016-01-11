using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectK
{
    public class HpBarUI : UIBase
    {
        private HpBarUIDetail detail;

        protected override void Init()
        {
            detail = GameObject.GetComponent<HpBarUIDetail>();
        }

        protected override void OnRefresh(params object[] args)
        {
            float hpPercent = (float)args[0];
            detail._Hp.transform.localScale = new Vector3(hpPercent, 1, 1);
        }
    }
}
