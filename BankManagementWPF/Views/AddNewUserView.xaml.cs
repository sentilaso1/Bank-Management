using BankBusinessLayer;
using BankDataAccessLayer;
using BankManagementSystem.WPF.Security;
using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem.WPF.Views
{
    public partial class AddNewUserView : UserControl
    {
        private User _user = new User();

        public AddNewUserView()
        {
            InitializeComponent();
            CurrentUserSession.CheckPermission(Permission.AddUser);
        }

        private bool FillData()
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Username is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (User.IsUserExists(UsernameTextBox.Text))
            {
                MessageBox.Show("Username already exists", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Passwords do not match", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (PhoneTextBox.Text.Length != 10)
            {
                MessageBox.Show("Phone number must be 10 digits", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("First name and Last name are required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !EmailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("Valid email is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            _user.Username = UsernameTextBox.Text;
            _user.FirstName = FirstNameTextBox.Text;
            _user.LastName = LastNameTextBox.Text;
            _user.Email = EmailTextBox.Text;
            _user.PhoneNumber = PhoneTextBox.Text;
            _user.Password = PasswordBox.Password;
            _user.Role = GetSelectedRole();
            return true;
        }

        private string GetSelectedRole()
        {
            if (AdminRoleRadio.IsChecked == true)
                return "Administrator";
            if (ManagerRoleRadio.IsChecked == true)
                return "Manager";
            if (UserRoleRadio.IsChecked == true)
                return "User";
            return "Viewer";
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentUserSession.CheckPermission(Permission.AddUser);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You don't have permission to add users.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!FillData())
                return;

            if (MessageBox.Show("Create this user?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (_user.Save())
            {
                if (_user.Role == "User")
                {
                    Client client = new Client
                    {
                        FirstName = _user.FirstName,
                        LastName = _user.LastName,
                        Email = _user.Email,
                        PhoneNumber = _user.PhoneNumber,
                        AccountNumber = _user.accountNumber
                    };
                    Random random = new Random();
                    client.Balance = 0;
                    string accountNumber = GenerateUniqueAccountNumber();
                    client.AccountNumber = accountNumber;
                    client.PinCode = random.Next(1000, 9999).ToString();
                    client.Save();
                }
                MessageBox.Show("User added successfully", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Failed to add user", "Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GenerateUniqueAccountNumber()
        {
            Random random = new Random();
            string accountNumber;
            do
            {
                accountNumber = "ACC" + random.Next(0, 999).ToString();
            } while (ClientsData.isClientExist(accountNumber));
            return accountNumber;
        }
        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Clear();
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            EmailTextBox.Clear();
            PhoneTextBox.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm_Click(sender, e);
        }
    }
}
