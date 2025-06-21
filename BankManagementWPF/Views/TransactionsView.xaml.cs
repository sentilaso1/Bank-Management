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
            TransactionContentArea.Content = new TextBlock { Text = "Withdraw view not implemented." };
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TextBlock { Text = "Transfer view not implemented." };
        }

        private void TransferLogs_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TextBlock { Text = "Transfer logs view not implemented." };
        }

        private void TotalBalances_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TextBlock { Text = "Total balances view not implemented." };
        }

        private void AllTransactions_Click(object sender, RoutedEventArgs e)
        {
            TransactionContentArea.Content = new TextBlock { Text = "All transactions view not implemented." };
        }
    }
}
