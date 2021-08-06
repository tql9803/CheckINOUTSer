using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CheckINOUTSer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            DoorConSer ServiceDebug = new DoorConSer();

            //ServiceDebug.Disposed += Atdispose;
            ServiceDebug.OnDebug();
            //ServiceController serviceController = new ServiceController("CheckINOUTSer");
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DoorConSer()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }

        //static void Atdispose(object sender, EventArgs arg)
        //{
        //    DoorConSer ServiceDebug = new DoorConSer();

        //    ServiceDebug.OnDebug();
        //}

    }
}
