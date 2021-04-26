using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Logger
{
    class LogEntry
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private Dictionary<string, object> context;

        public Dictionary<string, object> Context
        {
            get { return context; }
            set { context = value; }
        }

        private IList<string> category;

        public IList<string> Category
        {
            get { return category; }
            set { category = value; }
        }

        private Exception exception;

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        private Type callerType;

        public Type CallerType
        {
            get { return callerType; }
            set { callerType = value; }
        }
    }
}
