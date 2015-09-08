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

namespace EditorK
{
    /// <summary>
    /// Interaction logic for TerrainPanelEntry.xaml
    /// </summary>
    public partial class TerrainPanelEntry : UserControl
    {
        public int Index;
        public MapCellFlag Flag;
        public Color Color;

        public TerrainPanelEntry()
        {
            InitializeComponent();
        }

        public void Load(TerrainFlagInfo info)
        {
            Flag = info.Flag;
            Color = Color.FromRgb((byte)(info.Color.r * 255), (byte)(info.Color.g * 255), (byte)(info.Color.b * 255));

            ColorBlock.Background = new SolidColorBrush(Color);
            NameField.Content = info.Name;
            TypeField.Content = info.FlagName;

            int visibleFlags = Properties.Settings.Default.TerrainVisibleFlags;
            CheckField.IsChecked = EditorUtils.HasFlag(visibleFlags, (int)Flag);
        }

        public void Select()
        {
            Background = new SolidColorBrush(Color * 0.5f);
        }

        public void Deselect()
        {
            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void OnCheckFieldClick(object sender, RoutedEventArgs e)
        {
            int visibleFlags = Properties.Settings.Default.TerrainVisibleFlags;
            bool visible = (bool)CheckField.IsChecked;
            EditorUtils.SetFlag(ref visibleFlags, (int)Flag, visible);
            Properties.Settings.Default.TerrainVisibleFlags = visibleFlags;
            Properties.Settings.Default.Save();


            //if (entry != selectedEntry)
            //    GameEditor.Instance.Map.ToggleShowTerrain(flag, visible);
        }
    }
}
