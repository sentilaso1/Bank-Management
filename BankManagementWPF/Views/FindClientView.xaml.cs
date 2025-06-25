using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class FindClientView : UserControl
    {
        public FindClientView()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                MessageBox.Show("Please enter search text.");
                return;
            }

            DataTable dt = Client.GetAllClients();
            var rows = dt.AsEnumerable();

            if (SearchByAccountRadio.IsChecked == true)
            {
                rows = rows.Where(r => r.Field<string>("AccountNumber")
                    .Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            else if (SearchByNameRadio.IsChecked == true)
            {
                rows = rows.Where(r => r.Field<string>("FirstName")
                        .Contains(search, StringComparison.OrdinalIgnoreCase)
                    || r.Field<string>("LastName")
                        .Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            else if (SearchByPhoneRadio.IsChecked == true)
            {
                rows = rows.Where(r => r.Field<string>("Phone")
                    .Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var result = rows.Select(r => new
            {
                AccountNumber = r.Field<string>("AccountNumber"),
                FullName = r.Field<string>("FirstName") + " " + r.Field<string>("LastName"),
                Email = r.Field<string>("Email"),
                PhoneNumber = r.Field<string>("Phone"),
                Balance = r.Field<decimal>("Balance")
            }).ToList();

            SearchResultsDataGrid.ItemsSource = result;

            if (result.Count == 0)
            {
                MessageBox.Show("No matching clients found.", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
