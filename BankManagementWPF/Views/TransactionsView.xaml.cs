using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem.WPF.Views
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
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
