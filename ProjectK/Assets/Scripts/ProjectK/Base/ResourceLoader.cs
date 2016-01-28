using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class ResourceLoader : Disposable
    {
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private ResourceManager manager = ResourceManager.Instance;

        private bool batching = false;
        private int batchTotalCount = 0;
        private int batchCompleteCount = 0;

        public IniFile LoadIniFile(string url)
        {
            return Load<IniFile>(url);
        }

        public IniFile LoadIniFileAsync(string url, ResourceLoadComplete onLoadComplete)
        {
            return LoadAsync<IniFile>(url, onLoadComplete);
        }

        public CsvFile<T> LoadCsvFile<T>(string url) where T : CsvFileObject, new()
        {
             return Load<CsvFile<T>>(url);
        }

        public CsvFile<T> LoadCsvFileAsync<T>(string url, ResourceLoadComplete onLoadComplete) where T : CsvFileObject, new()
        {
            return LoadAsync<CsvFile<T>>(url, onLoadComplete);
        }

        public JsonFile<T> LoadJsonFile<T>(string url)
        {
             return Load<JsonFile<T>>(url);
        }

        public JsonFile<T> LoadJsonFileAsync<T>(string url, ResourceLoadComplete onLoadComplete)
        {
            return LoadAsync<JsonFile<T>>(url, onLoadComplete);
        }

        public JsonFile LoadJsonFile(string url)
        {
             return Load<JsonFile>(url);
        }

        public JsonFile LoadJsonFileAsync(string url, ResourceLoadComplete onLoadComplete)
        {
            return LoadAsync<JsonFile>(url, onLoadComplete);
        }

        public PrefabResource LoadPrefab(string url)
        {
            return Load<PrefabResource>(url);
        }

        public PrefabResource LoadPrefabAsync(string url, ResourceLoadComplete onLoadComplete = null)
        {
            return LoadAsync<PrefabResource>(url, onLoadComplete);
        }

        public MaterialResource LoadMaterial(string url)
        {
            return Load<MaterialResource>(url);
        }

        public MaterialResource LoadMaterialAsync(string url, ResourceLoadComplete onLoadComplete = null)
        {
            return LoadAsync<MaterialResource>(url, onLoadComplete);
        }

        public SpriteResource LoadSprite(string url)
        {
            return Load<SpriteResource>(url);
        }

        public SpriteResource LoadSpriteAsync(string url, ResourceLoadComplete onLoadComplete = null)
        {
            return LoadAsync<SpriteResource>(url, onLoadComplete);
        }

        public T Load<T>(string url) where T: Resource, new()
        {
            Resource res;
            if (!resources.TryGetValue(url, out res))
            {
                res = manager.GetResource<T>(url);
                resources[url] = res;
            }

            // 用户有可能先调用LoadAsync()，再调用Load()
            // 那么为了保证资源加载完成，这里统一调用一次res.Load()
            if (!res.Complete)
                res.Load();

            return (T)res;
        }

        public T LoadAsync<T>(string url, ResourceLoadComplete onLoadComplete = null) where T: Resource, new()
        {
            Resource res;
            if (!resources.TryGetValue(url, out res))
            {
                res = manager.GetResource<T>(url);
                resources[url] = res;
            }

            if (onLoadComplete != null)
                res.OnLoadComplete += onLoadComplete;

            if (batching)
            {
                batchTotalCount += 1;
                res.OnLoadComplete += OnBatchResourceComplete;
            }

            manager.AppendResource(res);
            return (T)res;
        }

        private void OnBatchResourceComplete(Resource res)
        {
            batchCompleteCount += 1;
        }

        /// <summary>
        /// 开始统计接下来的所有LoadAsync操作
        /// 随后可以通过BatchProgress查询总进度
        /// </summary>
        public void BeginBatch()
        {
            if (!batching)
            {
                batching = true;
                batchTotalCount = 0;
                batchCompleteCount = 0;
            }
        }

        /// <summary>
        /// 结束统计LoadAsync操作
        /// </summary>
        public void EndBatch()
        {
            batching = false;
        }

        public float BatchProgress
        {
            get 
            {
                if (batchTotalCount == 0)
                    return 1;
                return batchCompleteCount / (float)batchTotalCount;
            }
        }

        protected override void OnDispose()
        {
            foreach (Resource res in resources.Values)
            {
                res.DecRef();
            }
            resources = null;

            manager = null;

            base.OnDispose();
        }
    }
}
