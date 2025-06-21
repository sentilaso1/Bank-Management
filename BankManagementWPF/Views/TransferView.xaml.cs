using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class TransferView : UserControl
    {
        private Client _fromClient;
        private Client _toClient;
        private decimal _feePercent = 0.02m; // 2% fee
        public TransferView()
        {
            InitializeComponent();
        }

        private void FindFromAccount_Click(object sender, RoutedEventArgs e)
        {
            FromAccountInfoBorder.Visibility = Visibility.Collapsed;
            TransferDetailsGroup.IsEnabled = false;
            ProcessTransferButton.IsEnabled = false;
            if (!Client.IsClientExist(FromAccountTextBox.Text))
            {
                MessageBox.Show("Source account not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _fromClient = Client.Find(FromAccountTextBox.Text);
            if (_fromClient == null)
            {
                MessageBox.Show("Failed to load source account.", "Find", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FromClientNameTextBlock.Text = _fromClient.FullName;
            FromBalanceTextBlock.Text = _fromClient.Balance.ToString("C");
            FromAccountInfoBorder.Visibility = Visibility.Visible;
            EnableTransferDetails();
        }

        private void FindToAccount_Click(object sender, RoutedEventArgs e)
        {
            ToAccountInfoBorder.Visibility = Visibility.Collapsed;
            TransferDetailsGroup.IsEnabled = false;
            ProcessTransferButton.IsEnabled = false;
            if (!Client.IsClientExist(ToAccountTextBox.Text))
            {
                MessageBox.Show("Destination account not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _toClient = Client.Find(ToAccountTextBox.Text);
            if (_toClient == null)
            {
                MessageBox.Show("Failed to load destination account.", "Find", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ToClientNameTextBlock.Text = _toClient.FullName;
            ToBalanceTextBlock.Text = _toClient.Balance.ToString("C");
            ToAccountInfoBorder.Visibility = Visibility.Visible;
            EnableTransferDetails();
        }

        private void EnableTransferDetails()
        {
            if (_fromClient != null && _toClient != null)
            {
                TransferDetailsGroup.IsEnabled = true;
                ProcessTransferButton.IsEnabled = true;
                TransferAmountTextBox.Focus();
                UpdatePreview();
            }
        }

        private void TransferAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (_fromClient == null || _toClient == null)
                return;

            if (!decimal.TryParse(TransferAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                TransferFeeTextBlock.Text = string.Empty;
                TotalDeductedTextBlock.Text = string.Empty;
                FromNewBalanceTextBlock.Text = string.Empty;
                ToNewBalanceTextBlock.Text = string.Empty;
                ProcessTransferButton.IsEnabled = false;
                return;
            }

            decimal fee = amount * _feePercent;
            decimal totalDeducted = amount + fee;
            TransferFeeTextBlock.Text = fee.ToString("C");
            TotalDeductedTextBlock.Text = totalDeducted.ToString("C");
            FromNewBalanceTextBlock.Text = (_fromClient.Balance - totalDeducted).ToString("C");
            ToNewBalanceTextBlock.Text = (_toClient.Balance + amount).ToString("C");
            ProcessTransferButton.IsEnabled = amount > 0 && totalDeducted <= _fromClient.Balance;
        }

        private void ProcessTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (_fromClient == null || _toClient == null)
            {
                MessageBox.Show("Load both accounts first.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TransferAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                MessageBox.Show("Invalid amount.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal fee = amount * _feePercent;
            decimal totalDeducted = amount + fee;
            if (totalDeducted > _fromClient.Balance)
            {
                MessageBox.Show("Insufficient balance in source account.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Transfer {amount:C} (+{fee:C} fee) from {_fromClient.AccountNumber} to {_toClient.AccountNumber}?",
                "Confirm Transfer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            if (!_fromClient.Transfer(_fromClient.AccountNumber, _toClient.AccountNumber, amount))
            {
                MessageBox.Show("Transfer failed.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _fromClient = Client.Find(_fromClient.AccountNumber);
            _toClient = Client.Find(_toClient.AccountNumber);
            FromBalanceTextBlock.Text = _fromClient.Balance.ToString("C");
            ToBalanceTextBlock.Text = _toClient.Balance.ToString("C");
            MessageBox.Show("Transfer completed successfully", "Transfer", MessageBoxButton.OK, MessageBoxImage.Information);
            TransferAmountTextBox.Clear();
            TransferDescriptionTextBox.Clear();
            UpdatePreview();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            FromAccountTextBox.Clear();
            ToAccountTextBox.Clear();
            TransferAmountTextBox.Clear();
            TransferDescriptionTextBox.Clear();
            TransferFeeTextBlock.Text = string.Empty;
            TotalDeductedTextBlock.Text = string.Empty;
            FromNewBalanceTextBlock.Text = string.Empty;
            ToNewBalanceTextBlock.Text = string.Empty;
            FromAccountInfoBorder.Visibility = Visibility.Collapsed;
            ToAccountInfoBorder.Visibility = Visibility.Collapsed;
            TransferDetailsGroup.IsEnabled = false;
            ProcessTransferButton.IsEnabled = false;
            _fromClient = null;
            _toClient = null;
        }
    }
}
