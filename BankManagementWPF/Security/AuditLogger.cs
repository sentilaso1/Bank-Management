using System;
using System.IO;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Security
{
    public static class AuditLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audit.log");

        public static void LogPermissionCheck(Permission permission, bool granted)
        {
            var username = CurrentUserSession.CurrentUser?.Username ?? "Unknown";
            var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{username}\t{permission}\t{(granted ? "Granted" : "Denied")}";
            try
            {
                File.AppendAllText(LogFilePath, entry + Environment.NewLine);
            }
            catch
            {
                // ignore logging failures
            }
        }
    }
}
