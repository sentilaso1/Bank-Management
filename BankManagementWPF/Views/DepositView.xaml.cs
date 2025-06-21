using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class DepositView : UserControl
    {
        private Client _client;

        public DepositView()
        {
            InitializeComponent();
        }

        private void FindAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountInfoBorder.Visibility = Visibility.Collapsed;
            DepositFormGroup.IsEnabled = false;
            ProcessDepositButton.IsEnabled = false;
            NewBalanceTextBlock.Text = string.Empty;
            if (!Client.IsClientExist(AccountNumberTextBox.Text))
            {
                MessageBox.Show($"Client Not Found [{AccountNumberTextBox.Text}] Try Another One", "Find", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _client = Client.Find(AccountNumberTextBox.Text);
            if (_client == null)
            {
                MessageBox.Show("Failed to load client info", "Find", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClientNameTextBlock.Text = _client.FullName;
            CurrentBalanceTextBlock.Text = $"Current Balance: {_client.Balance:C}";
            AccountInfoBorder.Visibility = Visibility.Visible;
            DepositFormGroup.IsEnabled = true;
            ProcessDepositButton.IsEnabled = true;
            DepositAmountTextBox.Focus();
        }

        private void ProcessDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null)
            {
                MessageBox.Show("Search for a client first", "Deposit", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(DepositAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                MessageBox.Show("Please enter a valid amount.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to deposit {amount:C} to account [{_client.AccountNumber}]?",
                "Confirm Deposit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            if (!Client.Deposit(_client.AccountNumber, amount))
            {
                MessageBox.Show("Deposit failed. The amount must be a positive number. Please try again.", "Deposit", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _client = Client.Find(_client.AccountNumber);
            CurrentBalanceTextBlock.Text = $"Current Balance: {_client.Balance:C}";
            NewBalanceTextBlock.Text = _client.Balance.ToString("C");
            MessageBox.Show("Amount Deposited Successfully", "Deposit", MessageBoxButton.OK, MessageBoxImage.Information);
            DepositAmountTextBox.Clear();
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            AccountNumberTextBox.Clear();
            DepositAmountTextBox.Clear();
            DescriptionTextBox.Clear();
            AccountInfoBorder.Visibility = Visibility.Collapsed;
            DepositFormGroup.IsEnabled = false;
            ProcessDepositButton.IsEnabled = false;
            NewBalanceTextBlock.Text = string.Empty;
            ClientNameTextBlock.Text = string.Empty;
            CurrentBalanceTextBlock.Text = string.Empty;
            _client = null;
        }
    }
}
