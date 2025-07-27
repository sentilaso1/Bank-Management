using System.Data;
using System.Linq;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class AllTransactionsView : UserControl
    {
        public AllTransactionsView()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            string username = CurrentUserSession.CurrentUser?.accountNumber;
            DataTable allLogs = TransferLogs.GetAllTransferLogs();
            if (string.IsNullOrWhiteSpace(username))
            {
                TransactionsDataGrid.ItemsSource = allLogs.DefaultView;
            }
            else
            {
                var rows = allLogs.AsEnumerable()
                    .Where(r => r.Field<string>("PerformedBy") == username || r.Field<string>("ToAccountNumber") == username);
                DataTable table = rows.Any() ? rows.CopyToDataTable() : allLogs.Clone();
                TransactionsDataGrid.ItemsSource = table.DefaultView;
            }
        }
    }
}
