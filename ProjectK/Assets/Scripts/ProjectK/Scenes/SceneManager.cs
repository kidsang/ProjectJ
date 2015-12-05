using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class SceneManager
    {
        private static SceneManager instance;

        public Scene Scene { get; private set; }

        public static void Init()
        {
            if (instance != null)
                return;

            instance = new SceneManager();
        }

        public static SceneManager Instance
        {
            get { return instance; }
        }

        public void SwitchTo()
        {
            if (Scene != null)
                Scene.Dispose();

            GameObject sceneRoot = new GameObject("SceneRoot");
            Scene = sceneRoot.AddComponent<Scene>();
            Scene.Init();

            // TODO:
            Scene.Load("Settings/test.map");
            Scene.StartScene();

            Camera camera = Camera.main;
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * camera.aspect;
            float left = cameraHalfWidth;
            float right = Scene.Map.Width - cameraHalfWidth;
            float bottom = cameraHalfHeight;
            float top = Scene.Map.Height - cameraHalfHeight;

            Vector3 cameraPosition = camera.transform.position;
            if (left > right)
                cameraPosition.x = (left + right) / 2;
            else if (cameraPosition.x < left)
                cameraPosition.x = left;
            else if (cameraPosition.x > right)
                cameraPosition.x = right;
            if (bottom > top)
                cameraPosition.y = (bottom + top) / 2;
            else if (cameraPosition.y < bottom)
                cameraPosition.y = bottom;
            else if (cameraPosition.y > top)
                cameraPosition.y = top;
            camera.transform.position = cameraPosition;
        }

        public void SwitchTo(Scene scene)
        {
            this.Scene = scene;
        }
    }
}
