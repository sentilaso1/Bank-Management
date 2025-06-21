using System.Windows;
using BankManagementSystem.WPF.Views;

namespace BankManagementSystem.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new HomeView();
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
        }

        private void btnTransactions_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnLoginRegisters_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Global.CurrentUser = null;
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
