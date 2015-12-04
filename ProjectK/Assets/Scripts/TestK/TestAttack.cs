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

            float mapWidth = scene.Map.Width;
            float mapHeight = scene.Map.Height;
            for (int i = 0; i < 500; ++i)
            {
                MonsterEntity monsterEntity = scene.CreateMonsterEntity(0);
                scene.AddEntityToScene(monsterEntity, new Vector3(Random.value * mapWidth, Random.value * mapHeight, 0));
            }
        }

    }
}
