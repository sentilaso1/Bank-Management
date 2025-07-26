using BankManagementSystem.WPF.Security;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using System.Linq;

namespace BankManagementSystem.WPF.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Loaded += HomeView_Loaded;
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Only show Delete Account button if current user is a 'User' (not Admin/Manager/Viewer)
            var user = CurrentUserSession.CurrentUser;
            if (user == null || user.Role != "User")
            {
                var btn = this.FindName("DeleteAccountButton") as Button;
                if (btn != null)
                    btn.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var user = CurrentUserSession.CurrentUser;
            if (user == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.", "Delete Account", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if (User.LockUser(user.Username))
            {
                MessageBox.Show("Your account has been deleted.", "Account Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                Global.CurrentUser = null;
                CurrentUserSession.Logout();

                // Đóng MainWindow và show lại LoginWindow
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    mainWindow.Close();
                }
            }
            else
            {
                MessageBox.Show("Failed to deactivate your account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
