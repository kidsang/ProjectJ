using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace TestK
{
    public class TestUI : MonoBehaviour
    {
        void Start()
        {
            ResourceManager.Init();
            UIManager.Init();

            BuildTowerUI ui = UIManager.Instance.CreateUI<BuildTowerUI>();
        }
    }
}
