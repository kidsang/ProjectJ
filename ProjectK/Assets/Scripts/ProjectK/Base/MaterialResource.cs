using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class MaterialResource : UnityResource<Material>
    {
        public Material Material
        {
            get 
            {
                if (_Data == null)
                    return null;
                return _Data as Material; 
            }
        }
    }
}
