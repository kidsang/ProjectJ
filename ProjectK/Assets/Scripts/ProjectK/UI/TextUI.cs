using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectK
{
    public class TextUI : UIBase
    {
        private TextUIDetail detail;

        protected override void Init()
        {
            detail = GameObject.GetComponent<TextUIDetail>();
        }

        protected override void OnRefresh(params object[] args)
        {
            string textStr = (string)args[0];
            detail._Text.text = textStr;
        }

    }
}
