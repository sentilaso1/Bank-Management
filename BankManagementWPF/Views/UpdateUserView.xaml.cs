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
