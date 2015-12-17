using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class MaterialResource : Resource
    {
        private Material material;

        internal override void Load()
        {
            material = Resources.Load<Material>(Url);
            if (material == null)
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", Url, "\nType:", GetType());
            }

            state = ResourceState.Complete;
        }
        
        internal override void LoadAsync()
        {
            Load();
        }

        internal override void OnLoadAsync()
        {
        }

        protected override void OnDispose()
        {
            if (material != null)
            {
                Resources.UnloadAsset(material);
                material = null;
            }

            base.OnDispose();
        }

        public Material Material
        {
            get { return material; }
        }
    }
}
