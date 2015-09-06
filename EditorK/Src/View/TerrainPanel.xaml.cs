﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    /// <summary>
    /// Interaction logic for TerrainPanel.xaml
    /// </summary>
    public partial class TerrainPanel : UserControl
    {
        private TerrainPanelEntry selectedEntry;

        public TerrainPanel()
        {
            InitializeComponent();

            foreach (TerrainFlagInfo info in TerrainFlagInfo.Infos)
            {
                TerrainPanelEntry entry = new TerrainPanelEntry();
                entry.Load(info);
                EntryGroup.Children.Add(entry);

                entry.MouseDown += OnEntryMouseDown;
            }
        }

        private void OnEntryMouseDown(object sender, MouseButtonEventArgs e)
        {
            TerrainPanelEntry entry = sender as TerrainPanelEntry;
            if (entry == null)
                return;

            if (selectedEntry != null)
                selectedEntry.Deselect();

            selectedEntry = entry;
            selectedEntry.Select();
        }

        private void OnToggleAllVisibleClick(object sender, RoutedEventArgs e)
        {
            bool visible = false;
            int visibleFlags = 0;

            foreach (var child in EntryGroup.Children)
            {
                TerrainPanelEntry entry = child as TerrainPanelEntry;
                visible = visible || !(bool)entry.CheckField.IsChecked;
            }

            foreach (var child in EntryGroup.Children)
            {
                TerrainPanelEntry entry = child as TerrainPanelEntry;
                entry.CheckField.IsChecked = visible;
                EditorUtils.SetFlag(ref visibleFlags, (int)entry.Flag, visible);
            }

            Properties.Settings.Default.TerrainVisibleFlags = visibleFlags;
            Properties.Settings.Default.Save();
        }
    }
}