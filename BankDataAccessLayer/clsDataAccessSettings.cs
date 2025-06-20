using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDataAccessLayer
{
    public class clsDataAccessSettings
    {
        public static string ConnectionString = "Server=localhost;Port=3306;Database=BankDB;Uid=root;Pwd=senti123;Charset=utf8mb4;SslMode=Required;";
    }
}
