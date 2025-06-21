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
            // TODO: load UpdateClientView when implemented
            MessageBox.Show("UpdateClientView not implemented yet.");
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            // TODO: load DeleteClientView when implemented
            MessageBox.Show("DeleteClientView not implemented yet.");
        }

        private void FindClient_Click(object sender, RoutedEventArgs e)
        {
            // TODO: load FindClientView when implemented
            MessageBox.Show("FindClientView not implemented yet.");
        }

        private void ClientsList_Click(object sender, RoutedEventArgs e)
        {
            LoadClientsListView();
        }

        private void LoadClientsListView()
        {
            // TODO: replace with ClientsListView when implemented
            ClientContentArea.Content = new TextBlock { Text = "Clients list not implemented yet." };
        }
    }
}
