using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class MonsterEntity : SceneEntity
    {
        public AnimComp AnimComp { get; private set; }
        public BuffMgrComp BuffMgrComp { get; private set; }
        public MazeMoveComp MazeMoveComp { get; private set; }

        public override void Init(ResourceLoader loader, EntitySetting template)
        {
            base.Init(loader, template);

            MonsterEntitySetting setting = template as MonsterEntitySetting;
            AttrComp.MoveSpeedBase = setting.MoveSpeed;
            AttrComp.MaxHpBase = setting.MaxHp;
            AttrComp.Hp = AttrComp.MaxHp;
            AttrComp.RegisterAttrChangeCallback(AttrName.Hp, OnHpChange);

            AnimComp = AddComp<AnimComp>();
            BuffMgrComp = AddComp<BuffMgrComp>();
            MazeMoveComp = AddComp<MazeMoveComp>();
            MazeMoveComp.OnReachEnd += OnReachEnd;
        }

        override protected void OnDispose()
        {
            if (AttrComp != null)
            {
                AttrComp.UnrigisterAttrChangeCallback(AttrName.Hp, OnHpChange);
            }

            base.OnDispose();
        }

        public void SetPath(MapPath path)
        {
            MazeMoveComp.SetPath(path);
        }

        public void Die()
        {
            Scene scene = SceneManager.Instance.Scene;
            scene.DestroyEntity(this);
        }

        private void OnHpChange(double value)
        {
            if (value <= 0)
                Die();
        }

        private void OnReachEnd()
        {
            Die();
        }
    }
}
