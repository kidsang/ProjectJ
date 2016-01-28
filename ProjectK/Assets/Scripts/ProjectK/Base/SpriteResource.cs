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
                if (_Data == null)
                    return null;
                return _Data as Sprite; 
            }
        }
    }
}

