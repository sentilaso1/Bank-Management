using System.Collections.Generic;

namespace BankManagementSystem.WPF.Security
{
    public static class RolePermissions
    {
        public static Dictionary<string, List<Permission>> GetRolePermissions()
        {
            return new Dictionary<string, List<Permission>>(System.StringComparer.OrdinalIgnoreCase)
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
                    // Managers can work with clients
                    Permission.AddClient, Permission.UpdateClient, Permission.DeleteClient, Permission.ViewClients,
                    // Managers handle transactions
                    Permission.ProcessDeposit, Permission.ProcessWithdraw, Permission.ProcessTransfer,
                    Permission.ViewTransactions, Permission.ApproveTransactions
                },
                // Regular users can manage their own transactions
                ["User"] = new List<Permission>
                {
                    Permission.ProcessDeposit,
                    Permission.ProcessWithdraw,
                    Permission.ProcessTransfer,
                    Permission.ViewTransactions
                },
                ["Viewer"] = new List<Permission>()
            };
        }
    }
}
