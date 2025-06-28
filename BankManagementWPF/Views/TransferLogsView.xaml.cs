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
    public partial class TransferLogsView : UserControl
    {
        private DataTable _logsTable;

        public TransferLogsView()
        {
            InitializeComponent();
            LoadTransferLogs();
        }

        private void LoadTransferLogs()
        {
            _logsTable = TransferLogs.GetAllTransferLogs();
            TransferLogsDataGrid.ItemsSource = _logsTable.DefaultView;
            UpdateQuickStats();
            UpdateSummary(_logsTable);            
        }

        private void UpdateQuickStats()
        {
            if (_logsTable == null)
                return;

            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
            DateTime monthStart = new DateTime(today.Year, today.Month, 1);

            int todayCount = _logsTable.AsEnumerable()
                .Count(r => r.Field<DateTime>("TransferDate").Date == today);
            int weekCount = _logsTable.AsEnumerable()
                .Count(r => r.Field<DateTime>("TransferDate") >= weekStart);
            int monthCount = _logsTable.AsEnumerable()
                .Count(r => r.Field<DateTime>("TransferDate") >= monthStart);
            decimal totalVolume = _logsTable.AsEnumerable()
                .Sum(r => r.Field<decimal>("Amount"));

            TodayTransfersTextBlock.Text = todayCount.ToString();
            WeekTransfersTextBlock.Text = weekCount.ToString();
            MonthTransfersTextBlock.Text = monthCount.ToString();
            TotalVolumeTextBlock.Text = totalVolume.ToString("C", CultureInfo.CurrentCulture);
        }

        private void UpdateSummary(DataTable table)
        {
            if (table == null)
                return;
            TotalRecordsTextBlock.Text = table.Rows.Count.ToString();
            decimal total = table.AsEnumerable().Sum(r => r.Field<decimal>("Amount"));
            TotalAmountTextBlock.Text = total.ToString("C", CultureInfo.CurrentCulture);
        }

        private DataTable ApplyFilters()
        {
            if (_logsTable == null)
                return null;
            var rows = _logsTable.AsEnumerable();
            if (FromDatePicker.SelectedDate.HasValue)
                rows = rows.Where(r => r.Field<DateTime>("TransferDate") >= FromDatePicker.SelectedDate.Value);
            if (ToDatePicker.SelectedDate.HasValue)
                rows = rows.Where(r => r.Field<DateTime>("TransferDate") <= ToDatePicker.SelectedDate.Value);
            if (!string.IsNullOrWhiteSpace(FromAccountFilterTextBox.Text))
                rows = rows.Where(r => r.Field<string>("FromAccountNumber")?.Contains(FromAccountFilterTextBox.Text) == true);
            if (!string.IsNullOrWhiteSpace(ToAccountFilterTextBox.Text))
                rows = rows.Where(r => r.Field<string>("ToAccountNumber")?.Contains(ToAccountFilterTextBox.Text) == true);
            if (decimal.TryParse(MinAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal min))
                rows = rows.Where(r => r.Field<decimal>("Amount") >= min);
            if (decimal.TryParse(MaxAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal max))
                rows = rows.Where(r => r.Field<decimal>("Amount") <= max);

            DataTable filtered = rows.Any() ? rows.CopyToDataTable() : _logsTable.Clone();
            TransferLogsDataGrid.ItemsSource = filtered.DefaultView;
            UpdateSummary(filtered);
            return filtered;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            FromAccountFilterTextBox.Clear();
            ToAccountFilterTextBox.Clear();
            MinAmountTextBox.Clear();
            MaxAmountTextBox.Clear();
            TransferLogsDataGrid.ItemsSource = _logsTable.DefaultView;
            UpdateSummary(_logsTable);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            DataView view = TransferLogsDataGrid.ItemsSource as DataView;
            if (view == null || view.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "TransferLogs.csv"
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
    }
}
