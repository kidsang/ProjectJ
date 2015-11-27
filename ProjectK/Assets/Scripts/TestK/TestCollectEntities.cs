using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectK;
using ProjectK.Base;
using Vectrosity;

namespace TestK 
{
    public class TestCollectEntities : MonoBehaviour
    {
        private Vector3 position = new Vector3(0, 0, 0);
        private CollectEntityShape shape;
        private float radius = 3;
        private float width = 5;
        private float height = 2;
        private float rangeAngle = 45 * Mathf.PI / 180;
        private float rotateAngle = 0;
        private float innerRadius = 1;

        private DebugDraw debugDraw;
        private Scene scene;

        void Start()
        {
            GameObject debugDrawGo = new GameObject("DebugDraw");
            debugDraw = debugDrawGo.AddComponent<DebugDraw>();

            ResourceManager.Init();
            SettingManager.Init(StartTest);

        }

        void StartTest()
        {
            GameObject sceneRoot = new GameObject("SceneRoot");
            scene = sceneRoot.AddComponent<Scene>();
            scene.Init();
            //SceneSetting sceneSetting = new SceneSetting(true);
            //sceneSetting.Map.CellCountX = 10;
            //sceneSetting.Map.CellCountY = 10;
            //scene.Load(sceneSetting);
            scene.Load("Settings/test.map");

            float mapWidth = scene.Map.Width;
            float mapHeight = scene.Map.Height;
            position = new Vector3(mapWidth / 2, mapHeight / 2, 0);
            for (int i = 0; i < 500; ++i)
            {
                MonsterEntity monsterEntity = scene.CreateMonsterEntity(0);
                scene.AddEntityToScene(monsterEntity, new Vector3(Random.value * mapWidth, Random.value * mapHeight, 0));
            }
        }

