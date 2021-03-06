﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public class SceneSetting
    {
        public MapSetting Map;
        public SpawnSetting Spawn;

        public SceneSetting()
        {
        }

        public SceneSetting(bool initEmpty)
        {
            if (initEmpty)
            {
                Map = new MapSetting(initEmpty);
                Spawn = new SpawnSetting(initEmpty);
            }
        }
    }

    public class SpawnSetting
    {
        public SpawnLocationSetting[] Locations;
        public float WaveIntervalTime; // 波次间刷怪间隔，单位秒

        public SpawnSetting()
        {
        }

        public SpawnSetting(bool initEmpty)
        {
            if (initEmpty)
            {
                Locations = new SpawnLocationSetting[0];
            }
        }
    }

    public class SpawnLocationSetting
    {
        // 刷怪路线，索引至MapPath
        public int PathIndex;

        // 刷怪波次，可以是不连续的，也可以有重复
        // 通过WaveIndex，指定波次，而非在数组中的位置
        public SpawnWaveSetting[] Waves;
    }

    public class SpawnWaveSetting
    {
        public int WaveIndex; // 刷怪波次
        public float DelayTime; // 波次内刷怪延迟，单位秒
        public float IntervalTime; // 波次内刷怪间隔，单位秒
        public int SpawnTimes; // 波次内刷怪次数
        public int SpawnPerTime; // 每次刷怪数量。 该波次刷怪总数 = SpawnTimes * SpawnPerTime
        public int TemplateID; // 怪物模板ID
    }
}
