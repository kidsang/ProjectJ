using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectK
{
    /// <summary>
    /// 用于显示相关的各种奇妙接口
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// 修改元件的颜色，目标GameObject必须绑定有SpriteRenderer组件
        /// </summary>
        public static void ColorTransformSprite(GameObject go, float r = 1, float g = 1, float b = 1, float a = 1)
        {
            (go.GetComponent<Renderer>() as SpriteRenderer).color = new Color(r, g, b, a);
        }

        /// <summary>
        /// 修改元件的颜色，目标GameObject必须绑定有SpriteRenderer组件
        /// </summary>
        public static void ColorTransformSprite(SceneEntity entity, float r = 1, float g = 1, float b = 1, float a = 1)
        {
            ColorTransformSprite(entity.gameObject, r, g, b, a);
        }

        /// <summary>
        /// 在目标头顶显示血条
        /// </summary>
        public static void ShowHpBar(GameObject gameObject, float hpPercent)
        {
            HudManager hudManager = HudManager.GetHudManager(gameObject);
            UIBase hud = hudManager.GetHud("hp_bar");
            if (hud == null)
            {
                hud = UIManager.Instance.CreateHud<HpBarUI>();
                hudManager.AddHud("hp_bar", hud, Vector2.zero);
            }
            hud.Show();
            hud.Refresh(hpPercent);
        }

        /// <summary>
        /// 隐藏目标头顶血条
        /// </summary>
        public static void HideHpBar(GameObject gameObject)
        {
            HudManager hudManager = HudManager.GetHudManager(gameObject);
            hudManager.HideHud("hp_bar");
        }

        /// <summary>
        /// 显示冒血数字
        /// </summary>
        public static void ShowHpText(GameObject gameObject, int hpChange)
        {
            HudManager hudManager = HudManager.GetHudManager(gameObject);
            UIBase hud = hudManager.GetHud("hp_text");
            if (hud == null)
            {
                hud = UIManager.Instance.CreateHud<HpTextUI>();
                hudManager.AddHud("hp_text", hud, new Vector2());
            }
            hud.Show();
            hud.Refresh(hpChange.ToString());
        }

        /// <summary>
        /// 根据权重随机取值
        /// </summary>
        public static int RandomWeights(double[] weights)
        {
            double totalWeight = weights.Sum();
            double randomValue = UnityEngine.Random.Range(0, (float)totalWeight);
            for (int i = 0; i < weights.Length; ++i)
            {
                double weight = weights[i];
                if (randomValue < weight)
                    return i;
                randomValue -= weight;
            }
            return weights.Length - 1;
        }

        /// <summary>
        /// 根据权重随机取值
        /// </summary>
        public static int RandomWeights(int[] weights)
        {
            int totalWeight = weights.Sum();
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            for (int i = 0; i < weights.Length; ++i)
            {
                int weight = weights[i];
                if (randomValue < weight)
                    return i;
                randomValue -= weight;
            }
            return weights.Length - 1;
        }
    }
}
