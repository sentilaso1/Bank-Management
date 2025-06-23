using System.Windows;
using BankBusinessLayer;
using BankManagementSystem.WPF.Views;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadNavigationBasedOnRole();
            MainContent.Content = new HomeView();
        }

        private void LoadNavigationBasedOnRole()
        {
            if (CurrentUserSession.CurrentUser == null)
                return;

            var permissionService = CurrentUserSession.PermissionService;

            btnUsers.Visibility = permissionService.HasPermission(Permission.ViewUsers)
                ? Visibility.Visible : Visibility.Collapsed;
            btnClients.Visibility = permissionService.HasPermission(Permission.ViewClients)
                ? Visibility.Visible : Visibility.Collapsed;
            btnTransactions.Visibility = permissionService.HasPermission(Permission.ViewTransactions)
                ? Visibility.Visible : Visibility.Collapsed;
            btnLoginRegisters.Visibility = permissionService.HasPermission(Permission.ViewAuditLogs)
                ? Visibility.Visible : Visibility.Collapsed;
        }


        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new HomeView();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ClientsView();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UsersView();
        }

        private void btnTransactions_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TransactionsView();
        }

        private void btnLoginRegisters_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LoginRegistersView();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentUser = null;
            CurrentUserSession.Logout();
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
