using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigox.DataTransfer.Database
{
    class DBUtil
    {
        public static string QueryUserPassword(string accountName)
        {
            string encoded = (string)Module.Db.ScalarText("SELECT AccountPassword FROM Users WHERE AccountName='" + accountName + "'");
            string origin = "";
            if (!String.IsNullOrEmpty(encoded))
            {
                origin = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            }
            return origin;
        }

        public static void UpdateUserPassword(string accountName, string pwd)
        {
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(pwd));
            Module.Db.ExecuteText("UPDATE Users SET AccountPassword='" + pwd + "' WHERE AccountName='" + encoded + "'");
        }
    }
}
