using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem.WPF.Views
{
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
            LoadUsersList();
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new AddNewUserView();
        }

        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new TextBlock { Text = "Update user view not implemented." };
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new TextBlock { Text = "Delete user view not implemented." };
        }

        private void FindUser_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new TextBlock { Text = "Find user view not implemented." };
        }

        private void UsersList_Click(object sender, RoutedEventArgs e)
        {
            LoadUsersList();
        }

        private void Permissions_Click(object sender, RoutedEventArgs e)
        {
            UserContentArea.Content = new TextBlock { Text = "Permissions view not implemented." };
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
            UserContentArea.Content = new TextBlock { Text = "Users list view not implemented." };
        }
    }
}
