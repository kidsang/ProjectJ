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

        public CsvFile<MonsterEntitySetting> MonsterEntitySettings;

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
            MonsterEntitySettings = LoadCsvFile<MonsterEntitySetting>("Settings/MonsterEntities.csv");
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
                allComplete();
                allComplete = null;
            }
        }

        public static SettingManager Instance
        {
            get { return instance; }
        }
    }
}
