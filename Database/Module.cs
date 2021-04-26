using Indigox.Common.Data;
using Indigox.Common.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Database
{
    class Module
    {
        private static IDatabase db;

        public static IDatabase Db
        {
            get
            {
                if (db == null)
                {
                    DatabaseFactory factory = new DatabaseFactory();
                    db = factory.CreateDatabase("UUM");
                }
                return db;
            }
        }
    }
}
