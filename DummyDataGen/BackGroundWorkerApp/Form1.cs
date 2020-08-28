using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackGroundWorkerApp
{
    public partial class Form1 : Form
    {
        public BackgroundWorker BgwTest { get;set; }
        public Form1()
        {
            InitializeComponent();

            BgwTest = new BackgroundWorker();
            BgwTest.DoWork += BgwTest_DoWork;
            BgwTest.RunWorkerCompleted += BgwTest_RunWorkerCompleted;
            BgwTest.ProgressChanged += BgwTest_ProgressChanged;

        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if(BgwTest.IsBusy != true)
            {
                BgwTest.RunWorkerAsync();
                LblResult.Text = "실행";
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (BgwTest.WorkerSupportsCancellation)
            {
                BgwTest.CancelAsync();
                LblResult.Text = "실행 취소";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BgwTest.WorkerReportsProgress = true;
            BgwTest.WorkerSupportsCancellation = true;
        }

        private void BgwTest_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker) sender;

            for (int i = 0; i <= 100; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    Thread.Sleep(20);
                    worker.ReportProgress(i);
                }
            }
        }

        private void BgwTest_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //LblResult.Text = $"{e.ProgressPercentage} %";
            PgbTest.Value = e.ProgressPercentage;
        }

        private void BgwTest_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                LblResult.Text = "실행 취소";
            }
            else if (e.Error !=null)
            {
                LblResult.Text = $"에러 : {e.Error.Message}";
            }
            else
            {
                LblResult.Text = "실행 완료";
            }
        }
    }
}
