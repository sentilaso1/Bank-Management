using System.Windows;
using System.Windows.Controls;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            DepositButton.Visibility = CurrentUserSession.HasPermission(Permission.ProcessDeposit)
                ? Visibility.Visible : Visibility.Collapsed;
            WithdrawButton.Visibility = CurrentUserSession.HasPermission(Permission.ProcessWithdraw)
                ? Visibility.Visible : Visibility.Collapsed;
            TransferButton.Visibility = CurrentUserSession.HasPermission(Permission.ProcessTransfer)
                ? Visibility.Visible : Visibility.Collapsed;
            ExportButton.Visibility = CurrentUserSession.HasPermission(Permission.ExportData)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new DepositView();
        }

        private void Withdraw_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new WithdrawView();
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TransferView();
        }

        private void TransferLogs_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TransferLogsView();
        }

        private void TotalBalances_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TotalBalancesView();
        }

        private void AllTransactions_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TextBlock { Text = "All transactions view not implemented." };
        }
    }
}
