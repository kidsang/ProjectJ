﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectK
{
    public class BuildTowerUI : UIBase
    {
        private BuildTowerUIDetail detail;
        private List<Button> buttons = new List<Button>();

        private static readonly int BoxWidth = 128;

        private MapCell cell;

        protected override void Init()
        {
            detail = GameObject.GetComponent<BuildTowerUIDetail>();
            buttons.Add(detail._Icon0);
            buttons.Add(detail._Icon1);
            buttons.Add(detail._Icon2);
            buttons.Add(detail._Icon3);
            buttons.Add(detail._Icon4);
            buttons.Add(detail._Icon5);

            Player player = Player.Me;
            List<int> towers = player.SelectedTowers;
            int numButtons = buttons.Count;
            int numTowers = towers.Count;
            for (int i = 0; i < numButtons; ++i)
            {
                Button button = buttons[i];
                GameObject buttonObject = button.gameObject;

                if (i >= numTowers)
                {
                    buttonObject.SetActive(false);
                    continue;
                }

                buttonObject.SetActive(true);
                float offsetX = (i - (int)(numTowers / 2)) * BoxWidth;
                if (numTowers % 2 == 0)
                    offsetX += BoxWidth / 2;
                buttonObject.transform.localPosition = new Vector3(offsetX, 0);

                int towerID = towers[i];
                TowerEntitySetting towerSetting = SettingManager.Instance.TowerEntitySettings.GetValue(towerID);
                Sprite sprite = Loader.LoadSprite(towerSetting.Icon).Sprite;
                button.image.sprite = sprite;
                button.onClick.AddListener(OnTowerButtonClick(i));
            }

            GameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BoxWidth * numTowers);
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
            cell = null;
        }

        protected override void OnRefresh(params object[] args)
        {
            cell = (MapCell)args[0];
        }

        private UnityEngine.Events.UnityAction OnTowerButtonClick(int index)
        {
            return () =>
            {
                if (cell == null)
                    return;

                Scene scene = SceneManager.Instance.Scene;
                int towerID = Player.Me.SelectedTowers[index];
                TowerEntity towerEntity = scene.CreateTowerEntity(towerID);

                // TODO:
                towerEntity.AddComp<SkillComp1001>();
                towerEntity.AddComp<SkillComp1005>();
                towerEntity.AddComp<SkillComp2012>();
                towerEntity.AddComp<SkillComp2013>();
                towerEntity.AttrComp.AtkRangeBase = 4;
                towerEntity.ShowDebugDraw = true;

                scene.AddTowerEntity(towerEntity, cell.Position);

                Remove();
            };
        }
    }
}
