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
        public CsvFile<BuffSetting> BuffSettings;
        public CsvFile<SkillCompositeSetting> SkillCompositeSettings;

        public CsvFile<ItemTypeSetting> ItemTypeSettings;
        public Dictionary<int, ItemSetting> ItemSettings = new Dictionary<int, ItemSetting>();
        public Dictionary<int, Resource> ItemSettingsByType = new Dictionary<int, Resource>();

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
            DamageTypeSettings = LoadCsvFile<DamageTypeSetting>("Settings/DamageTypes");
            MonsterEntitySettings = LoadCsvFile<MonsterEntitySetting>("Settings/MonsterEntities");
            TowerEntitySettings = LoadCsvFile<TowerEntitySetting>("Settings/TowerEntities");
            BuffSettings = LoadCsvFile<BuffSetting>("Settings/Buffs");
            SkillCompositeSettings = LoadCsvFile<SkillCompositeSetting>("Settings/SkillComposites");

            ItemTypeSettings = LoadCsvFile<ItemTypeSetting>("Settings/Items/ItemTypes");
            ItemSettingsByType[(int)ItemType.Material] = LoadCsvFile<ItemMaterialSetting>("Settings/Items/Materials");
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

        public ItemSetting GetItemSetting(int itemID, bool throwError = true)
        {
            ItemSetting setting;
            ItemSettings.TryGetValue(itemID, out setting);
            if (throwError && setting == null)
                Log.Assert(false, "找不到物品配置表！itemID：", itemID);
            return setting;
        }

        private IniFile LoadIniFile(string url, ResourceLoadComplete callback = null)
        {
            ++loadingCount;
            if (callback != null)
                return loader.LoadIniFileAsync(url, callback);
            else
                return loader.LoadIniFileAsync(url, OnLoadComplete);
        }

        private CsvFile<T> LoadCsvFile<T>(string url, ResourceLoadComplete callback = null) where T: CsvFileObject, new()
        {
            ++loadingCount;
            if (callback != null)
                return loader.LoadCsvFileAsync<T>(url, OnLoadComplete);
            else
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
