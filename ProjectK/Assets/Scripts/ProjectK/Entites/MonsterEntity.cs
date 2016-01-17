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
        public BuffMgrComp BuffMgrComp { get; private set; }

        private MapPath path;
        private int nextWaypointIndex = 1;
        private int nextPositionIndex;
        private List<Vector3> wayPositions;

        public override void Init(ResourceLoader loader, EntitySetting template)
        {
            base.Init(loader, template);

            MonsterEntitySetting setting = template as MonsterEntitySetting;
            AttrComp.MoveSpeedBase = setting.MoveSpeed;
            AttrComp.MaxHpBase = setting.MaxHp;
            AttrComp.Hp = AttrComp.MaxHp;
            AttrComp.RegisterAttrChangeCallback(AttrName.Hp, OnHpChange);

            BuffMgrComp = AddComp<BuffMgrComp>();
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
            this.path = path;
        }

        public void InvalidWayPositions()
        {
            wayPositions = null;
        }

        public override void Activate()
        {
            base.Activate();

            if (nextWaypointIndex < path.WaypointCount)
            {
                if (wayPositions == null)
                    path.FindPathPosition(Location, nextWaypointIndex, out wayPositions, out nextPositionIndex);

                Vector3 position = NaviComp.Position;
                if (nextPositionIndex < wayPositions.Count)
                {
                    Vector3 wayPosition = wayPositions[nextPositionIndex];
                    Vector3 direction = wayPosition - position;
                    float move = AttrComp.MoveSpeed * Scene.DeltaTime;
                    if (direction.sqrMagnitude > move * move)
                    {
                        direction.Normalize();
                        position += direction * move;
                    }
                    else
                    {
                        position = wayPosition;
                        nextPositionIndex += 1;
                    }

                    NaviComp.Position = position;
                    float angle = MapUtils.Angle(direction);
                    gameObject.transform.localRotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg - 90, MapUtils.Vector3Z);
                }
                else
                {
                    nextWaypointIndex += 1;
                    wayPositions = null;
                }
            }
            else
            {
                Die();
            }
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
    }
}
