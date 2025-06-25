using System.Windows;
using System.Windows.Controls;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
            ApplyPermissions();
            LoadUsersList();
        }

        private void ApplyPermissions()
        {
            AddNewUserButton.Visibility = CurrentUserSession.HasPermission(Permission.AddUser)
                ? Visibility.Visible : Visibility.Collapsed;
            UpdateUserButton.Visibility = CurrentUserSession.HasPermission(Permission.UpdateUser)
                ? Visibility.Visible : Visibility.Collapsed;
            DeleteUserButton.Visibility = CurrentUserSession.HasPermission(Permission.DeleteUser)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new AddNewUserView();
        }

        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new UpdateUserView();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new DeleteUserView();
        }

        private void FindUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new FindUserView();
        }

        private void UsersList_Click(object sender, RoutedEventArgs e)
        {
            LoadUsersList();
        }


        private void UserAnalytics_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new TextBlock { Text = "User analytics view not implemented." };
        }

        private void SecurityLogs_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new TextBlock { Text = "Security logs view not implemented." };
        }

        private void LoadUsersList()
        {
            UserContentArea.Content = new UsersListView();
        }
    }
}
