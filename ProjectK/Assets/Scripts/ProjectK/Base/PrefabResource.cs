using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class PrefabResource : UnityResource<GameObject>
    {
        public GameObject Instantiate()
        {
            if (_Data == null)
                return null;
            return GameObject.Instantiate((GameObject)_Data);
        }
    }
}
