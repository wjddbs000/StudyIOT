using Caliburn.Micro;
using MqttMonitoringApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Messages;
namespace MqttMonitoringApp.ViewModels
{
    class RealTimeViewModel : Conductor<object>
    {
        private double livingTempValue;
        public double LivingTempValue
        {
            get => livingTempValue;
            set
            {
                livingTempValue = value;
                NotifyOfPropertyChange(() => LivingTempValue);
            }
        }
        private double livingHumidValue;
        public double LivingHumidValue
        {
            get => livingHumidValue;
            set
            {
                livingHumidValue = value;
                NotifyOfPropertyChange(() => LivingHumidValue);
            }
        }
        private double livingPressValue;
        public double LivingPressValue
        {
            get => livingPressValue;
            set
            {
                livingPressValue = value;
                NotifyOfPropertyChange(() => LivingPressValue);
            }
        }

        private double diningTempValue;
        public double DiningTempValue
        {
            get => diningTempValue;
            set
            {
                diningTempValue = value;
                NotifyOfPropertyChange(() => DiningTempValue);
            }
        }
        private double diningHumidValue;
        public double DiningHumidValue
        {
            get => diningHumidValue;
            set
            {
                diningHumidValue = value;
                NotifyOfPropertyChange(() => DiningHumidValue);
            }
        }
        private double diningPressValue;
        public double DiningPressValue
        {
            get => diningPressValue;
            set
            {
                diningPressValue = value;
                NotifyOfPropertyChange(() => DiningPressValue);
            }
        }
        private double bedTempValue;
        public double BedTempValue
        {
            get => bedTempValue;
            set
            {
                bedTempValue = value;
                NotifyOfPropertyChange(() => BedTempValue);
            }
        }
        private double bedHumidValue;
        public double BedHumidValue
        {
            get => bedHumidValue;
            set
            {
                bedHumidValue = value;
                NotifyOfPropertyChange(() => BedHumidValue);
            }
        }
        private double bedPressValue;
        public double BedPressValue
        {
            get => bedPressValue;
            set
            {
                bedPressValue = value;
                NotifyOfPropertyChange(() => BedPressValue);
            }
        }
        public RealTimeViewModel()
        {
            LivingTempValue = 0f;

            if(Commons.BROKERCLIENT !=null && Commons.BROKERCLIENT.IsConnected)
            {
                Commons.BROKERCLIENT.MqttMsgPublishReceived += BROKERCLIENT_MqttMsgPublishReceived;
            }
        }

        private void BROKERCLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            switch (currDatas["Dev_Id"].ToString())
            {
                case "LivingRoom":
                    LivingTempValue = double.Parse(currDatas["Temp"]);
                    LivingHumidValue = double.Parse(currDatas["Humid"]);
                    LivingPressValue = double.Parse(currDatas["Press"]);
                    break;
                case "DiningRoom":
                    DiningTempValue = double.Parse(currDatas["Temp"]);
                    DiningHumidValue = double.Parse(currDatas["Humid"]);
                    DiningPressValue = double.Parse(currDatas["Press"]);
                    break;
                case "BedRoom":
                    BedTempValue = double.Parse(currDatas["Temp"]);
                    BedHumidValue = double.Parse(currDatas["Humid"]);
                    BedPressValue = double.Parse(currDatas["Press"]);
                    break;

                default:
                    break;
            }
        }
    }
}
