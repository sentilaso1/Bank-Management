using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;
using BankDataAccessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class DepositView : UserControl
    {
        private Client _client;

        public DepositView()
        {
            InitializeComponent();

            // Load current user's account automatically
            if (CurrentUserSession.CurrentUser?.Role == "User")
            {
                var username = CurrentUserSession.CurrentUser.accountNumber;
                if (Client.IsClientExist(username))
                {
                    AccountNumberTextBlock.Text = username;
                    LoadAccount(username);
                }
                else
                {
                    MessageBox.Show("No client account found for this user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    DepositFormGroup.IsEnabled = false;
                    ProcessDepositButton.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("User role not supported for deposit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DepositFormGroup.IsEnabled = false;
                ProcessDepositButton.IsEnabled = false;
            }
        }

        private void LoadAccount(string accountNumber)
        {
            AccountInfoBorder.Visibility = Visibility.Collapsed;
            DepositFormGroup.IsEnabled = false;
            ProcessDepositButton.IsEnabled = false;
            NewBalanceTextBlock.Text = string.Empty;

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
            CurrentBalanceTextBlock.Text = $"Current Balance: {_client.Balance:C}";
            AccountInfoBorder.Visibility = Visibility.Visible;
            DepositFormGroup.IsEnabled = true;
            ProcessDepositButton.IsEnabled = true;
            DepositAmountTextBox.Focus();
        }

        private bool VerifyPinCode(string enteredPin)
        {
            string firstName = string.Empty, lastName = string.Empty, email = string.Empty, phoneNumber = string.Empty, pinCode = string.Empty;
            decimal balance = 0m;
            int clientId = -1;

            bool isFound = ClientsData.GetClientInfoByAccountNumber(_client.AccountNumber, ref firstName, ref lastName, ref email, ref phoneNumber, ref pinCode, ref balance, ref clientId);

            return isFound && pinCode == enteredPin;
        }

        private void ProcessDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null)
            {
                MessageBox.Show("No client account loaded.", "Deposit", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(DepositAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
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

            if (MessageBox.Show($"Are you sure you want to deposit {amount:C} to account [{_client.AccountNumber}]?",
                "Confirm Deposit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            if (!Client.Deposit(_client.AccountNumber, amount))
            {
                MessageBox.Show("Deposit failed. Please try again.", "Deposit", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var log = new TransferLogs
            {
                Date = DateTime.Now,
                AccountForom = null,
                AccountTo = _client.AccountNumber,
                Amount = amount,
                PerformedBy = CurrentUserSession.CurrentUser?.accountNumber
            };
            log.Tin = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? "Deposit" : DescriptionTextBox.Text;
            log.AddTrnasferLog();

            _client = Client.Find(_client.AccountNumber);
            CurrentBalanceTextBlock.Text = $"Current Balance: {_client.Balance:C}";
            NewBalanceTextBlock.Text = _client.Balance.ToString("C");
            MessageBox.Show("Amount Deposited Successfully", "Deposit", MessageBoxButton.OK, MessageBoxImage.Information);
            DepositAmountTextBox.Clear();
            PinCodeTextBox.Clear();
            DescriptionTextBox.Clear();
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            DepositAmountTextBox.Clear();
            PinCodeTextBox.Clear();
            DescriptionTextBox.Clear();
            NewBalanceTextBlock.Text = string.Empty;
        }
    }
}