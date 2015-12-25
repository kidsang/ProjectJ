using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vectrosity;
using ProjectK.Base;

namespace ProjectK
{
    class DebugDraw : MonoBehaviour
    {
        private VectorLine line;
        private int numPoints;

        public static DebugDraw Create(string name, GameObject parent, Color color, int lineWidth = 2, LineType lineType = LineType.Continuous, int numPoints = 33)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent.transform, false);

            DebugDraw debugDraw = gameObject.AddComponent<DebugDraw>();
            debugDraw.Init(color, lineWidth, lineType, numPoints);
            return debugDraw;
        }

        public void Init(Color color, int lineWidth, LineType lineType, int numPoints)
        {
            this.numPoints = numPoints;

            line = new VectorLine("PathLine", new Vector3[numPoints], null, lineWidth, lineType);
            line.SetColor(color);
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

        public void DrawCenterRect(float width, float height, float rotateAngle)
        {
            ResetPoints();
            line.MakeRect(new Rect(-width / 2, -height / 2, width, height));
            transform.rotation = Quaternion.AngleAxis(rotateAngle * Mathf.Rad2Deg, MapUtils.Vector3Z);
        }

        private void ResetPoints()
        {
            for (int i = 0; i < numPoints; ++i)
                line.points3[i] = Vector3.zero;
        }
    }
}
