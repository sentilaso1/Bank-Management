using System;
using System.Collections.Generic;

namespace BankManagementSystem.WPF.Security
{
    public class PermissionService
    {
        private readonly string _currentUserRole;
        private readonly HashSet<Permission> _userPermissions;

        public PermissionService(string userRole)
        {
            _currentUserRole = userRole;
            if (!RolePermissions.GetRolePermissions().TryGetValue(userRole, out var perms))
            {
                perms = new List<Permission>();
            }
            _userPermissions = new HashSet<Permission>(perms);
        }

        public bool HasPermission(Permission permission)
        {
            bool granted = _userPermissions.Contains(permission);
            AuditLogger.LogPermissionCheck(permission, granted);
            return granted;
        }

        public void CheckPermission(Permission permission)
        {
            if (!HasPermission(permission))
            {
                throw new UnauthorizedAccessException($"User does not have {permission} permission");
            }
        }
    }
}
