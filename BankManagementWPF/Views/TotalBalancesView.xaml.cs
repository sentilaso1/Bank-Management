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
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

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
                filter += $"(AccountNumber LIKE '%{SearchAccountTextBox.Text}%' OR ClientFullName LIKE '%{SearchAccountTextBox.Text}%')";
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
            if (_balancesTable == null || _logsTable == null)
            {
                MessageBox.Show("No data available to display charts.", "Charts", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a new window to display charts
            Window chartWindow = new Window
            {
                Title = "Bank Financial Charts",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // Create a grid to hold charts
            Grid chartGrid = new Grid
            {
                Margin = new Thickness(20)
            };
            chartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            chartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Bar Chart for Deposits, Withdrawals, and Transfers
            var deposits = _logsTable.AsEnumerable().Where(r => string.IsNullOrEmpty(r["FromAccountNumber"].ToString()));
            var withdrawals = _logsTable.AsEnumerable().Where(r => string.IsNullOrEmpty(r["ToAccountNumber"].ToString()));
            var transfers = _logsTable.AsEnumerable().Where(r => !string.IsNullOrEmpty(r["FromAccountNumber"].ToString()) && !string.IsNullOrEmpty(r["ToAccountNumber"].ToString()));

            decimal depSum = deposits.Sum(r => r.Field<decimal>("Amount"));
            decimal withSum = withdrawals.Sum(r => r.Field<decimal>("Amount"));
            decimal transSum = transfers.Sum(r => r.Field<decimal>("Amount"));

            CartesianChart barChart = new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Deposits",
                        Values = new ChartValues<decimal> { depSum },
                        Fill = Brushes.Green
                    },
                    new ColumnSeries
                    {
                        Title = "Withdrawals",
                        Values = new ChartValues<decimal> { withSum },
                        Fill = Brushes.Red
                    },
                    new ColumnSeries
                    {
                        Title = "Transfers",
                        Values = new ChartValues<decimal> { transSum },
                        Fill = Brushes.Blue
                    }
                },
                AxisX = new AxesCollection
                {
                    new Axis { Title = "Transaction Type", Labels = new[] { "Deposits", "Withdrawals", "Transfers" } }
                },
                AxisY = new AxesCollection
                {
                    new Axis { Title = "Amount", LabelFormatter = value => value.ToString("C", CultureInfo.CurrentCulture) }
                }
            };
            Grid.SetRow(barChart, 0);

            // Pie Chart for Balance Distribution
            int highBalance = _balancesTable.AsEnumerable().Count(r => r.Field<decimal>("Balance") > 50000);
            int mediumBalance = _balancesTable.AsEnumerable().Count(r => r.Field<decimal>("Balance") >= 10000 && r.Field<decimal>("Balance") <= 50000);
            int lowBalance = _balancesTable.AsEnumerable().Count(r => r.Field<decimal>("Balance") < 10000 && r.Field<decimal>("Balance") > 0);
            int zeroBalance = _balancesTable.AsEnumerable().Count(r => r.Field<decimal>("Balance") == 0);

            PieChart pieChart = new PieChart
            {
                Series = new SeriesCollection
                {
                    new PieSeries { Title = "High Balance (>50,000)", Values = new ChartValues<int> { highBalance }, DataLabels = true, Fill = Brushes.Purple },
                    new PieSeries { Title = "Medium Balance (10,000-50,000)", Values = new ChartValues<int> { mediumBalance }, DataLabels = true, Fill = Brushes.Orange },
                    new PieSeries { Title = "Low Balance (<10,000)", Values = new ChartValues<int> { lowBalance }, DataLabels = true, Fill = Brushes.Yellow },
                    new PieSeries { Title = "Zero Balance", Values = new ChartValues<int> { zeroBalance }, DataLabels = true, Fill = Brushes.Gray }
                },
                LegendLocation = LegendLocation.Right
            };
            Grid.SetRow(pieChart, 1);

            chartGrid.Children.Add(barChart);
            chartGrid.Children.Add(pieChart);
            chartWindow.Content = chartGrid;
            chartWindow.ShowDialog();
        }

        private void FilterAccounts_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
    }
}