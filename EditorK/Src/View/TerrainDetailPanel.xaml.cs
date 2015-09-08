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

namespace EditorK
{
    /// <summary>
    /// Interaction logic for TerrainDetailPanel.xaml
    /// </summary>
    public partial class TerrainDetailPanel : UserControl, IDetailPanel
    {
        private bool erasing = false;
        private TerrainFlagInfo info;

        public TerrainDetailPanel()
        {
            InitializeComponent();

        }

        public void OnShow(object[] args)
        {
            int index = (int)args[0];
            info = TerrainFlagInfo.Infos[index];

            BrushSizeSlider.Value = Properties.Settings.Default.TerrainBrushSize;
            BrushSizeSlider.ValueChanged += OnSizeSliderValueChange;

            SetMouseData();
        }

        public void OnHide()
        {

        }

        private void OnFillEraseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
                return;

            erasing = sender == EraseButton;
            FillButton.IsChecked = !erasing;
            EraseButton.IsChecked = erasing;
            SetMouseData();
        }

        private void OnSizeSliderValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized)
                return;

            Properties.Settings.Default.TerrainBrushSize = (int)BrushSizeSlider.Value;
            Properties.Settings.Default.Save();
            SetMouseData();
        }

        private void SetMouseData()
        {

            //if (!ready)
            //    return;

            //InfoMap infos = new InfoMap();
            //infos["flag"] = info.Flag;
            //infos["size"] = (int)BrushSizeSlider.value;
            //infos["erase"] = erasing;

            //GameObject preview = Instantiate(FillMarkPrefab) as GameObject;
            //preview.GetComponent<SpriteRenderer>().color = info.Color;
            //float scale = (BrushSizeSlider.value - 0.5f) * Mathf.Sqrt(3);
            //preview.transform.localScale = new Vector3(scale, scale);

            //EditorMouse.Instance.Clear();
            //EditorMouse.Instance.SetData(EditorMouseDataType.TerrainFill, infos, preview);

            App.Instance.Net.RemoteCallParams("EditorMouseSetData", (int)EditorMouseDataType.TerrainFill,
                info.Flag, (int)BrushSizeSlider.Value, erasing);
        }
    }
}
