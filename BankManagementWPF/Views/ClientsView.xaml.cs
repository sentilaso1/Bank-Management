using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem.WPF.Views
{
    public partial class ClientsView : UserControl
    {
        public ClientsView()
        {
            InitializeComponent();
            LoadClientsListView();
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
