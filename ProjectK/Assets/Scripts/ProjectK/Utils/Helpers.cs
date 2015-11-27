using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    /// <summary>
    /// 用于显示相关的各种奇妙接口
    /// </summary>
    public static class Helpers
    {
        public static void ColorTransformSprite(GameObject go, float r = 1, float g = 1, float b = 1, float a = 1)
        {
            (go.GetComponent<Renderer>() as SpriteRenderer).color = new Color(r, g, b, a);
        }

        public static void ColorTransformSprite(SceneEntity entity, float r = 1, float g = 1, float b = 1, float a = 1)
        {
            ColorTransformSprite(entity.gameObject, r, g, b, a);
        }
    }
}
