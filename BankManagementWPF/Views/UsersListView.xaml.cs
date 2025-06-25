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
            // Sử dụng Loaded event thay vì gọi LoadData() trực tiếp
            this.Loaded += UsersListView_Loaded;
        }

        private void UsersListView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            this.Loaded -= UsersListView_Loaded; // Unsubscribe để tránh gọi nhiều lần
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
            try
            {
                DataTable dt = User.GetAllUsers();
                _rows = dt.AsEnumerable().Select(r => new UserRow
                {
                    Username = r.Field<string>("Username") ?? string.Empty,
                    FullName = (r.Field<string>("FirstName") ?? "") + " " + (r.Field<string>("LastName") ?? ""),
                    Email = r.Field<string>("Email") ?? string.Empty,
                    Role = r.Field<string>("Role") ?? PermissionToRole(r.Field<int?>("Permission") ?? 0)
                }).ToList();

                if (UsersDataGrid != null)
                {
                    UsersDataGrid.ItemsSource = _rows;
                }

                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string PermissionToRole(int p) => p switch
        {
            1 => "Administrator",
            2 => "Manager",
            3 => "User",
            _ => "Viewer"
        };

        private void UpdateStats()
        {
            // Thêm null check cho tất cả TextBlock
            if (AdminCountTextBlock != null)
                AdminCountTextBlock.Text = _rows.Count(r => r.Role == "Administrator").ToString();

            if (ManagerCountTextBlock != null)
                ManagerCountTextBlock.Text = _rows.Count(r => r.Role == "Manager").ToString();

            if (UserCountTextBlock != null)
                UserCountTextBlock.Text = _rows.Count(r => r.Role == "User").ToString();

            if (ViewerCountTextBlock != null)
                ViewerCountTextBlock.Text = _rows.Count(r => r.Role == "Viewer").ToString();

            if (LockedCountTextBlock != null)
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
            if (SearchTextBox != null && SearchTextBox.Text == "Search users...")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = SystemColors.ControlTextBrush;
            }
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox != null && string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search users...";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ApplyFilters()
        {
            // Kiểm tra null cho tất cả controls trước khi sử dụng
            if (RoleFilterComboBox == null || SearchTextBox == null || UsersDataGrid == null)
            {
                return; // Thoát sớm nếu controls chưa sẵn sàng
            }

            try
            {
                IEnumerable<UserRow> query = _rows;

                if (RoleFilterComboBox.SelectedIndex > 0 && RoleFilterComboBox.SelectedItem != null)
                {
                    string role = ((ComboBoxItem)RoleFilterComboBox.SelectedItem).Content?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(role))
                    {
                        query = query.Where(r => r.Role == role);
                    }
                }

                // Dòng code đã sửa với null check
                if (SearchTextBox != null && !string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Search users...")
                {
                    string searchText = SearchTextBox.Text;
                    query = query.Where(r =>
                        (r.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (r.FullName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (r.Email?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                UsersDataGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            try
            {
                if (UsersDataGrid?.ItemsSource == null)
                {
                    MessageBox.Show("No data to export", "Export", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Microsoft.Win32.SaveFileDialog dlg = new() { Filter = "CSV|*.csv" };
                if (dlg.ShowDialog() == true)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Username,FullName,Email,Role");

                    foreach (UserRow row in UsersDataGrid.ItemsSource as IEnumerable<UserRow> ?? Enumerable.Empty<UserRow>())
                    {
                        sb.AppendLine($"{row.Username},{row.FullName},{row.Email},{row.Role}");
                    }

                    File.WriteAllText(dlg.FileName, sb.ToString());
                    MessageBox.Show("Export completed", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
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
