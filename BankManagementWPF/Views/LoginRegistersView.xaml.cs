using System.Data;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class LoginRegistersView : UserControl
    {
        public LoginRegistersView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DataTable dt = User.GetAllLoginRegisters();
            LoginRegistersDataGrid.ItemsSource = dt.DefaultView;
        }
    }
}
