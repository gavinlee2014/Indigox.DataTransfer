using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger.Pattern
{
    class ExceptionPatternConverter : PatternLayoutConverter
    {
        public ExceptionPatternConverter()
        {
            this.IgnoresException = false;
        }

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            LogEntry log = loggingEvent.MessageObject as LogEntry;
            if (log == null)
            {
                throw new Exception();
            }
            if (log.Exception == null)
            {
                return;
            }
            writer.Write(log.Exception.ToString());
        }
    }
}
