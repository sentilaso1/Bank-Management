using System;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class UpdateUserView : UserControl
    {
        private User _user;

        public UpdateUserView()
        {
            InitializeComponent();
            CurrentUserSession.CheckPermission(Permission.UpdateUser);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchErrorTextBlock.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(SearchUsernameTextBox.Text))
            {
                SearchErrorTextBlock.Text = "Please enter a username.";
                SearchErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            _user = User.FindUserByUsername(SearchUsernameTextBox.Text);
            if (_user == null)
            {
                SearchErrorTextBlock.Text = "User not found.";
                SearchErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            UserIdTextBox.Text = _user.UserID.ToString();
            UsernameTextBox.Text = _user.Username;
            FirstNameTextBox.Text = _user.FirstName;
            LastNameTextBox.Text = _user.LastName;
            EmailTextBox.Text = _user.Email;
            PhoneTextBox.Text = _user.PhoneNumber;
            PermissionComboBox.SelectedIndex = RoleMapping.GetRoleId(_user.Role) - 1;

            FormGrid.IsEnabled = true;
            UpdateButton.IsEnabled = true;
            SearchUsernameTextBox.IsEnabled = false;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentUserSession.CheckPermission(Permission.UpdateUser);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You don't have permission to update users.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_user == null)
                return;
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || UsernameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Username is too short. Please enter a valid username.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || FirstNameTextBox.Text.Length < 3)
            {
                MessageBox.Show("First Name is too short. Please enter a valid first name.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || LastNameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Last Name is too short. Please enter a valid last name.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(EmailTextBox.Text.Length < 8 || !EmailTextBox.Text.Contains("@") || !EmailTextBox.Text.Contains("."))
            {
                MessageBox.Show("Email is invalid. Please enter a valid email address.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || PhoneTextBox.Text.Length != 10)
            {
                MessageBox.Show("Phone Number is invalid. Please enter a valid phone number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _user.Username = UsernameTextBox.Text;
            _user.FirstName = FirstNameTextBox.Text;
            _user.LastName = LastNameTextBox.Text;
            _user.Email = EmailTextBox.Text;
            _user.PhoneNumber = PhoneTextBox.Text;
            if (PermissionComboBox.SelectedItem is ComboBoxItem cbItem)
                _user.Role = cbItem.Content.ToString();

            if (MessageBox.Show("Save changes to this user?", "Update", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            if (_user.Save())
            {
                Client a = Client.Find(_user.accountNumber);
                a.FirstName = FirstNameTextBox.Text;
                a.LastName = LastNameTextBox.Text;
                a.Email = EmailTextBox.Text;
                a.PhoneNumber = PhoneTextBox.Text;
                a.Save();
                MessageBox.Show("User updated successfully", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to update user", "Update", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SearchUsernameTextBox.Clear();
            SearchUsernameTextBox.IsEnabled = true;
            FormGrid.IsEnabled = false;
            UpdateButton.IsEnabled = false;
            UserIdTextBox.Clear();
            UsernameTextBox.Clear();
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            EmailTextBox.Clear();
            PhoneTextBox.Clear();
            PermissionComboBox.SelectedIndex = -1;
            SearchErrorTextBlock.Visibility = Visibility.Collapsed;
            _user = null;
        }
    }
}
