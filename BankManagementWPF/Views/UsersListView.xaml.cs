using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class UsersListView : UserControl
    {
        private List<UserRow> _rows = new();

        public UsersListView()
        {
            InitializeComponent();
            LoadData();
        }

        private class UserRow
        {
            public bool IsSelected { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }

        private void LoadData()
        {
            DataTable dt = User.GetAllUsers();
            _rows = dt.AsEnumerable().Select(r => new UserRow
            {
                Username = r.Field<string>("Username"),
                FullName = r.Field<string>("FirstName") + " " + r.Field<string>("LastName"),
                Email = r.Field<string>("Email"),
                Role = PermissionToRole(r.Field<int>("Permission"))
            }).ToList();
            UsersDataGrid.ItemsSource = _rows;
            UpdateStats();
        }

        private static string PermissionToRole(int p) => p switch
        {
            1 => "Administrator",
            2 => "Manager",
            3 => "Cashier",
            _ => "Viewer"
        };

        private void UpdateStats()
        {
            AdminCountTextBlock.Text = _rows.Count(r => r.Role == "Administrator").ToString();
            ManagerCountTextBlock.Text = _rows.Count(r => r.Role == "Manager").ToString();
            CashierCountTextBlock.Text = _rows.Count(r => r.Role == "Cashier").ToString();
            ViewerCountTextBlock.Text = _rows.Count(r => r.Role == "Viewer").ToString();
            LockedCountTextBlock.Text = "0"; // no status info
        }

        private void RoleFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search users...")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = SystemColors.ControlTextBrush;
            }
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search users...";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ApplyFilters()
        {
            IEnumerable<UserRow> query = _rows;
            if (RoleFilterComboBox.SelectedIndex > 0)
            {
                string role = ((ComboBoxItem)RoleFilterComboBox.SelectedItem).Content.ToString();
                query = query.Where(r => r.Role == role);
            }
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Search users...")
            {
                query = query.Where(r => r.Username.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase)
                                       || r.FullName.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase)
                                       || r.Email.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase));
            }
            UsersDataGrid.ItemsSource = query.ToList();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add user - not implemented", "Add", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new() { Filter = "CSV|*.csv" };
            if (dlg.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Username,FullName,Email,Role");
                foreach (UserRow row in UsersDataGrid.ItemsSource as IEnumerable<UserRow>)
                    sb.AppendLine($"{row.Username},{row.FullName},{row.Email},{row.Role}");
                File.WriteAllText(dlg.FileName, sb.ToString());
                MessageBox.Show("Export completed", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BulkLock_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bulk lock - not implemented", "Lock", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BulkUnlock_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bulk unlock - not implemented", "Unlock", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Analytics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Analytics view - not implemented", "Analytics", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
