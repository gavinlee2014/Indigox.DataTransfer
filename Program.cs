using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DataTransfer()
            };
            ServiceBase.Run(ServicesToRun);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        static void HandleException(Exception e)
        {
            
            //Handle your Exception here
        }

    }
}
