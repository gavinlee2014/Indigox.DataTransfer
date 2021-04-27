using Indigox.Common.Logging;
using Indigox.DataTransfer.Socket;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Indigox.DataTransfer
{
    public partial class DataTransfer : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            try
            {
                SocketServer.Instance.Start();
                TaskTimer.Instance.Start();
            }
            catch (Exception e)
            {
                this.EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                Log.Error(String.Format("server start fail of error:{0}", e.ToString()));
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            SocketServer.Instance.Stop();
            TaskTimer.Instance.Stop();
            Log.Debug("server paused");
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            SocketServer.Instance.Start();
            TaskTimer.Instance.Start();
            Log.Debug("server resumed");
        }


        protected override void OnStop()
        {

            SocketServer.Instance.Stop();
            TaskTimer.Instance.Stop();
        }
    }
}
