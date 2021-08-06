using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using CheckINOUTSer.Classes;
using System.Timers;
using System.Diagnostics;

namespace CheckINOUTSer
{
    public partial class DoorConSer : ServiceBase
    {
        public NFCReader nfcreader = new NFCReader();

        private SerialConnection PortConn = new SerialConnection();

        private bool Checked = false;
        //private bool isMem = false;

        System.Timers.Timer OpenTm = new System.Timers.Timer
        {
            Interval = 5000,
            AutoReset = false
        };

        public DoorConSer()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {

            int SerialSta = PortConn.SerialSetup();
            string cardUID;

            TableProcess checkinandout = new TableProcess();

            //Process CurPro = Process.GetCurrentProcess();
            //long ProMem = CurPro.PrivateMemorySize64;

            OpenTm.Elapsed += atTimerTick;
            

            do
            {

                if (!Checked)
                {
                    if (nfcreader.Connect())
                    {
                        checkinandout = new TableProcess();
                        cardUID = nfcreader.GetCardUID();
                        int buf = checkinandout.CheckInAndOut(cardUID);

                        if (buf == 1)
                        {
                            Checked = true;
                            //isMem = true;
                            PortConn.SendConfirmation("OPEN");
                        }
                        else
                        {
                            Checked = true;
                            //isMem = false;
                            PortConn.SendConfirmation("CLOSE");
                        }

                        cardUID = null;
                        checkinandout.Dispose();
                    }                   

                }
                else
                {
                    //if (isMem)
                    //{
                    //    OpenTm.Interval = 5000;
                    //}
                    //else
                    //{
                    //    OpenTm.Interval = 2000;
                    //}
                    OpenTm.Start();
                    
                }
                    
            }
            while (true);
        }

        private void atTimerTick(object sender, EventArgs ar)
        {
            //if(isMem)
            PortConn.SendConfirmation("CLOSE");

            OpenTm.Stop();
            Checked = false;
            //isMem = false;
        }

        protected override void OnStop()
        {
            nfcreader.Disconnect();
            OpenTm.Dispose();
            //PortConn.ThisPort.Close();
            //PortConn = null;

            this.Dispose();
        }
    }
}
