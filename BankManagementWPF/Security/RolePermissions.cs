using System.Collections.Generic;

namespace BankManagementSystem.WPF.Security
{
    public static class RolePermissions
    {
        public static Dictionary<string, List<Permission>> GetRolePermissions()
        {
            return new Dictionary<string, List<Permission>>
            {
                ["Administrator"] = new List<Permission>
                {
                    Permission.AddUser, Permission.UpdateUser, Permission.DeleteUser, Permission.ViewUsers, Permission.ManagePermissions,
                    Permission.AddClient, Permission.UpdateClient, Permission.DeleteClient, Permission.ViewClients,
                    Permission.ProcessDeposit, Permission.ProcessWithdraw, Permission.ProcessTransfer, Permission.ViewTransactions, Permission.ApproveTransactions,
                    Permission.ViewReports, Permission.ExportData, Permission.ViewAuditLogs,
                    Permission.SystemSettings, Permission.BackupRestore
                },
                ["Manager"] = new List<Permission>
                {
                    Permission.AddUser, Permission.UpdateUser, Permission.ViewUsers,
                    Permission.AddClient, Permission.UpdateClient, Permission.DeleteClient, Permission.ViewClients,
                    Permission.ProcessDeposit, Permission.ProcessWithdraw, Permission.ProcessTransfer, Permission.ViewTransactions, Permission.ApproveTransactions,
                    Permission.ViewReports, Permission.ExportData, Permission.ViewAuditLogs
                },
                ["Cashier"] = new List<Permission>
                {
                    Permission.AddClient, Permission.UpdateClient, Permission.ViewClients,
                    Permission.ProcessDeposit, Permission.ProcessWithdraw, Permission.ProcessTransfer, Permission.ViewTransactions,
                    Permission.ViewReports
                },
                ["Viewer"] = new List<Permission>
                {
                    Permission.ViewUsers,
                    Permission.ViewClients,
                    Permission.ViewTransactions,
                    Permission.ViewReports
                }
            };
        }
    }
}
