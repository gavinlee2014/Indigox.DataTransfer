using Indigox.DataTransfer.Logger.Pattern;
using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;
using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger
{
    class LogEntryLayout : LayoutSkeleton
    {
        public const string DefaultConversionPattern = "%title";

        private static Hashtable globalPatternsRegistry;

        private static Hashtable patternsRegistry = new Hashtable();

        private PatternConverter head;

        private string pattern;

        public string ConversionPattern
        {
            get { return pattern; }
            set { pattern = value; }
        }

        static LogEntryLayout()
        {
            globalPatternsRegistry = new Hashtable(12);

            Assembly assembly = Assembly.GetAssembly(typeof(LogEntryLayout).BaseType);
            globalPatternsRegistry.Add("literal", assembly.GetType("log4net.Util.PatternStringConverters.LiteralPatternConverter"));
            globalPatternsRegistry.Add("newline", assembly.GetType("log4net.Util.PatternStringConverters.NewLinePatternConverter"));
            globalPatternsRegistry.Add("thread", assembly.GetType("log4net.Layout.Pattern.ThreadPatternConverter"));
            globalPatternsRegistry.Add("level", assembly.GetType("log4net.Layout.Pattern.LevelPatternConverter"));
            globalPatternsRegistry.Add("n", assembly.GetType("log4net.Util.PatternStringConverters.NewLinePatternConverter"));

            globalPatternsRegistry.Add("i", typeof(IdentityPatternConverter));
            globalPatternsRegistry.Add("id", typeof(IdentityPatternConverter));

            globalPatternsRegistry.Add("C", typeof(TypeNamePatternConverter));
            globalPatternsRegistry.Add("class", typeof(TypeNamePatternConverter));

            globalPatternsRegistry.Add("m", typeof(MessagePatternConverter));
            globalPatternsRegistry.Add("message", typeof(MessagePatternConverter));

            globalPatternsRegistry.Add("t", typeof(TitlePatternConverter));
            globalPatternsRegistry.Add("title", typeof(TitlePatternConverter));

            globalPatternsRegistry.Add("c", typeof(CategoryPatternConverter));
            globalPatternsRegistry.Add("category", typeof(CategoryPatternConverter));

            globalPatternsRegistry.Add("x", typeof(ContextPatternConverter));
            globalPatternsRegistry.Add("context", typeof(ContextPatternConverter));

            globalPatternsRegistry.Add("e", typeof(ExceptionPatternConverter));
            globalPatternsRegistry.Add("exception", typeof(ExceptionPatternConverter));

            globalPatternsRegistry.Add("d", typeof(DateTimePatternConverter));
            globalPatternsRegistry.Add("date", typeof(DateTimePatternConverter));
        }

        public LogEntryLayout()
            : this(DefaultConversionPattern)
        {
        }

        public LogEntryLayout(string pattern)
        {
            this.IgnoresException = true;

            this.pattern = pattern;
            if (this.pattern == null)
            {
                this.pattern = DefaultConversionPattern;
            }

            ActivateOptions();
        }

        protected virtual PatternParser CreatePatternParser(string pattern)
        {
            PatternParser patternParser = new PatternParser(pattern);

            foreach (DictionaryEntry entry in globalPatternsRegistry)
            {
                patternParser.PatternConverters[entry.Key] = entry.Value;
            }
            foreach (DictionaryEntry entry in patternsRegistry)
            {
                patternParser.PatternConverters[entry.Key] = entry.Value;
            }

            return patternParser;
        }

        public override void ActivateOptions()
        {
            head = CreatePatternParser(pattern).Parse();

            PatternConverter curConverter = head;
            while (curConverter != null)
            {
                PatternLayoutConverter layoutConverter = curConverter as PatternLayoutConverter;
                if (layoutConverter != null)
                {
                    if (!layoutConverter.IgnoresException)
                    {
                        // Found converter that handles the exception
                        this.IgnoresException = false;

                        break;
                    }
                }
                curConverter = curConverter.Next;
            }
        }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            PatternConverter c = head;

            while (c != null)
            {
                c.Format(writer, loggingEvent);
                c = c.Next;
            }
        }
    }
}
