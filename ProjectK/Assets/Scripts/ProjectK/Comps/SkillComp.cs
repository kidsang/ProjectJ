using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class SkillComp : GameComp
    {
        /// <summary>
        /// 用于标识多目标攻击类型的技能不可相互作用
        /// </summary>
        public const string ConflicMark1 = "_cm1";

        private bool hasAfterFireBullet = false;
        private List<Bullet.BeforeBulletHitCallback> beforeBulletHitCallbacks;
        private List<Bullet.AfterBulletHitCallback> afterBulletHitCallbacks;
        private List<Bullet.BulletActivateCallback> bulletActivateCallbacks;

        public override void Destroy()
        {
            if (hasAfterFireBullet)
            {
                UnregisterOnAfterFireBullet(OnAfterFireBullet);
                hasAfterFireBullet = false;
            }

            beforeBulletHitCallbacks = null;
            afterBulletHitCallbacks = null;
            bulletActivateCallbacks = null;

            base.Destroy();
        }

        public void RegisterOnBeforeFireBullet(AtkComp.BeforeFireBulletCallback callback)
        {
            AtkComp atkComp = Entity.GetComp<AtkComp>();
            if (atkComp != null)
                atkComp.OnBeforeFireBullet += callback;
            else
                Log.Error("RegisterOnBeforeFireBullet failed! SkillComp:", this);
        }

        public void UnregisterOnBeforeFireBullet(AtkComp.BeforeFireBulletCallback callback)
        {
            AtkComp atkComp = Entity.GetComp<AtkComp>();
            if (atkComp != null)
                atkComp.OnBeforeFireBullet -= callback;
        }

        public void RegisterOnAfterFireBullet(AtkComp.AfterFireBulletCallback callback)
        {
            AtkComp atkComp = Entity.GetComp<AtkComp>();
            if (atkComp != null)
                atkComp.OnAfterFireBullet += callback;
            else
                Log.Error("RegisterOnAfterFireBullet failed! SkillComp:", this);
        }

        public void UnregisterOnAfterFireBullet(AtkComp.AfterFireBulletCallback callback)
        {
            AtkComp atkComp = Entity.GetComp<AtkComp>();
            if (atkComp != null)
                atkComp.OnAfterFireBullet -= callback;
        }

        public void RegisterOnBeforeBulletHit(Bullet.BeforeBulletHitCallback callback)
        {
            if (!hasAfterFireBullet)
            {
                hasAfterFireBullet = true;
                RegisterOnAfterFireBullet(OnAfterFireBullet);
            }

            if (beforeBulletHitCallbacks == null)
                beforeBulletHitCallbacks = new List<Bullet.BeforeBulletHitCallback>();

            if (!beforeBulletHitCallbacks.Contains(callback))
                beforeBulletHitCallbacks.Add(callback);
        }

        public void RegisterOnAfterBulletHit(Bullet.AfterBulletHitCallback callback)
        {
            if (!hasAfterFireBullet)
            {
                hasAfterFireBullet = true;
                RegisterOnAfterFireBullet(OnAfterFireBullet);
            }

            if (afterBulletHitCallbacks == null)
                afterBulletHitCallbacks = new List<Bullet.AfterBulletHitCallback>();

            if (!afterBulletHitCallbacks.Contains(callback))
                afterBulletHitCallbacks.Add(callback);
        }

        public void RegisterOnBulletActivate(Bullet.BulletActivateCallback callback)
        {
            if (!hasAfterFireBullet)
            {
                hasAfterFireBullet = true;
                RegisterOnAfterFireBullet(OnAfterFireBullet);
            }

            if (bulletActivateCallbacks == null)
                bulletActivateCallbacks = new List<Bullet.BulletActivateCallback>();

            if (!bulletActivateCallbacks.Contains(callback))
                bulletActivateCallbacks.Add(callback);
        }

        private void OnAfterFireBullet(AtkComp atkComp, List<Bullet> bullets)
        {
            foreach (Bullet bullet in bullets)
            {
                if (beforeBulletHitCallbacks != null)
                {
                    foreach (var callback in beforeBulletHitCallbacks)
                        bullet.OnBeforeBulletHit += callback;
                }

                if (afterBulletHitCallbacks != null)
                {
                    foreach (var callback in afterBulletHitCallbacks)
                        bullet.OnAfterBulletHit += callback;
                }

                if (bulletActivateCallbacks != null)
                {
                    foreach (var callback in bulletActivateCallbacks)
                        bullet.OnBulletActivate += callback;
                }
            }
        }

    }
}
