namespace BankManagementSystem.WPF.Security
{
    public enum Permission
    {
        // User Management
        AddUser = 1,
        UpdateUser = 2,
        DeleteUser = 3,
        ViewUsers = 4,
        ManagePermissions = 5,

        // Client Management
        AddClient = 10,
        UpdateClient = 11,
        DeleteClient = 12,
        ViewClients = 13,

        // Transaction Management
        ProcessDeposit = 20,
        ProcessWithdraw = 21,
        ProcessTransfer = 22,
        ViewTransactions = 23,
        ApproveTransactions = 24,

        // Reports & Analytics
        ViewReports = 30,
        ExportData = 31,
        ViewAuditLogs = 32,

        // System Administration
        SystemSettings = 40,
        BackupRestore = 41
    }
}
