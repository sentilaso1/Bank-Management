using System.Windows;
using System.Windows.Controls;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class ClientsView : UserControl
    {
        public ClientsView()
        {
            InitializeComponent();
            ApplyPermissions();
            LoadClientsListView();
        }

        private void ApplyPermissions()
        {
            UpdateClientButton.Visibility = CurrentUserSession.HasPermission(Permission.UpdateClient)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateClient_Click(object sender, RoutedEventArgs e)
        {
            ClientContentArea.Content = new UpdateClientView();
        }

        private void FindClient_Click(object sender, RoutedEventArgs e)
        {
            ClientContentArea.Content = new FindClientView();
        }

        private void ClientsList_Click(object sender, RoutedEventArgs e)
        {
            LoadClientsListView();
        }

        private void LoadClientsListView()
        {
            ClientContentArea.Content = new ClientsListView();
        }
    }
}
