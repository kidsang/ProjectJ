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
        private float radius = 1;
        private float width = 1;
        private float height = 1;
        private float rangeAngle = 30 * Mathf.PI / 180;
        private float rotateAngle = 0;

        private DebugDraw debugDraw;
        private Scene scene;

        void Start()
        {
            GameObject debugDrawGo = new GameObject("DebugDraw");
            debugDraw = debugDrawGo.AddComponent<DebugDraw>();

            ResourceManager.Init();

            GameObject sceneRoot = new GameObject("SceneRoot");
            scene = sceneRoot.AddComponent<Scene>();
            scene.Init();
            //SceneSetting sceneSetting = new SceneSetting(true);
            //sceneSetting.Map.CellCountX = 10;
            //sceneSetting.Map.CellCountY = 10;
            //scene.Load(sceneSetting);
            scene.Load("Settings/test.map");
        }

        void Update()
        {
            debugDraw.gameObject.transform.position = position;
            switch (shape)
            {
                case CollectEntityShape.Circle:
                    debugDraw.DrawCircle(radius);
                    break;
                case CollectEntityShape.Fan:
                    debugDraw.DrawFan(radius, rangeAngle, rotateAngle);
                    break;
                case CollectEntityShape.Rectangle:
                    debugDraw.DrawRectangle(width, height, rotateAngle);
                    break;
            }
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
            if (GUILayout.Toggle(shape == CollectEntityShape.Rectangle, "Rectangle"))
                shape = CollectEntityShape.Rectangle;
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
                width = GUILayout.HorizontalSlider(width, 1, 5);

                GUILayout.Label("宽：" + height.ToString("0.00"));
                height = GUILayout.HorizontalSlider(height, 0, Mathf.PI * 2);

                GUILayout.Label("旋转角度：" + ((int)(rotateAngle * 180 / Mathf.PI)).ToString());
                rotateAngle = GUILayout.HorizontalSlider(rotateAngle, 0, Mathf.PI * 2);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        class DebugDraw : MonoBehaviour
        {
            private VectorLine line;
            private CollectEntityShape shape;

            private static readonly int numPoints = 33;

            void Start()
            {
                line = new VectorLine("PathLine", new Vector3[numPoints], null, 2, LineType.Continuous);
                VectorManager.useDraw3D = true;
                VectorManager.ObjectSetup(gameObject, line, Visibility.Dynamic, Brightness.None);
            }

            public void DrawCircle(float radius)
            {
                ResetPoints();
                line.MakeCircle(Vector3.zero, new Vector3(0, 0, 1), radius);
            }

            public void DrawFan(float radius, float rangeAngle, float rotateAngle)
            {
                ResetPoints();
                float startAngle = rotateAngle - rangeAngle / 2;
                float endAngle = startAngle + rangeAngle;
                float deltaAngle = rangeAngle / (numPoints - 2);
                Vector3 point = new Vector3(radius, 0, 0);
                for (int i = 1; i < numPoints - 1; ++i)
                {
                    float angle = startAngle + deltaAngle * (i - 1);
                    Quaternion rotation = Quaternion.EulerAngles(0, 0, angle);
                    line.points3[i] = rotation * point;
                }
            }

            public void DrawRectangle(float width, float height, float rotateAngle)
            {
                ResetPoints();
                line.MakeRect(new Rect(0, -height / 2, width, height));
                transform.rotation = Quaternion.EulerAngles(0, 0, rotateAngle);
            }

            private void ResetPoints()
            {
                for (int i = 0; i < numPoints; ++i)
                    line.points3[i] = Vector3.zero;
            }
        }
    }
}
