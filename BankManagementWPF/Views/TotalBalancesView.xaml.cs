using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class TotalBalancesView : UserControl
    {
        private DataTable _balancesTable;
        private DataTable _logsTable;

        public TotalBalancesView()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            _balancesTable = Client.GetAllBalances();
            _logsTable = TransferLogs.GetAllTransferLogs();
            AccountBalancesDataGrid.ItemsSource = _balancesTable.DefaultView;
            UpdateMetrics();
        }

        private void UpdateMetrics()
        {
            if (_balancesTable == null || _logsTable == null)
                return;

            int totalClients = Client.GetTotalClients();
            decimal totalBalance = Client.GetTotalBalances();
            TotalClientsTextBlock.Text = totalClients.ToString();
            BankTotalTextBlock.Text = totalBalance.ToString("C", CultureInfo.CurrentCulture);

            var deposits = _logsTable.AsEnumerable().Where(r => string.IsNullOrEmpty(r["FromAccountNumber"].ToString()));
            var withdrawals = _logsTable.AsEnumerable().Where(r => string.IsNullOrEmpty(r["ToAccountNumber"].ToString()));
            var transfers = _logsTable.AsEnumerable().Where(r => !string.IsNullOrEmpty(r["FromAccountNumber"].ToString()) && !string.IsNullOrEmpty(r["ToAccountNumber"].ToString()));

            decimal depSum = deposits.Sum(r => r.Field<decimal>("Amount"));
            decimal withSum = withdrawals.Sum(r => r.Field<decimal>("Amount"));
            decimal transSum = transfers.Sum(r => r.Field<decimal>("Amount"));

            TotalDepositsTextBlock.Text = depSum.ToString("C", CultureInfo.CurrentCulture);
            DepositsCountTextBlock.Text = deposits.Count().ToString();
            TotalWithdrawalsTextBlock.Text = withSum.ToString("C", CultureInfo.CurrentCulture);
            WithdrawalsCountTextBlock.Text = withdrawals.Count().ToString();
            TotalTransfersTextBlock.Text = transSum.ToString("C", CultureInfo.CurrentCulture);
            TransfersCountTextBlock.Text = transfers.Count().ToString();
        }

        private void BalanceFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void SearchAccount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchAccountTextBox.Foreground == System.Windows.Media.Brushes.Gray)
            {
                SearchAccountTextBox.Text = string.Empty;
                SearchAccountTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchAccount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchAccountTextBox.Text))
            {
                SearchAccountTextBox.Text = "Search by account or client name...";
                SearchAccountTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ApplyFilter()
        {
            if (_balancesTable == null)
                return;

            DataView view = _balancesTable.DefaultView;
            string filter = string.Empty;
            switch (BalanceFilterComboBox.SelectedIndex)
            {
                case 1:
                    filter = "Balance > 50000";
                    break;
                case 2:
                    filter = "Balance >= 10000 AND Balance <= 50000";
                    break;
                case 3:
                    filter = "Balance < 10000";
                    break;
                case 4:
                    filter = "Balance = 0";
                    break;
            }
            if (SearchAccountTextBox.Foreground != System.Windows.Media.Brushes.Gray && !string.IsNullOrWhiteSpace(SearchAccountTextBox.Text))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += $"(AccountNumber LIKE '%{SearchAccountTextBox.Text}%' OR FirstName LIKE '%{SearchAccountTextBox.Text}%' OR LastName LIKE '%{SearchAccountTextBox.Text}%')";
            }
            view.RowFilter = filter;
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            DataView view = AccountBalancesDataGrid.ItemsSource as DataView;
            if (view == null || view.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "BalancesReport.csv"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    using StreamWriter writer = new StreamWriter(dlg.FileName, false, Encoding.UTF8);
                    var cols = view.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
                    writer.WriteLine(string.Join(",", cols));
                    foreach (DataRowView rowView in view)
                    {
                        string line = string.Join(",", rowView.Row.ItemArray.Select(v => v.ToString()));
                        writer.WriteLine(line);
                    }
                    MessageBox.Show("Export completed.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewCharts_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chart view is not implemented.", "Charts", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterAccounts_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
    }
}
