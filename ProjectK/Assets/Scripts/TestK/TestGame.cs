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
            scene.Load("Settings/test_game_s.map");
			scene.Map.ShowDebugDraw(true);

            SpawnWaveSetting spawnWaveSetting = new SpawnWaveSetting();
            spawnWaveSetting.WaveIndex = 0;
            spawnWaveSetting.IntervalTime = 1;
            spawnWaveSetting.SpawnTimes = int.MaxValue;
            spawnWaveSetting.SpawnPerTime = 1;
            spawnWaveSetting.TemplateID = 0;
            SpawnLocationSetting spawnLocationSetting = new SpawnLocationSetting();
            spawnLocationSetting.PathIndex = 0;
            spawnLocationSetting.Waves = new SpawnWaveSetting[] { spawnWaveSetting };
            SpawnSetting spawnSetting = new SpawnSetting();
            spawnSetting.Locations = new SpawnLocationSetting[] { spawnLocationSetting };
            scene.SpawnManager.Load(spawnSetting);

            SceneManager.Init();
            SceneManager.Instance.SwitchTo(scene);
            scene.StartScene();
            scene.SpawnManager.Start();

            Player.Me.SelectedTowers.Add(0);
            Player.Me.SelectedTowers.Add(1);

            UIManager.Instance.CreateUI<SceneEventUI>(UILayer.LayerLow);
        }
    }
}
