using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class AddNewUserView : UserControl
    {
        private User _user = new User();

        public AddNewUserView()
        {
            InitializeComponent();
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

            _user.Username = UsernameTextBox.Text;
            _user.FirstName = FirstNameTextBox.Text;
            _user.LastName = LastNameTextBox.Text;
            _user.Email = EmailTextBox.Text;
            _user.PhoneNumber = PhoneTextBox.Text;
            _user.Password = PasswordBox.Password;
            _user.Permission = GetSelectedPermission();
            return true;
        }

        private int GetSelectedPermission()
        {
            if (AdminRoleRadio.IsChecked == true)
                return 1;
            if (ManagerRoleRadio.IsChecked == true)
                return 2;
            if (CashierRoleRadio.IsChecked == true)
                return 3;
            return 4;
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            if (!FillData())
                return;

            if (MessageBox.Show("Create this user?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (_user.Save())
            {
                MessageBox.Show("User added successfully", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Failed to add user", "Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            UserIdTextBox.Clear();
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
