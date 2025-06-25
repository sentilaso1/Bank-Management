using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class WithdrawView : UserControl
    {
        private Client _client;
        private decimal _currentBalance;
        public WithdrawView()
        {
            InitializeComponent();

            if (CurrentUserSession.CurrentUser?.Role == "User")
            {
                var username = CurrentUserSession.CurrentUser.Username;
                if (Client.IsClientExist(username))
                {
                    AccountSearchGroup.Visibility = Visibility.Collapsed;
                    AccountNumberTextBox.Text = username;
                    LoadAccount(username);
                }
                else
                {
                    AccountSearchGroup.Visibility = Visibility.Visible;
                }
            }
        }

        private void LoadAccount(string accountNumber)
        {
            AccountInfoBorder.Visibility = Visibility.Collapsed;
            WithdrawFormGroup.IsEnabled = false;
            ProcessWithdrawButton.IsEnabled = false;
            NewBalanceTextBlock.Text = string.Empty;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;

            if (!Client.IsClientExist(accountNumber))
            {
                MessageBox.Show($"Client Not Found [{accountNumber}] Try Another One", "Find", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _client = Client.Find(accountNumber);
            if (_client == null)
            {
                MessageBox.Show("Failed to load client info", "Find", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClientNameTextBlock.Text = _client.FirstName;
            CurrentBalanceTextBlock.Text = _client.Balance.ToString("C");
            _currentBalance = _client.Balance;
            MaxAmountTextBlock.Text = $"Max: {_currentBalance:C}";
            BalanceWarningTextBlock.Visibility = _currentBalance < 100 ? Visibility.Visible : Visibility.Collapsed;
            AccountInfoBorder.Visibility = Visibility.Visible;
            WithdrawFormGroup.IsEnabled = true;
            ProcessWithdrawButton.IsEnabled = true;
            WithdrawAmountTextBox.Focus();
        }

        private void FindAccount_Click(object sender, RoutedEventArgs e)
        {
            LoadAccount(AccountNumberTextBox.Text);
        }

        private void WithdrawAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            if (!decimal.TryParse(WithdrawAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                NewBalanceTextBlock.Text = string.Empty;
                return;
            }

            decimal newBalance = _currentBalance - amount;
            NewBalanceTextBlock.Text = newBalance.ToString("C");
        }

        private void ProcessWithdraw_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null)
            {
                MessageBox.Show("Search for a client first", "Withdraw", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(WithdrawAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                MessageBox.Show("Please enter a valid amount.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (amount <= 0 || amount > _currentBalance)
            {
                ValidationMessageTextBlock.Text = "Insufficient balance";
                ValidationMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (MessageBox.Show($"Are you sure you want to withdraw {amount:C} from account [{_client.AccountNumber}]?",
                "Confirm Withdrawal", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            if (!Client.Withdraw(_client.AccountNumber, _currentBalance, amount))
            {
                MessageBox.Show("Withdrawal failed. Please check amount and try again.", "Withdraw", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var log = new TransferLogs
            {
                Date = DateTime.Now,
                AccountForom = _client.AccountNumber,
                AccountTo = null,
                Amount = amount,
                PerformedBy = CurrentUserSession.CurrentUser?.Username
            };
            log.AddTrnasferLog();

            _client = Client.Find(_client.AccountNumber);
            _currentBalance = _client.Balance;
            CurrentBalanceTextBlock.Text = _currentBalance.ToString("C");
            NewBalanceTextBlock.Text = _currentBalance.ToString("C");
            MaxAmountTextBlock.Text = $"Max: {_currentBalance:C}";
            MessageBox.Show("Amount Withdrawn Successfully", "Withdraw", MessageBoxButton.OK, MessageBoxImage.Information);
            WithdrawAmountTextBox.Clear();
            DescriptionTextBox.Clear();
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            AccountNumberTextBox.Clear();
            WithdrawAmountTextBox.Clear();
            DescriptionTextBox.Clear();
            AccountInfoBorder.Visibility = Visibility.Collapsed;
            WithdrawFormGroup.IsEnabled = false;
            ProcessWithdrawButton.IsEnabled = false;
            NewBalanceTextBlock.Text = string.Empty;
            ClientNameTextBlock.Text = string.Empty;
            CurrentBalanceTextBlock.Text = string.Empty;
            MaxAmountTextBlock.Text = string.Empty;
            BalanceWarningTextBlock.Visibility = Visibility.Collapsed;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            _client = null;
            _currentBalance = 0;
        }
    }
}
