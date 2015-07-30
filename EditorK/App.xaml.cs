using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ProjectK.Base;

namespace EditorK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get; private set; }

        public EditorServer Net { get; private set; }
        private DispatcherTimer timer;

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            Instance = this;

            Net = new EditorServer();
            Net.Init(OnConnectedCallback, this);

            timer = new DispatcherTimer();
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        private void OnConnectedCallback()
        {
        }

        private void OnTimerTick(object sender, EventArgs args)
        {
            Net.Activate();
        }


    }
}
