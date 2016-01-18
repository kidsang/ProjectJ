using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    /// <summary>
    /// 多重攻击，同时攻击范围内的x个目标
    /// </summary>
    public class SkillComp2012 : SkillComp
    {
        private const int maxAttackCount = 2;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            RegisterOnBeforeFireBullet(OnBeforeFireBullet);
            return true;
        }

        public override void Destroy()
        {
            UnregisterOnBeforeFireBullet(OnBeforeFireBullet);
            base.Destroy();
        }

        private void OnBeforeFireBullet(AtkComp atkComp, List<Bullet> bullets)
        {
            if (bullets.Count == 0)
                return;

            Bullet originBullet = bullets[0];
            if (originBullet.GetUserData(ConflicMark1) != null)
                return;

            Scene scene = SceneManager.Instance.Scene;
            int targetCount = 1;
            foreach (SceneEntity targetEntity in atkComp.TargetEntities)
            {
                if (targetCount >= maxAttackCount)
                    break;

                if (targetEntity.UID == originBullet.TargetEntityUID)
                    continue;

                Bullet bullet = scene.FireBullet(originBullet.FromEntityUID, targetEntity.UID);
                bullet.SetUserData(ConflicMark1, true);
                bullets.Add(bullet);
                targetCount += 1;
            }
        }
    }
}
