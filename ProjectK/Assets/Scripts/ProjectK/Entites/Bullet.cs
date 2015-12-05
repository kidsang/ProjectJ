using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;
using UnityEngine;

namespace ProjectK
{
    public class Bullet : DisposableBehaviour
    {
        public uint UID;
        public uint FromEntityUID;
        public uint TargetEntityUID;

        public float speed = 1;
        public float explodeRange = 0.2f;


        public void Init()
        {

        }

        public void Activate()
        {
            Scene scene = SceneManager.Instance.Scene;
            SceneEntity fromEntity = scene.GetEntity(FromEntityUID);
            SceneEntity targetEntity = scene.GetEntity(TargetEntityUID);
            if (fromEntity == null || targetEntity == null)
            {
                Destroy();
                return;
            }

            Vector3 position = transform.position;
            Vector3 targetPosition = targetEntity.Position;
            Vector3 diretion = targetPosition - position;
            float deltaMove = speed * Time.fixedDeltaTime;
            float deltaMove2 = deltaMove * deltaMove;
            float distance2 = diretion.sqrMagnitude;
            if (deltaMove2 > distance2)
            {
                // TODO: 计算伤害并爆炸
                Destroy();
                return;
            }

            diretion.Normalize();
            diretion *= deltaMove;
            position += diretion;
            transform.position = position;
        }

        public void Destroy()
        {
            Scene scene = SceneManager.Instance.Scene;
            scene.DestroyBullet(this);
        }
    }
}