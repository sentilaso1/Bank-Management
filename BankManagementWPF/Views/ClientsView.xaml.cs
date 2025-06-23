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
            AddNewClientButton.Visibility = CurrentUserSession.HasPermission(Permission.AddClient)
                ? Visibility.Visible : Visibility.Collapsed;
            UpdateClientButton.Visibility = CurrentUserSession.HasPermission(Permission.UpdateClient)
                ? Visibility.Visible : Visibility.Collapsed;
            DeleteClientButton.Visibility = CurrentUserSession.HasPermission(Permission.DeleteClient)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddNewClient_Click(object sender, RoutedEventArgs e)
        {
            ClientContentArea.Content = new AddNewClientView();
        }

        private void UpdateClient_Click(object sender, RoutedEventArgs e)
        {
            ClientContentArea.Content = new UpdateClientView();
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            ClientContentArea.Content = new DeleteClientView();
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
