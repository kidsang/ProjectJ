using System;
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
using EventManager = ProjectK.Base.EventManager;

namespace EditorK
{
    /// <summary>
    /// Interaction logic for TerrainPanel.xaml
    /// </summary>
    public partial class TerrainPanel : UserControl
    {
        private TerrainPanelEntry selectedEntry;
        private bool draging = false;

        public TerrainPanel()
        {
            InitializeComponent();

            for (int i = 0; i < TerrainFlagInfo.Infos.Length; ++i)
            {
                TerrainFlagInfo info = TerrainFlagInfo.Infos[i];
                TerrainPanelEntry entry = new TerrainPanelEntry();
                entry.Load(info);
                entry.Index = i;
                EntryGroup.Children.Add(entry);

                entry.MouseDown += OnEntryMouseDown;
            }
        }

        private void OnEntryMouseDown(object sender, MouseButtonEventArgs e)
        {
            TerrainPanelEntry entry = sender as TerrainPanelEntry;
            if (entry == null)
                return;

            SelectEntry(entry.Index);
        }

        private void SelectEntry(int index)
        {
            TerrainPanelEntry entry = EntryGroup.Children[index] as TerrainPanelEntry;

            if (selectedEntry != null)
                selectedEntry.Deselect();

            selectedEntry = entry;
            entry.Select();
            DetailPanels.Instance.ShowPanel(DetailPanelType.Terrain, index);
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

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                SelectEntry(0);
                EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_DOWN, OnSceneMouseDown);
                EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_UP, OnSceneMouseUp);
                EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_OVER_CELL_CHANGE, OnSceneMouseOverCellChange);
                EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnMouseRightClick);
            }
            else
            {
                EventManager.Instance.UnregisterAll(this);
                DetailPanels.Instance.HidePanel(DetailPanelType.Terrain);
            }
        }

        private void OnMouseRightClick(object[] args)
        {
            DeselectEntry();
        }

        private void OnSceneMouseDown(object[] args)
        {
            EditorMouse mouse = EditorMouse.Instance;
            if (mouse.DataType != EditorMouseDataType.TerrainFill)
                return;

            draging = true;
            SceneDataProxy.Instance.Recording = false;
            OnSceneMouseOverCellChange(null);
        }

        private void OnSceneMouseUp(object[] args)
        {
            if (!draging)
                return;

            SceneDataProxy.Instance.Recording = true;
            OnSceneMouseOverCellChange(null);
            draging = false;
        }

        private void OnSceneMouseOverCellChange(object[] args)
        {
            if (!draging)
                return;

            EditorMouse mouse = EditorMouse.Instance;
            MapCell cell = mouse.OverMapCell;
            if (cell == null)
                return;

            InfoMap infos = mouse.Data as InfoMap;
            MapCellFlag flag = (MapCellFlag)infos["flag"];
            int size = (int)infos["size"];
            bool erase = (bool)infos["erase"];

            SceneDataProxy.Instance.SetTerrainFlag(cell.X, cell.Y, size - 1, flag, !erase);
        }
    }
}
