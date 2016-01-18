using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    /// <summary>
    /// 弹射攻击，攻击弹射x次
    /// </summary>
    public class SkillComp2013 : SkillComp
    {
        private const float reboundRange = 3f;
        private const int maxReboundCount = 2;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            RegisterOnBeforeBulletHit(OnBeforeBulletHit);
            return true;
        }

        private void OnBeforeBulletHit(Bullet bullet)
        {
            int reboundCount = (int)bullet.GetUserData("reboundCount", 0);
            if (reboundCount >= maxReboundCount)
                return;

            if (reboundCount == 0 && bullet.GetUserData(ConflicMark1) != null)
                return;

            Scene scene = SceneManager.Instance.Scene;
            List<SceneEntity> targetEntities = scene.CollectEntitiesCircle(MapUtils.PositionToLocation(bullet.transform.position), reboundRange, null, CampType.Enemy, true);
            foreach (SceneEntity targetEntity in targetEntities)
            {
                if (targetEntity.UID == bullet.TargetEntityUID)
                    continue;

                Bullet reboundBullet = scene.FireBullet(Entity.UID, targetEntity.UID);
                reboundBullet.SetUserData(ConflicMark1, true);
                reboundBullet.SetUserData("reboundCount", reboundCount + 1);

                SceneEntity sourceEntity = scene.GetEntity(bullet.TargetEntityUID);
                reboundBullet.transform.position = sourceEntity.Position;

                AtkComp atkComp = Entity.GetComp<AtkComp>();
                if (atkComp != null && atkComp.HasOnAfterFireBulletEvent)
                {
                    List<Bullet> bullets = new List<Bullet> { reboundBullet };
                    atkComp.CallOnAfterFireBulletEvent(bullets);
                }
                break;
            }
        }
    }
}
