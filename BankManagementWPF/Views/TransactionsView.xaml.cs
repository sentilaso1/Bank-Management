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
            var role = CurrentUserSession.CurrentUser?.Role;

            if (role == "User")
            {
                DepositButton.Visibility = Visibility.Visible;
                WithdrawButton.Visibility = Visibility.Visible;
                TransferButton.Visibility = Visibility.Visible;
                AllTransactionsButton.Visibility = Visibility.Visible;
                TransferLogsButton.Visibility = Visibility.Collapsed;
                TotalBalancesButton.Visibility = Visibility.Collapsed;
            }
            else if (role == "Administrator" || role == "Manager")
            {
                DepositButton.Visibility = Visibility.Collapsed;
                WithdrawButton.Visibility = Visibility.Collapsed;
                TransferButton.Visibility = Visibility.Collapsed;
                AllTransactionsButton.Visibility = Visibility.Collapsed;
                TransferLogsButton.Visibility = Visibility.Visible;
                TotalBalancesButton.Visibility = Visibility.Visible;
            }
            else
            {
                DepositButton.Visibility = Visibility.Collapsed;
                WithdrawButton.Visibility = Visibility.Collapsed;
                TransferButton.Visibility = Visibility.Collapsed;
                AllTransactionsButton.Visibility = Visibility.Collapsed;
                TransferLogsButton.Visibility = Visibility.Collapsed;
                TotalBalancesButton.Visibility = Visibility.Collapsed;
            }
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
            TransactionContentArea.Content = new AllTransactionsView();
        }
    }
}
