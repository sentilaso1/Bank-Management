using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem.WPF.Views
{
    public partial class PermissionsView : UserControl
    {
        public PermissionsView()
        {
            InitializeComponent();
            LoadRoles();
        }

        private readonly List<string> _allPermissions = new()
        {
            "Add Clients",
            "Update Clients",
            "Delete Clients",
            "Process Deposits",
            "Process Withdrawals",
            "Process Transfers",
            "Add Users",
            "Update Users",
            "Delete Users",
            "View Reports",
            "Export Data",
            "System Settings"
        };

        private void LoadRoles()
        {
            RolesListBox.SelectedIndex = 0;
            DisplayRolePermissions("Administrator");
            UsersComboBox.ItemsSource = new List<string> { "admin", "manager", "cashier" };
        }

        private void DisplayRolePermissions(string role)
        {
            RolePermissionsPanel.Children.Clear();
            foreach (string perm in _allPermissions)
            {
                RolePermissionsPanel.Children.Add(new CheckBox { Content = perm, IsChecked = true });
            }
        }

        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesListBox.SelectedItem is ListBoxItem item)
            {
                DisplayRolePermissions(item.Tag.ToString());
            }
        }

        private void RefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Refresh users - not implemented", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void User_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentUserPermissionsPanel.Children.Clear();
            AvailablePermissionsPanel.Children.Clear();
            if (UsersComboBox.SelectedItem == null)
                return;

            foreach (string perm in _allPermissions)
            {
                CurrentUserPermissionsPanel.Children.Add(new CheckBox { Content = perm, IsChecked = true });
            }
        }

        private void LoadAudit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Load audit log - not implemented", "Audit", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
