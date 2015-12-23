using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class UnityResource<T> : Resource where T : UnityEngine.Object
    {
        private T data;
        private ResourceRequest request;

        internal override void Load()
        {
            data = Resources.Load<T>(Url);
            if (data == null)
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", Url, "\nType:", GetType());
            }

            state = ResourceState.Complete;
        }

        internal override void LoadAsync()
        {
            request = Resources.LoadAsync<GameObject>(Url);
            state = ResourceState.Loading;
        }

        internal override void OnLoadAsync()
        {
            if (request.isDone)
            {
                data = request.asset as T;
                if (data == null)
                {
                    loadFailed = true;
                    Log.Error("资源加载错误! Url:", Url, "\nType:", GetType());
                }

                request = null;
                state = ResourceState.Complete;
            }
        }

        protected override void OnDispose()
        {
            if (request != null)
            {
                request = null;
            }

            if (data != null)
            {
                // 没办法直接Unload一个GameObject，也许只能调用Resources.UnloadUnusedAssets()
                //Resources.UnloadAsset(gameObject);
                data = null;
            }

            base.OnDispose();
        }

        public T Data
        {
            get
            {
                return data;
            }
        }
    }
}
