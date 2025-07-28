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

            btnUsers.IsEnabled = permissionService.HasPermission(Permission.ViewUsers);
            btnClients.IsEnabled = permissionService.HasPermission(Permission.ViewClients);
            btnTransactions.IsEnabled = permissionService.HasPermission(Permission.ViewTransactions);
            btnLoginRegisters.IsEnabled = permissionService.HasPermission(Permission.ViewAuditLogs);
            btnGoal.IsEnabled = CurrentUserSession.CurrentUser.Role == "User" ? true : false;
            btnRevenue.IsEnabled = CurrentUserSession.CurrentUser.Role == "Manager" ? true : false;
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

        private void btnRevenue_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new RevenueView();
        }

        private void btnGoal_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GoalSettingControl();
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
