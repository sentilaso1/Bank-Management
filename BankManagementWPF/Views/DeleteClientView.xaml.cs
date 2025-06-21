using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class DeleteClientView : UserControl
    {
        private Client _client;
        public DeleteClientView()
        {
            InitializeComponent();
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
                MessageBox.Show("Client does not exist!", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _client = Client.Find(SearchAccountNumberTextBox.Text);
            if (_client == null)
            {
                MessageBox.Show("Unable to load client information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AccountNumberText.Text = _client.AccountNumber;
            FullNameText.Text = _client.FirstName + " " + _client.LastName;
            EmailText.Text = _client.Email;
            PhoneText.Text = _client.PhoneNumber;

            ClientInfoGroup.IsEnabled = true;
            DeleteButton.IsEnabled = true;
            SearchAccountNumberTextBox.IsEnabled = false;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete this client", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;

            if (Client.DeleteClient(_client.AccountNumber))
            {
                MessageBox.Show("Client Deleted Successfully", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
            }
            else
            {
                MessageBox.Show("Failed to delete client!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            SearchAccountNumberTextBox.Clear();
            SearchAccountNumberTextBox.IsEnabled = true;
            AccountNumberText.Text = string.Empty;
            FullNameText.Text = string.Empty;
            EmailText.Text = string.Empty;
            PhoneText.Text = string.Empty;
            ClientInfoGroup.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            _client = null;
        }
    }
}
