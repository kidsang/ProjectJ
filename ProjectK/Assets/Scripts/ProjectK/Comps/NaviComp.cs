using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class NaviComp : GameComp
    {
        private Vector2 location;
        private Vector3 position;

        public override bool Start()
        {
            if (!base.Start())
                return false;

            position = Entity.transform.position;
            location = MapUtils.PositionToLocation(position);
            return true;
        }

        public Vector2 Location
        {
            get { return location; }
            set
            {
                if (value == location)
                    return;
                location = value;
                position = MapUtils.LocationToPosition(location);
                UpateEntityPosition();
            }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                if (value == position)
                    return;
                position = value;
                location = MapUtils.PositionToLocation(position);
                UpateEntityPosition();
            }
        }

        private void UpateEntityPosition()
        {
            Entity.transform.position = position;
            if (Entity.Scene != null)
                Entity.Scene.Map.UpdateSceneEntityCell(Entity);
        }
    }
}
