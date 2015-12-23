using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class SpriteResource : UnityResource<Sprite>
    {
        public Sprite Sprite
        {
            get 
            {
                if (Data == null)
                    return null;
                return Data as Sprite; 
            }
        }
    }
}

