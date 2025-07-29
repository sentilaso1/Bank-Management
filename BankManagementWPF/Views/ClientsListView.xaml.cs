using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class ClientsListView : UserControl
    {
        private DataTable _clientsTable;

        public ClientsListView()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            _clientsTable = Client.GetAllClients();
            ClientsDataGrid.ItemsSource = _clientsTable.DefaultView;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_clientsTable == null)
                return;

            var view = _clientsTable.AsEnumerable();
            switch (FilterComboBox.SelectedIndex)
            {
                case 1: // Active Accounts - balance > 0
                    view = view.Where(r => r.Field<decimal>("Balance") > 0);
                    break;
                case 2: // High Balance
                    view = view.Where(r => r.Field<decimal>("Balance") > 1000000);
                    break;
                default:
                    break;
            }
            DataTable filtered = view.Any() ? view.CopyToDataTable() : _clientsTable.Clone();
            ClientsDataGrid.ItemsSource = filtered.DefaultView;
        }
    }
}
