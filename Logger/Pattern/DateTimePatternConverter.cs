using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger.Pattern
{
    class DateTimePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
