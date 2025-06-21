using System.Windows;
using Bank_Management_System.Forms.Users;
using Bank_Management_System.Forms.Clients;
using Bank_Management_System.Forms.Transactions;
using Bank_Management_System.Forms.LoginRegisters;
using Bank_Management_System.Forms.Home;

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
            // Placeholder for Clients view conversion
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