        void Update()
        {
            if (!scene)
                return;

            debugDraw.gameObject.transform.position = position;

            foreach (var entity in scene.EntityList)
                Helpers.ColorTransformSprite(entity);

            List<SceneEntity> entities = new List<SceneEntity>();
            switch (shape)
            {
                case CollectEntityShape.Circle:
                    debugDraw.DrawCircle(radius);
                    scene.CollectEntitiesCircle(position, radius, entities);
                    break;
                case CollectEntityShape.Fan:
                    debugDraw.DrawFan(radius, rangeAngle, rotateAngle);
                    scene.CollectEntitiesFan(position, radius, rangeAngle, rotateAngle, entities);
                    break;
                case CollectEntityShape.Rectangle:
                    debugDraw.DrawRectangle(width, height, rotateAngle);
                    scene.CollectEntitiesRectangle(position, width, height, rotateAngle, entities);
                    break;
                case CollectEntityShape.Ring:
                    debugDraw.DrawRing(radius, innerRadius);
                    scene.CollectEntitiesRing(position, radius, innerRadius, entities);
                    break;
            }

            foreach (var entity in entities)
                Helpers.ColorTransformSprite(entity, 1, 0.5f, 0.5f);
        }

        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 200, 300), "选怪测试");
            GUILayout.BeginArea(new Rect(20, 40, 180, 280));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("位置：");
            GUILayout.Label(position.ToString());
            GUILayout.EndHorizontal();

            GUILayout.Label("类型：");
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(shape == CollectEntityShape.Circle, "Circle"))
                shape = CollectEntityShape.Circle;
            if (GUILayout.Toggle(shape == CollectEntityShape.Fan, "Fan"))
                shape = CollectEntityShape.Fan;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(shape == CollectEntityShape.Rectangle, "Rectangle"))
                shape = CollectEntityShape.Rectangle;
            if (GUILayout.Toggle(shape == CollectEntityShape.Ring, "Ring"))
                shape = CollectEntityShape.Ring;
            GUILayout.EndHorizontal();

            GUILayout.Label("参数：");
            if (shape == CollectEntityShape.Circle)
            {
                GUILayout.Label("半径：" + radius.ToString("0.00"));
                radius = GUILayout.HorizontalSlider(radius, 1, 5);
            }
            else if (shape == CollectEntityShape.Fan)
            {
                GUILayout.Label("半径：" + radius.ToString("0.00"));
                radius = GUILayout.HorizontalSlider(radius, 1, 5);

                GUILayout.Label("范围角度：" + ((int)(rangeAngle * 180 / Mathf.PI)).ToString());
                rangeAngle = GUILayout.HorizontalSlider(rangeAngle, 0, Mathf.PI * 2);

                GUILayout.Label("旋转角度：" + ((int)(rotateAngle * 180 / Mathf.PI)).ToString());
                rotateAngle = GUILayout.HorizontalSlider(rotateAngle, 0, Mathf.PI * 2);
            }
            else if (shape == CollectEntityShape.Rectangle)
            {
                GUILayout.Label("长：" + width.ToString("0.00"));
                width = GUILayout.HorizontalSlider(width, 3, 8);

                GUILayout.Label("宽：" + height.ToString("0.00"));
                height = GUILayout.HorizontalSlider(height, 1, 4);

                GUILayout.Label("旋转角度：" + ((int)(rotateAngle * 180 / Mathf.PI)).ToString());
                rotateAngle = GUILayout.HorizontalSlider(rotateAngle, 0, Mathf.PI * 2);
            }
            else if (shape == CollectEntityShape.Ring)
            {
                GUILayout.Label("外半径：" + radius.ToString("0.00"));
                radius = GUILayout.HorizontalSlider(radius, 1, 5);

                float maxInnerRadius = radius - 0.5f;
                innerRadius = Mathf.Min(innerRadius, maxInnerRadius);
                GUILayout.Label("内半径：" + innerRadius.ToString("0.00"));
                innerRadius = GUILayout.HorizontalSlider(innerRadius, 0.5f, maxInnerRadius);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        class DebugDraw : MonoBehaviour
        {
            private VectorLine line;

            private static readonly int numPoints = 33;

            void Start()
            {
                line = new VectorLine("PathLine", new Vector3[numPoints], null, 2, LineType.Continuous);
                line.SetColor(new Color(1, 0, 0));
                VectorManager.useDraw3D = true;
                VectorManager.ObjectSetup(gameObject, line, Visibility.Dynamic, Brightness.None);
            }

            public void DrawCircle(float radius)
            {
                ResetPoints();
                line.MakeCircle(Vector3.zero, MapUtils.Vector3Z, radius);
            }

            public void DrawFan(float radius, float rangeAngle, float rotateAngle)
            {
                ResetPoints();
                float startAngle = -rangeAngle / 2;
                float endAngle = rangeAngle / 2;
                float deltaAngle = rangeAngle / (numPoints - 2);
                Vector3 point = new Vector3(radius, 0, 0);
                for (int i = 1; i < numPoints - 1; ++i)
                {
                    float angle = startAngle + deltaAngle * (i - 1);
                    line.points3[i] = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, MapUtils.Vector3Z) * point;
                }
                transform.rotation = Quaternion.AngleAxis(rotateAngle * Mathf.Rad2Deg, MapUtils.Vector3Z);
            }

            public void DrawRectangle(float width, float height, float rotateAngle)
            {
                ResetPoints();
                line.MakeRect(new Rect(0, -height / 2, width, height));
                transform.rotation = Quaternion.AngleAxis(rotateAngle * Mathf.Rad2Deg, MapUtils.Vector3Z);
            }

            public void DrawRing(float radius, float innerRadius)
            {
                ResetPoints();
                int innerSegments = (numPoints - 1) / 2;
                line.MakeCircle(Vector3.zero, MapUtils.Vector3Z, innerRadius, innerSegments);
                line.MakeCircle(Vector3.zero, MapUtils.Vector3Z, radius, numPoints - innerSegments - 1, innerSegments);
            }

            private void ResetPoints()
            {
                for (int i = 0; i < numPoints; ++i)
                    line.points3[i] = Vector3.zero;
            }
        }
    }
}
