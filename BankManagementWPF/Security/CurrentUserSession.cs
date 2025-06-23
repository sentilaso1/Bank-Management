using BankBusinessLayer;

namespace BankManagementSystem.WPF.Security
{
    public static class CurrentUserSession
    {
        public static User CurrentUser { get; private set; }
        public static PermissionService PermissionService { get; private set; }

        public static void SetUser(User user)
        {
            CurrentUser = user;
        }

        public static void SetPermissionService(PermissionService service)
        {
            PermissionService = service;
        }

        public static bool HasPermission(Permission permission)
        {
            return PermissionService?.HasPermission(permission) ?? false;
        }

        public static void CheckPermission(Permission permission)
        {
            PermissionService?.CheckPermission(permission);
        }

        public static void Logout()
        {
            CurrentUser = null;
            PermissionService = null;
        }
    }
}
