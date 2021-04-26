using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger
{
    class LoggerManager
    {
        public static ILogger GetLogger()
        {
            return new Log4netLogger();
        }
    }
}
