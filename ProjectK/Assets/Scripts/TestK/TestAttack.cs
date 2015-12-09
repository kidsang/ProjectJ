using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectK;
using ProjectK.Base;
using Vectrosity;

namespace TestK
{
    public class TestAttack : MonoBehaviour
    {
        private Scene scene;

        private TowerEntity tower;
        private double towerAtk;
        private double towerAtkSpeed;
        private float towerAtkRange;

        void Start()
        {
            ResourceManager.Init();
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

            float mapWidth = scene.Map.Width;
            float mapHeight = scene.Map.Height;
            for (int i = 0; i < 100; ++i)
            {
                MonsterEntity monsterEntity = scene.CreateMonsterEntity(0);
                scene.AddEntityToScene(monsterEntity, new Vector3(Random.value * mapWidth, Random.value * mapHeight, 0));
            }

            tower = scene.CreateTowerEntity(0);
            tower.ShowDebugDraw = true;
            scene.AddEntityToScene(tower, new Vector3(mapWidth / 2, mapHeight / 2, 0));

            towerAtk = tower.AttrComp.AtkBase;
            towerAtkSpeed = tower.AttrComp.AtkSpeedBase;
            towerAtkRange = tower.AttrComp.AtkRangeBase;
        }

        void RestartTest()
        {
            if (scene == null)
                return;

            scene.Dispose();
            scene = null;
            SettingManager.Instance.ReloadAll(StartTest);
        }


        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 200, 300), "攻击测试");
            GUILayout.BeginArea(new Rect(20, 40, 180, 280));
            GUILayout.BeginVertical();

            if (GUILayout.Button("重载数据表和场景"))
            {
                RestartTest();
                return;
            }

            GUILayout.Label("炮塔攻击力：" + tower.AttrComp.AtkBase.ToString("0.00"));
            tower.AttrComp.AtkBase = GUILayout.HorizontalSlider((float)tower.AttrComp.AtkBase, (float)(towerAtk / 2), (float)(towerAtk * 2));

            GUILayout.Label("炮塔攻击速度：" + tower.AttrComp.AtkSpeedBase.ToString("0.00"));
            tower.AttrComp.AtkSpeedBase = GUILayout.HorizontalSlider((float)tower.AttrComp.AtkSpeedBase, (float)(towerAtkSpeed / 2), (float)(towerAtkSpeed * 2));

            GUILayout.Label("炮塔攻击范围：" + tower.AttrComp.AtkRangeBase.ToString("0.00"));
            tower.AttrComp.AtkRangeBase = GUILayout.HorizontalSlider((float)tower.AttrComp.AtkRangeBase, (float)(towerAtkRange / 2), (float)(towerAtkRange * 2));

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

    }
}
