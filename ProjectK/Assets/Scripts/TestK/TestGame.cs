using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectK;
using ProjectK.Base;

namespace TestK
{
    public class TestGame : MonoBehaviour
    {
        private Scene scene;

        void Start()
        {
            ResourceManager.Init();
            UIManager.Init();
            SettingManager.Init(StartTest);
        }

        void StartTest()
        {
            GameObject sceneRoot = new GameObject("SceneRoot");
            scene = sceneRoot.AddComponent<Scene>();
            scene.Init();
            scene.Load("Settings/test.map");
            scene.StartScene();

            SceneManager.Init();
            SceneManager.Instance.SwitchTo(scene);

            UIManager.Instance.CreateUI<SceneEventUI>(UILayer.LayerLow);
        }
    }
}
