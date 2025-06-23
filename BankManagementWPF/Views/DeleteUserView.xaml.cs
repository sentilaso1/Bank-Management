using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class DeleteUserView : UserControl
    {
        private User _user;
        public DeleteUserView()
        {
            InitializeComponent();
            CurrentUserSession.CheckPermission(Permission.DeleteUser);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchUsernameTextBox.Text))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            _user = User.FindUserByUsername(SearchUsernameTextBox.Text);
            if (_user == null)
            {
                MessageBox.Show("User not found.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserIdTextBlock.Text = _user.UserID.ToString();
            UsernameTextBlock.Text = _user.Username;
            FullNameTextBlock.Text = _user.FirstName + " " + _user.LastName;

            UserInfoGroup.IsEnabled = true;
            DeleteButton.IsEnabled = true;
            SearchUsernameTextBox.IsEnabled = false;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentUserSession.CheckPermission(Permission.DeleteUser);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You don't have permission to delete users.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_user == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete this user?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if (User.DeleteUser(_user.Username))
            {
                MessageBox.Show("User deleted successfully", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Failed to delete user", "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SearchUsernameTextBox.Clear();
            SearchUsernameTextBox.IsEnabled = true;
            UserInfoGroup.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            UserIdTextBlock.Text = string.Empty;
            UsernameTextBlock.Text = string.Empty;
            FullNameTextBlock.Text = string.Empty;
            _user = null;
        }
    }
}
