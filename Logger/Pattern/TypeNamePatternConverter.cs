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
    class TypeNamePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            LogEntry log = loggingEvent.MessageObject as LogEntry;
            if (log == null)
            {
                throw new Exception();
            }

            writer.Write(log.CallerType.FullName);
        }
    }
}
