using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger
{
    interface ILogger
    {
        void Debug(LogEntry log);
        void Info(LogEntry log);
        void Warn(LogEntry log);
        void Error(LogEntry log);
        void Fatal(LogEntry log);
    }
}
