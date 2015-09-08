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

namespace EditorK
{
    using DetailsPanelDict = Dictionary<DetailPanelType, IDetailPanel>;

    public enum DetailPanelType
    {
        // int index
        Terrain,
    }

    /// <summary>
    /// Interaction logic for DetailPanels.xaml
    /// </summary>
    public partial class DetailPanels : UserControl
    {
        private static DetailPanels instance;
        public static DetailPanels Instance { get { return instance; } }

        private DetailsPanelDict panels = new DetailsPanelDict();
        private HashSet<IDetailPanel> showingPanels = new HashSet<IDetailPanel>();

        public DetailPanels()
        {
            InitializeComponent();
            instance = this;

            panels[DetailPanelType.Terrain] = new TerrainDetailPanel();
        }

        public void ShowPanel(DetailPanelType panelType, params object[] args)
        {
            IDetailPanel panel = panels[panelType];
            if (!showingPanels.Contains(panel))
            {
                showingPanels.Add(panel);
                panelGroup.Children.Add(panel as UIElement);
            }

            panel.OnShow(args);
        }

        public void HidePanel(DetailPanelType panelType)
        {
            IDetailPanel panel = panels[panelType];
            if (showingPanels.Contains(panel))
            {
                showingPanels.Remove(panel);
                panelGroup.Children.Remove(panel as UIElement);
                panel.OnHide();
            }
        }

        public void HidePanels()
        {
            panelGroup.Children.RemoveRange(0, panelGroup.Children.Count);
            showingPanels.Clear();
        }
    }
}
