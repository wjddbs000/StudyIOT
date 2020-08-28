using Caliburn.Micro;
using MqttMonitoringApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MqttMonitoringApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public MainViewModel()
        {
            DisplayName = "MQTT Monitoring App - v0.9";

        }
        protected override void OnDeactivate(bool close)
        {
            if (Commons.BROKERCLIENT.IsConnected)
            {
                Commons.BROKERCLIENT.Disconnect();
                Commons.BROKERCLIENT = null;
            }
            base.OnDeactivate(close);
        }

        public void ExitProgram()
        {
            Environment.Exit(0);
        }
        public void ExitApp()
        {
            ExitProgram();
        }

        public void LoadDataBaseView()
        {
            ActivateItem(new DataBaseViewModel());
        }
        public void LoadRealTimeView()
        {
            ActivateItem(new RealTimeViewModel());
        }
        public void LoadHistoryView()
        {
            ActivateItem(new HistoryViewModel());
        }
        public void PopInfoDialog()
        {
            TaskStart();
        }

        private void TaskStart()
        {
            var wManager = new WindowManager();
            var result = wManager.ShowDialog(new CustomPopupViewModel("New Broker"));
            if (result == true)
            {
                ActivateItem(new DataBaseViewModel());
            }
        }
    }
}
