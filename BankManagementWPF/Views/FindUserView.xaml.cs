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
    public partial class FindUserView : UserControl
    {
        private List<UserRow> _rows = new();

        public FindUserView()
        {
            InitializeComponent();
            LoadAllUsers();
        }

        private class UserRow
        {
            public bool IsSelected { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }

        private void LoadAllUsers()
        {
            DataTable dt = User.GetAllUsers();
            _rows = dt.AsEnumerable().Select(r => new UserRow
            {
                Username = r.Field<string>("Username"),
                FullName = r.Field<string>("FirstName") + " " + r.Field<string>("LastName"),
                Email = r.Field<string>("Email"),
                Role = PermissionToRole(r.Field<int>("Permission")),
                IsActive = Convert.ToBoolean(r["IsActive"])
            }).ToList();
            SearchResultsDataGrid.ItemsSource = _rows;
            UpdateStats();
        }

        private static string PermissionToRole(int p) => p switch
        {
            1 => "Administrator",
            2 => "Manager",
            3 => "User",
            _ => "Viewer"
        };

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<UserRow> query = _rows;
            if (!string.IsNullOrWhiteSpace(UsernameSearchTextBox.Text))
                query = query.Where(r => r.Username.Contains(UsernameSearchTextBox.Text, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(FullNameSearchTextBox.Text))
                query = query.Where(r => r.FullName.Contains(FullNameSearchTextBox.Text, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(EmailSearchTextBox.Text))
                query = query.Where(r => r.Email.Contains(EmailSearchTextBox.Text, StringComparison.OrdinalIgnoreCase));
            if (RoleFilterComboBox.SelectedIndex > 0)
            {
                string role = ((ComboBoxItem)RoleFilterComboBox.SelectedItem).Content.ToString();
                query = query.Where(r => r.Role == role);
            }
            if (StatusFilterComboBox.SelectedIndex == 1)
                query = query.Where(r => r.IsActive);
            else if (StatusFilterComboBox.SelectedIndex == 2)
                query = query.Where(r => !r.IsActive);
            SearchResultsDataGrid.ItemsSource = query.ToList();
            UpdateStats();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            UserIdSearchTextBox.Clear();
            UsernameSearchTextBox.Clear();
            FullNameSearchTextBox.Clear();
            EmailSearchTextBox.Clear();
            RoleFilterComboBox.SelectedIndex = 0;
            StatusFilterComboBox.SelectedIndex = 0;
            CreatedFromDatePicker.SelectedDate = null;
            CreatedToDatePicker.SelectedDate = null;
            LoadAllUsers();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new() { Filter = "CSV|*.csv" };
            if (dlg.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Username,FullName,Email,Role");
                foreach (UserRow row in SearchResultsDataGrid.ItemsSource as IEnumerable<UserRow>)
                {
                    sb.AppendLine($"{row.Username},{row.FullName},{row.Email},{row.Role}");
                }
                File.WriteAllText(dlg.FileName, sb.ToString());
                MessageBox.Show("Export completed", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (UserRow row in SearchResultsDataGrid.ItemsSource as IEnumerable<UserRow>)
                row.IsSelected = true;
            SearchResultsDataGrid.Items.Refresh();
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (UserRow row in SearchResultsDataGrid.ItemsSource as IEnumerable<UserRow>)
                row.IsSelected = false;
            SearchResultsDataGrid.Items.Refresh();
        }

        private void LockSelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = (_rows.Where(r => r.IsSelected)).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("No users selected", "Lock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int success = 0;
            foreach (var row in selected)
            {
                if (row.Role.Equals("Administrator"))
                {
                    MessageBox.Show($"Cannot lock Administrator: {row.Username}", "Lock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    continue;
                }
                if (User.LockUser(row.Username))
                {
                    success++;
                }
                    
            }

            LoadAllUsers();
            MessageBox.Show($"Locked {success} user(s)", "Lock", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UnlockSelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = (_rows.Where(r => r.IsSelected)).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("No users selected", "Unlock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int success = 0;
            foreach (var row in selected)
            {
                if (User.UnlockUser(row.Username))
                    success++;
            }

            LoadAllUsers();
            MessageBox.Show($"Unlocked {success} user(s)", "Unlock", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateStats()
        {
            var list = SearchResultsDataGrid.ItemsSource as IEnumerable<UserRow> ?? new List<UserRow>();
            int total = list.Count();
            int active = list.Count(r => r.IsActive);
            int locked = list.Count(r => !r.IsActive);
            ResultsCountTextBlock.Text = total.ToString();
            TotalUsersTextBlock.Text = total.ToString();
            ActiveUsersTextBlock.Text = active.ToString();
            LockedUsersTextBlock.Text = locked.ToString();
        }
    }
}
