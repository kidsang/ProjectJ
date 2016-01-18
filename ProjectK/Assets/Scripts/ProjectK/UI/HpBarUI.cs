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
        private Vector2 size;

        protected override void Init()
        {
            detail = GameObject.GetComponent<HpBarUIDetail>();
            size = detail._Background.rectTransform.sizeDelta;
            GameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        protected override void OnRefresh(params object[] args)
        {
            float hpPercent = (float)args[0];
            detail._Hp.rectTransform.sizeDelta = new Vector2(size.x * hpPercent, size.y);
            detail._Hp.rectTransform.anchoredPosition = new Vector2(size.x * (hpPercent - 1) / 2, 0);
        }
    }
}
