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
using System.IO;
using Microsoft.Win32;
using ProjectK;
using ProjectK.Base;
using EditorK.Properties;

namespace EditorK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoLoadFile(SceneSetting data, string path = null)
        {
            RemoteDataProxy.Instance.Load(data, path);
        }

        private void DoSaveFile(string path)
        {
            SceneSetting data = RemoteDataProxy.Instance.SceneData;
            string jsonData = SimpleJson.SerializeObject(data);
            File.WriteAllText(path, jsonData);
        }

        private void CmdNewFile(object sender, ExecutedRoutedEventArgs e)
        {
            SceneSetting data = new SceneSetting(true);
            data.Map.CellCountX = 10;
            data.Map.CellCountY = 10;
            DoLoadFile(data);
        }

        private void CmdOpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "打开文件";
            dialog.Filter = "地图文件(*.map)|*.map";
            dialog.InitialDirectory = Settings.Default.LastOpenFilePath;
            if (dialog.ShowDialog(App.Instance.MainWindow) == true)
            {
                Settings.Default.LastOpenFilePath = dialog.FileName;
                Settings.Default.Save();

                string jsonData = File.ReadAllText(dialog.FileName);
                SceneSetting data = SimpleJson.DeserializeObject<SceneSetting>(jsonData);
                DoLoadFile(data, dialog.FileName);
            }
        }

        private void CmdSaveFile(object sender, ExecutedRoutedEventArgs e)
        {
            CmdSaveAsFile(sender, e);
        }

        private void CmdSaveAsFile(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "保存文件";
            dialog.Filter = "地图文件(*.map)|*.map";
            dialog.InitialDirectory = Settings.Default.LastSaveFilePath;
            if (dialog.ShowDialog(App.Instance.MainWindow) == true)
            {
                Settings.Default.LastSaveFilePath = dialog.FileName;
                Settings.Default.Save();

                DoSaveFile(dialog.FileName);
            }
        }

        private void CmdUndo(object sender, ExecutedRoutedEventArgs e)
        {
            RemoteDataProxy.Instance.Undo();
        }

        private void CmdRedo(object sender, ExecutedRoutedEventArgs e)
        {
            RemoteDataProxy.Instance.Redo();
        }
    }
}
