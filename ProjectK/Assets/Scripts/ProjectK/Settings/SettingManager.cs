using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public class SettingManager
    {
        private static SettingManager instance;

        private ResourceLoader loader;
        private int loadingCount;
        private int completeCount;

        public delegate void AllCompleteCallback();
        private AllCompleteCallback allComplete;

        public CsvFile<DamageTypeSetting> DamageTypeSettings;
        public CsvFile<MonsterEntitySetting> MonsterEntitySettings;
        public CsvFile<TowerEntitySetting> TowerEntitySettings;

        public static void Init(AllCompleteCallback allComplete)
        {
            if (instance != null)
                return;

            instance = new SettingManager();
            instance.loader = new ResourceLoader();
            instance.allComplete = allComplete;
            instance.LoadAll();
        }

        private void LoadAll()
        {
            DamageTypeSettings = LoadCsvFile<DamageTypeSetting>("Settings/DamageTypes.csv");
            MonsterEntitySettings = LoadCsvFile<MonsterEntitySetting>("Settings/MonsterEntities.csv");
            TowerEntitySettings = LoadCsvFile<TowerEntitySetting>("Settings/TowerEntities.csv");
        }

        public void ReloadAll(AllCompleteCallback allComplete = null)
        {
            if (loader != null)
            {
                loader.Dispose();
                loader = null;
            }

            loadingCount = 0;
            completeCount = 0;
            loader = new ResourceLoader();
            this.allComplete = allComplete;
            LoadAll();
        }

        private IniFile LoadIniFile(string url)
        {
            ++loadingCount;
            return loader.LoadIniFileAsync(url, OnLoadComplete);
        }

        private CsvFile<T> LoadCsvFile<T>(string url) where T: CsvFileObject, new()
        {
            ++loadingCount;
            return loader.LoadCsvFileAsync<T>(url, OnLoadComplete);
        }

        private void OnLoadComplete(Resource res)
        {
            ++completeCount;

            if (loadingCount == completeCount)
            {
                if (allComplete != null)
                {
                    allComplete();
                    allComplete = null;
                }
            }
        }

        public static SettingManager Instance
        {
            get { return instance; }
        }
    }
}
