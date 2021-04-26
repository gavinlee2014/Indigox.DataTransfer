using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger
{
    class Log4netLogger : ILogger
    {

        static Log4netLogger()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        #region ILogger Members

        public void Debug(LogEntry log)
        {
            ILog logger = LogManager.GetLogger(log.CallerType);
            logger.Debug(log);
        }

        public void Info(LogEntry log)
        {
            ILog logger = LogManager.GetLogger(log.CallerType);
            logger.Info(log);
        }

        public void Warn(LogEntry log)
        {
            ILog logger = LogManager.GetLogger(log.CallerType);
            logger.Warn(log);
        }

        public void Error(LogEntry log)
        {
            ILog logger = LogManager.GetLogger(log.CallerType);
            logger.Error(log);
        }

        public void Fatal(LogEntry log)
        {
            ILog logger = LogManager.GetLogger(log.CallerType);
            logger.Fatal(log);
        }

        #endregion
    }
}
