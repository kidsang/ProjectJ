using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ProjectK.Base;

namespace ProjectK
{
    public class Test : DisposableBehaviour
    {
        void Start()
        {
            //Game.Init();
            //Log.Info("Test!");

            //Time.timeScale = 0;
        }

        //private float lastTime;
        void Update()
        {
            //Log.Info("Time:", Time.time);
            //if (Time.time - lastTime > 1)
            //{
            //    Log.Info("Time:", Time.time);
            //    lastTime = Time.time;
            //}

            Vector3 position = Input.mousePosition;

            Vector2 v1 = Vector2.right;
            Vector2 v2 = Camera.main.ScreenToViewportPoint(position) - new Vector3(0.5f, 0.5f, 0);

            float angle = MapUtils.Angle(v2);
            float degree = angle * 180 / Mathf.PI;
            Log.Info(angle, degree);
        }

        //private float lastFixedTime;
        void FixedUpdate()
        {
            //Log.Info("FixedTime:", Time.fixedTime);
            //if (Time.fixedTime - lastFixedTime > 1)
            //{
            //    Log.Info("FixedTime:", Time.fixedTime);
            //    lastFixedTime = Time.fixedTime;
            //}
        }

    }
}

