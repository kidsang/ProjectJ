using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class UnityResource<T> : Resource where T : UnityEngine.Object
    {
        private T _data;
        private ResourceRequest request;

        internal override void Load()
        {
            _data = Resources.Load<T>(Url);
            if (_data == null)
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", Url, "\nType:", GetType());
            }

            state = ResourceState.Complete;
        }

        internal override void LoadAsync()
        {
            request = Resources.LoadAsync<T>(Url);
            state = ResourceState.Loading;
        }

        internal override void OnLoadAsync()
        {
            if (request.isDone)
            {
                _data = request.asset as T;
                if (_data == null)
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

            if (_data != null)
            {
                // 没办法直接Unload一个GameObject，也许只能调用Resources.UnloadUnusedAssets()
                //Resources.UnloadAsset(gameObject);
                _data = null;
            }

            base.OnDispose();
        }

        public T _Data
        {
            get
            {
                return _data;
            }
        }
    }
}
