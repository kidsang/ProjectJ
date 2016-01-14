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

        // 自定义数据
        private Dictionary<string, object> userData;

        // TODO:
        public float speed = 8;

        override protected void OnDispose()
        {
            userData = null;
            base.OnDispose();
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

            if (OnBulletActivate != null)
                OnBulletActivate(this);

            Vector3 position = transform.position;
            Vector3 targetPosition = targetEntity.Position;
            Vector3 diretion = targetPosition - position;
            float deltaMove = speed * Time.fixedDeltaTime;
            float deltaMove2 = deltaMove * deltaMove;
            float distance2 = diretion.sqrMagnitude;
            if (deltaMove2 > distance2)
            {
                if (OnBeforeBulletHit != null)
                    OnBeforeBulletHit(this);

                Formula.AttackCalc(fromEntity, targetEntity);

                if (OnAfterBulletHit != null)
                    OnAfterBulletHit(this);

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

        public void SetUserData(string name, object value)
        {
            if (userData == null)
                userData = new Dictionary<string, object>();
            userData[name] = value;
        }

        public object GetUserData(string name, object defaultValue = null)
        {
            object value;
            if (userData != null && userData.TryGetValue(name, out value))
                return value;
            return defaultValue;
        }

        #region 战斗计算相关回调

        public delegate void BulletActivateCallback(Bullet bullet);
        public event BulletActivateCallback OnBulletActivate;

        public delegate void BeforeBulletHitCallback(Bullet bullet);
        public event BeforeBulletHitCallback OnBeforeBulletHit;

        public delegate void AfterBulletHitCallback(Bullet bullet);
        public event AfterBulletHitCallback OnAfterBulletHit;

        #endregion
    }
}