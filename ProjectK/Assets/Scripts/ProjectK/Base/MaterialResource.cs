﻿using System;
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
                if (Data == null)
                    return null;
                return Data as Material; 
            }
        }
    }
}
