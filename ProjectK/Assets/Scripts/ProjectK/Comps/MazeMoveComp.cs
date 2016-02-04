using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    /// <summary>
    /// 迷宫寻路移动组件
    /// </summary>
    public class MazeMoveComp : GameComp
    {
        private AttrComp attrComp;
        private MapPath path;

        private int nextWaypointIndex = 1;

        public delegate void OnReachEndCallback();
        public event OnReachEndCallback OnReachEnd;
        public bool ReachedEnd = false;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            attrComp = Entity.AttrComp;
            return true;
        }

        public override void Destroy()
        {
            attrComp = null;
            path = null;
            OnReachEnd = null;
            base.Destroy();
        }

        public void SetPath(MapPath path)
        {
            this.path = path;
        }

        public override void Activate()
        {
            if (path == null)
                return;

            if (nextWaypointIndex >= path.WaypointCount)
            {
                TryReachEnd();
                return;
            }

            float move = attrComp.MoveSpeed * Entity.Scene.DeltaTime;
            float move2 = move * move;
            Vector2 currLocation = Entity.Location;
            Vector2 nextLocation = Vector2.zero;
            Vector3 position = Entity.Position;
            Vector3 direction = Vector3.zero;
            while (move2 > 0)
            {
                if (!path.GetNextLocation(currLocation, nextWaypointIndex, out nextLocation))
                {
                    nextWaypointIndex += 1;
                    if (nextWaypointIndex >= path.WaypointCount)
                    {
                        TryReachEnd();
                        return;
                    }
                    break;
                }

                Vector3 nextPosition = MapUtils.LocationToPosition(nextLocation);
                direction = nextPosition - Entity.Position;
                float distance2 = direction.sqrMagnitude;
                if (distance2 >= move2)
                {
                    direction.Normalize();
                    position += direction * move;
                    Entity.Position = position;
                    break;
                }
                else
                {
                    position = nextPosition;
                    currLocation = nextLocation;
                    move2 -= distance2;
                    move -= direction.magnitude;
                    Entity.Position = position;
                }
            }

            Vector3 localScale = Entity.gameObject.transform.localScale;
            if (direction.x > 0 && localScale.x < 0
                || direction.x < 0 && localScale.x > 0)
            {
                localScale.x *= -1;
                Entity.gameObject.transform.localScale = localScale;
            }
        }

        private void TryReachEnd()
        {
            if (ReachedEnd)
                return;

            ReachedEnd = true;
            if (OnReachEnd != null)
                OnReachEnd();
        }
    }
}
