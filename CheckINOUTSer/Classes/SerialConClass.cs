using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace CheckINOUTSer.Classes
{
    public class SerialConnection
    {
        public SerialPort ThisPort { get; set;}
        
        private string[] ports;
        private string PortFin;
        private int PortNum;
        private int PortLn;
        private const string ConfirmString = "DoorControl";

        private string RecBuff;
        private string SendBuff;

        public int SerialSetup()
        {
            ports = SerialPort.GetPortNames();
            PortLn = ports.Length;
            PortNum = 0;
            PortFin = "";

            while (PortNum < PortLn && string.IsNullOrEmpty(PortFin))
            {
                ThisPort = new SerialPort(ports[PortNum]);
                ThisPort.BaudRate = 9600;

                try
                {                  

                    //ThisPort.Open();
                    
                    //ThisPort.Close();
                    return 0;
                    //ThisPort.ReadExisting();
                    //RecBuff = ThisPort.ReadLine();
                    //RecBuff = ThisPort.ReadLine();

                    //char[] trimChar = {'\r', '\n'};
                    //RecBuff = RecBuff.TrimEnd(trimChar);

                    //if (RecBuff == ConfirmString)
                    //{
                    //    SendBuff = "CONFIRMED";
                    //    ThisPort.WriteLine(SendBuff);

                    //    PortFin = ports[PortNum];   
                    //    return 0;
                    //}
                    //else
                    //{
                    //    PortNum++;
                    //}

                }
                catch (Exception ex) 
                {
                    return 2;
                }
            }

            return 1;
        }

        public int SendConfirmation(string Cmd)
        {
            SendBuff = Cmd;

            try
            {
                if (!ThisPort.IsOpen)
                {
                    ThisPort.Open();
                }

                ThisPort.WriteLine(SendBuff);

                if (ThisPort.IsOpen)
                {
                    ThisPort.Close();
                }
                return 0;

            }
            catch (Exception ex)
            {
                return 2;
            }

        }
    }
}
