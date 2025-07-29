using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class UpdateClientView : UserControl
    {
        private Client _client;
        private bool _isUpdateMode;

        public UpdateClientView()
        {
            InitializeComponent();
            CurrentUserSession.CheckPermission(Permission.UpdateClient);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchAccountNumberTextBox.Text))
            {
                MessageBox.Show("Please enter an account number.");
                return;
            }

            if (!Client.IsClientExist(SearchAccountNumberTextBox.Text))
            {
                MessageBox.Show($"Client Not Found [{SearchAccountNumberTextBox.Text}]", "Find", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _client = Client.Find(SearchAccountNumberTextBox.Text);
            if (_client == null)
            {
                MessageBox.Show("Unable to load client information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AccountNumberTextBox.Text = _client.AccountNumber;
            FirstNameTextBox.Text = _client.FirstName;
            LastNameTextBox.Text = _client.LastName;
            EmailTextBox.Text = _client.Email;
            PhoneTextBox.Text = _client.PhoneNumber;

            UpdateFormGroup.IsEnabled = true;
            UpdateButton.IsEnabled = true;
            _isUpdateMode = true;
            SearchAccountNumberTextBox.IsEnabled = false;
        }

        private bool FillData()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || FirstNameTextBox.Text.Length < 3)
            {
                MessageBox.Show($"First Name [{FirstNameTextBox.Text}] Is Too Short. Try Another One!");
                return false;
            }
            _client.FirstName = FirstNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || LastNameTextBox.Text.Length < 3)
            {
                MessageBox.Show($"Last Name [{LastNameTextBox.Text}] Is Too Short. Try Another One!");
                return false;
            }
            _client.LastName = LastNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || EmailTextBox.Text.Length < 8)
            {
                MessageBox.Show("Email is invalid!");
                return false;
            }
            if (!EmailTextBox.Text.Contains("@") || !EmailTextBox.Text.Contains("."))
            {
                MessageBox.Show("Email format is invalid!");
                return false;
            }
            _client.Email = EmailTextBox.Text;

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || PhoneTextBox.Text.Length != 10)
            {
                MessageBox.Show("Phone Number is invalid!");
                return false;
            }
            _client.PhoneNumber = PhoneTextBox.Text;

            return true;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentUserSession.CheckPermission(Permission.UpdateClient);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You don't have permission to update clients.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isUpdateMode)
                return;

            if (!FillData())
                return;

            if (MessageBox.Show("Are You Sure You Want To Update this Client", "Update", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (_client.Save())
            {
                User user = User.FindById(_client.ClientID + 4);
                user.FirstName = _client.FirstName;
                user.LastName = _client.LastName;
                user.Email = _client.Email;
                user.PhoneNumber = _client.PhoneNumber;
                user.Save();
                MessageBox.Show($"Client Updated Successfully [{_client.AccountNumber}]", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                _isUpdateMode = false;
                ClearFields();
            }
            else
            {
                MessageBox.Show($"Failed to Update client [{_client.AccountNumber}]", "Update", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            SearchAccountNumberTextBox.Clear();
            SearchAccountNumberTextBox.IsEnabled = true;
            AccountNumberTextBox.Clear();
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            EmailTextBox.Clear();
            PhoneTextBox.Clear();
            UpdateFormGroup.IsEnabled = false;
            UpdateButton.IsEnabled = false;
        }
    }
}
