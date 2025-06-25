using System.Collections.Generic;

namespace BankBusinessLayer
{
    public static class RoleMapping
    {
        public static readonly Dictionary<int, string> RoleIdToName = new Dictionary<int, string>
        {
            { 1, "Administrator" },
            { 2, "Manager" },
            { 3, "User" },
            { 4, "Viewer" }
        };

        public static readonly Dictionary<string, int> RoleNameToId = new Dictionary<string, int>
        {
            { "Administrator", 1 },
            { "Manager", 2 },
            { "User", 3 },
            { "Viewer", 4 }
        };

        public static string GetRoleName(int roleId)
        {
            return RoleIdToName.ContainsKey(roleId) ? RoleIdToName[roleId] : "Viewer";
        }

        public static int GetRoleId(string roleName)
        {
            return RoleNameToId.ContainsKey(roleName) ? RoleNameToId[roleName] : 4;
        }
    }
}
