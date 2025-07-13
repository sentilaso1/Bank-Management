using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;
using BankDataAccessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class WithdrawView : UserControl
    {
        private Client _client;
        private decimal _currentBalance;

        public WithdrawView()
        {
            InitializeComponent();

            // Load current user's account automatically
            if (CurrentUserSession.CurrentUser?.Role == "User")
            {
                var username = CurrentUserSession.CurrentUser.Username;
                if (Client.IsClientExist(username))
                {
                    AccountNumberTextBlock.Text = username;
                    LoadAccount(username);
                }
                else
                {
                    MessageBox.Show("No client account found for this user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    WithdrawFormGroup.IsEnabled = false;
                    ProcessWithdrawButton.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("User role not supported for withdrawal.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                WithdrawFormGroup.IsEnabled = false;
                ProcessWithdrawButton.IsEnabled = false;
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
                MessageBox.Show($"Client Not Found [{accountNumber}]", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _client = Client.Find(accountNumber);
            if (_client == null)
            {
                MessageBox.Show("Failed to load client info", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClientNameTextBlock.Text = $"{_client.FirstName} {_client.LastName}";
            CurrentBalanceTextBlock.Text = _client.Balance.ToString("C");
            _currentBalance = _client.Balance;
            MaxAmountTextBlock.Text = $"Max: {_currentBalance:C}";
            BalanceWarningTextBlock.Visibility = _currentBalance < 100 ? Visibility.Visible : Visibility.Collapsed;
            AccountInfoBorder.Visibility = Visibility.Visible;
            WithdrawFormGroup.IsEnabled = true;
            ProcessWithdrawButton.IsEnabled = true;
            WithdrawAmountTextBox.Focus();
        }

        private bool VerifyPinCode(string enteredPin)
        {
            string firstName = string.Empty, lastName = string.Empty, email = string.Empty, phoneNumber = string.Empty, pinCode = string.Empty;
            decimal balance = 0m;
            int clientId = -1;

            bool isFound = ClientsData.GetClientInfoByAccountNumber(_client.AccountNumber, ref firstName, ref lastName, ref email, ref phoneNumber, ref pinCode, ref balance, ref clientId);

            return isFound && pinCode == enteredPin;
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
                MessageBox.Show("No client account loaded.", "Withdraw", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(WithdrawAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PinCodeTextBox.Password))
            {
                MessageBox.Show("Please enter your PIN code.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!VerifyPinCode(PinCodeTextBox.Password))
            {
                MessageBox.Show("Invalid PIN code.", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (amount > _currentBalance)
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
            PinCodeTextBox.Clear();
            DescriptionTextBox.Clear();
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            WithdrawAmountTextBox.Clear();
            PinCodeTextBox.Clear();
            DescriptionTextBox.Clear();
            NewBalanceTextBlock.Text = string.Empty;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}