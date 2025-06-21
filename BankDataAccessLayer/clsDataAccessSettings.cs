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
        private static readonly string Server =
            Environment.GetEnvironmentVariable("BANK_DB_SERVER") ?? "localhost";
        private static readonly string Port =
            Environment.GetEnvironmentVariable("BANK_DB_PORT") ?? "3306";
        private static readonly string Database =
            Environment.GetEnvironmentVariable("BANK_DB_NAME") ?? "BankDB";
        private static readonly string User =
            Environment.GetEnvironmentVariable("BANK_DB_USER") ?? "root";
        private static readonly string Password =
            Environment.GetEnvironmentVariable("BANK_DB_PASSWORD") ?? string.Empty;

        public static string ConnectionString =>
            $"Server={Server};Port={Port};Database={Database};Uid={User};Pwd={Password};Charset=utf8mb4;SslMode=Required;";
    }
}
