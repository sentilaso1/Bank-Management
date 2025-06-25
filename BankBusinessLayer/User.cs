using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BankDataAccessLayer;

namespace BankBusinessLayer
{
    public class User : Person
    {
        public enum enMode { AddNew, Update }
        public enMode Mode;
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        private int _roleId;
        public int RoleId => _roleId;
        public string Role { get; set; }
        public bool IsActive { get; set; }

        private User(int userID, string FirstName, string LastName, string Email, string Phone,
            string username, string password, string role, int permission, bool isActive = true) :
            base(userID, FirstName, LastName, Email, Phone)
        {
            Username = username;
            Password = password;
            _roleId = permission;
            Role = role;
            IsActive = isActive;
            Mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            int roleId = RoleMapping.GetRoleId(this.Role);
            this.UserID = UsersData.AddNewUser(this.Username, this.FirstName, this.LastName,
                this.Email, this.PhoneNumber, this.Password, this.Role, roleId);

            return (UserID != -1);
        }

        private bool _UpdateUser()
        {
            int roleId = RoleMapping.GetRoleId(this.Role);
            return UsersData.UpdateUser(this.Username, this.FirstName, this.LastName,
                this.Email, this.PhoneNumber, this.Password, this.Role, roleId);
        }

        public User() : base()
        {
            Username = string.Empty;
            Password = string.Empty;
            _roleId = 4;
            Role = RoleMapping.GetRoleName(_roleId);
            IsActive = true;

            Mode = enMode.AddNew;
        }

        static public User FindUserByUsername(string username)
        {
            int Permission = 0, UserID = 0;
            string FirstName = "", LastName = "", Email = "", Phone = "", Password = "", Role = "";

            bool isActive = true;
            if (UsersData.GetUserByUsername(username, ref FirstName, ref LastName, ref Email, ref Phone, ref Password, ref Role, ref Permission, ref UserID, ref isActive))
            {
                if (Role.Equals("Cashier", System.StringComparison.OrdinalIgnoreCase))
                    Role = "User";
                return new User(UserID, FirstName, LastName, Email, Phone, username, Password, Role, Permission, isActive);
            }
            else
                return null;


        }

        static public User FindUserByUsernameAndPassword(string username, string password)
        {
            int Permission = 0, UserID = 0;
            string FirstName = "", LastName = "", Email = "", Phone = "", Role = "";

            if (UsersData.GetUserByUsernameAndPassword(username, password, ref Role, ref Permission))
            {
                if (Role.Equals("Cashier", System.StringComparison.OrdinalIgnoreCase))
                    Role = "User";
                return new User(UserID, FirstName, LastName, Email, Phone, username, password, Role, Permission, true);
            }
            else
                return null;
        }

        static public DataTable GetAllUsers()
        {
            return UsersData.GetAllUsers();
        }


        static public bool IsUserExists(string Username)
        {
            return UsersData.isUserExist( Username);
        }

        static public bool DeleteUser(string Username)
        {
            return UsersData.DeleteUser( Username);
        }

        public bool Save()
        {

            switch (Mode)
            { 
                
                case enMode.AddNew:
                    if(_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                    case enMode.Update:

                         return _UpdateUser();


            }

            return false;


        }

        public static DataTable GetAllLoginRegisters()
        {
            return LoginRegistersData.GetAllLoginRegisters();
        }

        public static int GetTotalLoginRegisters()
        {
            return LoginRegistersData.GetTotalLoginRegisters();
        }
        public static int GetTotalUsers()
        {
            return UsersData.GetTotalUsers();
        }

        public static bool LockUser(string username)
        {
            return UsersData.SetUserActive(username, false);
        }

        public static bool UnlockUser(string username)
        {
            return UsersData.SetUserActive(username, true);
        }

        public static bool AddNewLoginRegisters(string Username, DateTime date)
        {
            return LoginRegistersData.AddNewLoginRegister(Username, date) != -1;
        }
    }
}
