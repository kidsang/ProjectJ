using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class SkillComp : GameComp
    {
        public override bool Start()
        {
            if (!base.Start())
                return false;

            AtkComp atkComp = Entity.GetComp<AtkComp>();
            if (atkComp != null)
            {
                atkComp.OnBeforeFireBullet += OnBeforeFireBullet;
                atkComp.OnAfterFireBullet += OnAfterFireBullet;
            }

            return true;
        }

        public override void Destroy()
        {
            AtkComp atkComp = Entity.GetComp<AtkComp>();
            if (atkComp != null)
            {
                atkComp.OnBeforeFireBullet -= OnBeforeFireBullet;
                atkComp.OnAfterFireBullet -= OnAfterFireBullet;
            }

            base.Destroy();
        }

        virtual protected void OnBeforeFireBullet(AtkComp atkComp, List<Bullet> bullets)
        {

        }

        virtual protected void OnAfterFireBullet(AtkComp atkComp, List<Bullet> bullets)
        {
            foreach (Bullet bullet in bullets)
            {
                bullet.OnBulletActivate += OnBulletActivate;
                bullet.OnBeforeBulletHit += OnBeforeBulletHit;
                bullet.OnAfterBulletHit += OnAfterBulletHit;
            }
        }

        virtual protected void OnBulletActivate(Bullet bullet)
        {

        }

        virtual protected void OnBeforeBulletHit(Bullet bullet)
        {

        }

        virtual protected void OnAfterBulletHit(Bullet bullet)
        {

        }
    }
}
