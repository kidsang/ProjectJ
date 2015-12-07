using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorK.UI
{
    public class NewMapPopup : PopupPanel
    {
        public InputField CountXLabel;
        public InputField CountYLabel;
        public Button YesButton;
        public Button NoButton;

        public void OnYesButtonClick()
        {
            GameEditor.Instance.NewMap(int.Parse(CountXLabel.text), int.Parse(CountYLabel.text));
            PopupPanels.Instance.HidePopup(this);
        }

        public void OnNoButtonClick()
        {
            PopupPanels.Instance.HidePopup(this);
        }
    }
}
