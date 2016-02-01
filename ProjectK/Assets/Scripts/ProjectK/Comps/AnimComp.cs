using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    /// <summary>
    /// 动画组件
    /// </summary>
    public class AnimComp : GameComp
    {
        public static readonly int RunHash = Animator.StringToHash("run");
        public static readonly int IdleHash = Animator.StringToHash("idle");
        public static readonly int AttackHash = Animator.StringToHash("attack");
        public static readonly int DieHash = Animator.StringToHash("die");

        private AttrComp attrComp;
        private Animator animator;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            attrComp = Entity.AttrComp;
            animator = Entity.GetComponentInChildren<Animator>();
            if (animator)
            {
                attrComp.RegisterAttrChangeCallback(AttrName.MoveSpeed, OnMoveSpeedChange);
                OnMoveSpeedChange(attrComp.MoveSpeed);
            }

            return true;
        }

        public override void Destroy()
        {
            if (animator)
                attrComp.UnrigisterAttrChangeCallback(AttrName.MoveSpeed, OnMoveSpeedChange);
            attrComp = null;
            animator = null;
            base.Destroy();
        }

        public float MoveAnimSpeed
        {
            get { return attrComp.MoveSpeed / attrComp.MoveSpeedBase; }
        }

        private void OnMoveSpeedChange(double value)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == RunHash)
                animator.speed = MoveAnimSpeed;
        }
    }
}
