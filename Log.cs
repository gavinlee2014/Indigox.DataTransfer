using Indigox.DataTransfer.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Indigox.DataTransfer
{
    class Log
    {
        private static ILogger logger = LoggerManager.GetLogger();

        public static void Debug(string title)
        {
            Debug(title, null as string);
        }

        public static void Debug(string title, Dictionary<string, object> context)
        {
            Debug(title, context, null);
        }

        public static void Debug(string title, Dictionary<string, object> context, Exception ex)
        {
            Debug(title, null, context, ex);
        }

        public static void Debug(string title, string message)
        {
            Debug(title, message, null);
        }

        public static void Debug(string title, string message, Dictionary<string, object> context)
        {
            Debug(title, message, context, null);
        }

        public static void Debug(string title, string message, Dictionary<string, object> context, Exception ex)
        {
            Debug(0, title, message, null, context, ex);
        }

        public static void Debug(int id, string title, string message, string category)
        {
            Debug(id, title, message, new string[] { category });
        }

        public static void Debug(int id, string title, string message, IList<string> categories)
        {
            Debug(id, title, message, categories, null);
        }

        public static void Debug(int id, string title, string message, string category, Dictionary<string, object> context)
        {
            Debug(id, title, message, new string[] { category }, context);
        }

        public static void Debug(int id, string title, string message, IList<string> categories, Dictionary<string, object> context)
        {
            Debug(id, title, message, categories, context, null);
        }

        public static void Debug(int id, string title, string message, IList<string> categories, Dictionary<string, object> context, Exception ex)
        {
            LogEntry log = new LogEntry();
            log.Id = id;
            log.Title = title;
            log.Message = message;
            log.Category = categories;
            log.Context = context;
            log.Exception = ex;
            log.CallerType = Log.FindCallerType();
            logger.Debug(log);
        }



        public static void Info(string title)
        {
            Info(title, null as string);
        }

        public static void Info(string title, Dictionary<string, object> context)
        {
            Info(title, context, null);
        }

        public static void Info(string title, Dictionary<string, object> context, Exception ex)
        {
            Info(title, null, context, ex);
        }

        public static void Info(string title, string message)
        {
            Info(title, message, null);
        }

        public static void Info(string title, string message, Dictionary<string, object> context)
        {
            Info(title, message, context, null);
        }

        public static void Info(string title, string message, Dictionary<string, object> context, Exception ex)
        {
            Info(0, title, message, null, context, ex);
        }

        public static void Info(int id, string title, string message, string category)
        {
            Info(id, title, message, new string[] { category });
        }

        public static void Info(int id, string title, string message, IList<string> categories)
        {
            Info(id, title, message, categories, null);
        }

        public static void Info(int id, string title, string message, string category, Dictionary<string, object> context)
        {
            Info(id, title, message, new string[] { category }, context);
        }

        public static void Info(int id, string title, string message, IList<string> categories, Dictionary<string, object> context)
        {
            Info(id, title, message, categories, context, null);
        }

        public static void Info(int id, string title, string message, IList<string> categories, Dictionary<string, object> context, Exception ex)
        {
            LogEntry log = new LogEntry();
            log.Id = id;
            log.Title = title;
            log.Message = message;
            log.Category = categories;
            log.Context = context;
            log.Exception = ex;
            log.CallerType = Log.FindCallerType();
            logger.Info(log);
        }


        public static void Warn(string title)
        {
            Warn(title, null as string);
        }

        public static void Warn(string title, Dictionary<string, object> context)
        {
            Warn(title, context, null);
        }

        public static void Warn(string title, Dictionary<string, object> context, Exception ex)
        {
            Warn(title, null, context, ex);
        }

        public static void Warn(string title, string message)
        {
            Warn(title, message, null);
        }

        public static void Warn(string title, string message, Dictionary<string, object> context)
        {
            Warn(title, message, context, null);
        }

        public static void Warn(string title, string message, Dictionary<string, object> context, Exception ex)
        {
            Warn(0, title, message, null, context, ex);
        }

        public static void Warn(int id, string title, string message, string category)
        {
            Warn(id, title, message, new string[] { category });
        }

        public static void Warn(int id, string title, string message, IList<string> categories)
        {
            Warn(id, title, message, categories, null);
        }

        public static void Warn(int id, string title, string message, string category, Dictionary<string, object> context)
        {
            Warn(id, title, message, new string[] { category }, context);
        }

        public static void Warn(int id, string title, string message, IList<string> categories, Dictionary<string, object> context)
        {
            Warn(id, title, message, categories, context, null);
        }

        public static void Warn(int id, string title, string message, IList<string> categories, Dictionary<string, object> context, Exception ex)
        {
            LogEntry log = new LogEntry();
            log.Id = id;
            log.Title = title;
            log.Message = message;
            log.Category = categories;
            log.Context = context;
            log.Exception = ex;
            log.CallerType = Log.FindCallerType();
            logger.Warn(log);
        }


        public static void Error(string title)
        {
            Error(title, null as string);
        }

        public static void Error(string title, Dictionary<string, object> context)
        {
            Error(title, context, null);
        }

        public static void Error(string title, Dictionary<string, object> context, Exception ex)
        {
            Error(title, null, context, ex);
        }

        public static void Error(string title, string message)
        {
            Error(title, message, null);
        }

        public static void Error(string title, string message, Dictionary<string, object> context)
        {
            Error(title, message, context, null);
        }

        public static void Error(string title, string message, Dictionary<string, object> context, Exception ex)
        {
            Error(0, title, message, null, context, ex);
        }

        public static void Error(int id, string title, string message, string category)
        {
            Error(id, title, message, new string[] { category });
        }

        public static void Error(int id, string title, string message, IList<string> categories)
        {
            Error(id, title, message, categories, null);
        }

        public static void Error(int id, string title, string message, string category, Dictionary<string, object> context)
        {
            Error(id, title, message, new string[] { category }, context);
        }

        public static void Error(int id, string title, string message, IList<string> categories, Dictionary<string, object> context)
        {
            Error(id, title, message, categories, context, null);
        }

        public static void Error(int id, string title, string message, IList<string> categories, Dictionary<string, object> context, Exception ex)
        {
            LogEntry log = new LogEntry();
            log.Id = id;
            log.Title = title;
            log.Message = message;
            log.Category = categories;
            log.Context = context;
            log.Exception = ex;
            log.CallerType = Log.FindCallerType();
            logger.Error(log);
        }


        public static void Fatal(string title)
        {
            Fatal(title, null as string);
        }

        public static void Fatal(string title, Dictionary<string, object> context)
        {
            Fatal(title, context, null);
        }

        public static void Fatal(string title, Dictionary<string, object> context, Exception ex)
        {
            Fatal(title, null, context, ex);
        }

        public static void Fatal(string title, string message)
        {
            Fatal(title, message, null);
        }

        public static void Fatal(string title, string message, Dictionary<string, object> context)
        {
            Fatal(title, message, context, null);
        }

        public static void Fatal(string title, string message, Dictionary<string, object> context, Exception ex)
        {
            Fatal(0, title, message, null, context, ex);
        }

        public static void Fatal(int id, string title, string message, string category)
        {
            Fatal(id, title, message, new string[] { category });
        }

        public static void Fatal(int id, string title, string message, IList<string> categories)
        {
            Fatal(id, title, message, categories, null);
        }

        public static void Fatal(int id, string title, string message, string category, Dictionary<string, object> context)
        {
            Fatal(id, title, message, new string[] { category }, context);
        }

        public static void Fatal(int id, string title, string message, IList<string> categories, Dictionary<string, object> context)
        {
            Fatal(id, title, message, categories, context, null);
        }

        public static void Fatal(int id, string title, string message, IList<string> categories, Dictionary<string, object> context, Exception ex)
        {
            LogEntry log = new LogEntry();
            log.Id = id;
            log.Title = title;
            log.Message = message;
            log.Category = categories;
            log.Context = context;
            log.Exception = ex;
            log.CallerType = Log.FindCallerType();
            logger.Fatal(log);
        }


        private static Type FindCallerType()
        {
            StackTrace st = new StackTrace(true);
            int frameIndex = 0;

            while (frameIndex < st.FrameCount)
            {
                StackFrame frame = st.GetFrame(frameIndex);
                if (frame != null && frame.GetMethod().DeclaringType == typeof(Log))
                {
                    break;
                }
                frameIndex++;
            }

            while (frameIndex < st.FrameCount)
            {
                StackFrame frame = st.GetFrame(frameIndex);
                if (frame != null && frame.GetMethod().DeclaringType != typeof(Log))
                {
                    break;
                }
                frameIndex++;
            }

            if (frameIndex >= st.FrameCount)
            {
                throw new Exception();
            }

            StackFrame locationFrame = st.GetFrame(frameIndex);
            if (locationFrame == null)
            {
                throw new Exception();
            }

            MethodBase method = locationFrame.GetMethod();
            if (method == null)
            {
                throw new Exception();
            }

            Type callerType = method.DeclaringType;
            if (callerType == null)
            {
                throw new Exception();
            }

            return callerType;
        }
    }
}
