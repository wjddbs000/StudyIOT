using Caliburn.Micro;
using MqttMonitoringApp.Helpers;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttMonitoringApp.ViewModels
{
    public class DataBaseViewModel : Conductor<object>
    {
        private string brokerUrl;
        private string topic;
        private string connString;
        private string dbLog;
        private bool isConnected;
        public string BrokerUrl
        {
            get => brokerUrl;
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }
        public string Topic
        {
            get => topic;
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }
        public string ConnString
        {
            get => connString;
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }

        public string DbLog
        {
            get => dbLog;
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }

        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST;
            Topic = Commons.PUB_TOPIC;
            Commons.CONNSTRING = ConnString = "Server=localhost;Port=3306;" +
                "Database=iot_sensordata;Uid=root;Pwd=mysql_p@ssw0rd";
        }

        public void Connect()
        {



            if (IsConnected) //토글버튼 온
            {
                Commons.BROKERCLIENT = new MqttClient(BrokerUrl);
                try
                {
                    if (Commons.BROKERCLIENT.IsConnected != true)
                    {
                        Commons.BROKERCLIENT.MqttMsgPublishReceived += BROKERCLIENT_MqttMsgPublishReceived;
                        Commons.BROKERCLIENT.Connect("MqttMonitor");
                        Commons.BROKERCLIENT.Subscribe(new string[] { Topic },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        UpdateText(">>> Message : Broker Connected");
                    }

                }
                catch (Exception)
                { }
            }
            else //토글버튼 오프
            {
                try
                {
                    if (Commons.BROKERCLIENT.IsConnected == true)
                    {
                        Commons.BROKERCLIENT.Disconnect();
                        UpdateText(">>> Message : Broker Disconnected...");
                    }
                }
                catch (Exception)
                { }
            }
        }

        private void BROKERCLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            UpdateText($">>> Message : {message}");
            InsertDataBase(message);
        }

        private void InsertDataBase(string message)
        {
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            using (var conn = new MySqlConnection(Commons.CONNSTRING))
            {
                string strInsQuery = "INSERT INTO smarthometbl " +
                                     " ( " +
                                     " Dev_Id, " +
                                     " Curr_Time, " +
                                     " Temp, " +
                                     " Humid, " +
                                     " Press " +
                                     " ) " +
                                     "VALUES " +
                                     " ( " +
                                     " @Dev_Id, " +
                                     " @Curr_Time, " +
                                     " @Temp, " +
                                     " @Humid, " +
                                     " @Press " +
                                     " ) ";
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(strInsQuery, conn);
                    MySqlParameter paramDevId = new MySqlParameter("@Dev_Id", MySqlDbType.VarChar);
                    paramDevId.Value = currDatas["Dev_Id"];
                    cmd.Parameters.Add(paramDevId);

                    MySqlParameter paramCurrTime = new MySqlParameter("@Curr_Time", MySqlDbType.DateTime);
                    paramCurrTime.Value = DateTime.Parse(currDatas["Curr_Time"]);
                    cmd.Parameters.Add(paramCurrTime);

                    MySqlParameter paramTemp = new MySqlParameter("@Temp", MySqlDbType.Float);
                    paramTemp.Value = currDatas["Temp"];
                    cmd.Parameters.Add(paramTemp);

                    MySqlParameter paramHumid = new MySqlParameter("@Humid", MySqlDbType.Float);
                    paramHumid.Value = currDatas["Humid"];
                    cmd.Parameters.Add(paramHumid);

                    MySqlParameter paramPress = new MySqlParameter("@Press", MySqlDbType.Float);
                    paramPress.Value = currDatas["Press"];
                    cmd.Parameters.Add(paramPress);

                    if(cmd.ExecuteNonQuery() == 1)
                    {
                        UpdateText("[DB] Isnserted");
                    }
                    else
                    {
                        UpdateText("[DB] Failed");
                    }

                }
                catch (Exception ex)
                {
                    UpdateText($">>> Message : {ex.Message}");
                }
            }
        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }
    }
}
