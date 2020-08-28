using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace BogusMqttPubApp
{
    class Program
    {
        public static string MqttBrokerUrl { get; private set; }

        public static MqttClient BrokerClient { get; set; }

        private static Thread MqttThread { get; set; }

        private static Faker<SensorInfo> SensorFaker { get; set; }

        private static string CurrValue { get; set; }

        static void Main(string[] args)
        {
            InitializeAll(); //전체 초기화
            ConnectMqttBroker(); //MQTT브로커 접속
            StartPublish(); //fake 센싱 메시지 전송

        }

        private static void InitializeAll()
        {
            //MqttBrokerUrl = "localhost"; // 또는 127.0.0.1 / 210.119.12.76
            MqttBrokerUrl = "210.119.12.60"; // 또는 127.0.0.1 / 210.119.12.76

            string[] Rooms = new[] { "DiningRoom", "LivingRoom", "BathRoom", "BedRoom", "GuestRoom" };

            SensorFaker = new Faker<SensorInfo>()
            .RuleFor(o => o.Dev_Id, f => f.PickRandom(Rooms))
            .RuleFor(o => o.Curr_Time, f => f.Date.Past(0).ToString("yyyy-MM-dd HH:mm:ss.ff"))
            .RuleFor(o => o.Temp, f => float.Parse(f.Random.Float(19.0f, 35.9f).ToString("0.00")))
            .RuleFor(o => o.Humid, f => float.Parse(f.Random.Float(40.0f, 63.9f).ToString("0.0")))
            .RuleFor(o => o.Press, f => float.Parse(f.Random.Float(800.0f, 999.9f).ToString("0.0")));
        }

        private static void ConnectMqttBroker()
        {
            BrokerClient = new MqttClient(MqttBrokerUrl);
            BrokerClient.Connect("FakerDaemon");
        }

        private static void StartPublish()
        {
            MqttThread = new Thread(new ThreadStart(LoopPublish));
            //MqttThread = new Thread(() => LoopPublish);
            MqttThread.Start();
        }

        private static void LoopPublish()
        {
            while (true)
            {
                SensorInfo value = SensorFaker.Generate();
                CurrValue = JsonConvert.SerializeObject(value, Formatting.Indented);
                BrokerClient.Publish("home/device/data/", Encoding.Default.GetBytes(CurrValue));
                Console.WriteLine($"Publish : {CurrValue}");

                Thread.Sleep(1000);
            }
        }
    }
}
