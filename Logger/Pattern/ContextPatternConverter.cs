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
    class ContextPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            LogEntry entry = loggingEvent.MessageObject as LogEntry;
            if (entry == null)
            {
                throw new Exception();
            }

            if (Option != null)
            {
                // Write the value for the specified key
                if (entry.Context.ContainsKey(Option))
                {
                    WriteObject(writer, loggingEvent.Repository, entry.Context[Option]);
                }
                else
                {
                    WriteObject(writer, loggingEvent.Repository, null);
                }
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, entry.Context);
            }
        }
    }
}
