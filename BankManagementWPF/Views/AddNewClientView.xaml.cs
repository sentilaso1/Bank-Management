using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class AddNewClientView : UserControl
    {
        private Client _client = new Client();

        public AddNewClientView()
        {
            InitializeComponent();
        }

        private bool FillData()
        {
            if (string.IsNullOrWhiteSpace(txtAccountNumber.Text) || txtAccountNumber.Text.Length <= 3)
            {
                MessageBox.Show($"Account Number [{txtAccountNumber.Text}] Not Valid. Try another one!");
                return false;
            }
            _client.AccountNumber = txtAccountNumber.Text;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || txtFirstName.Text.Length < 3)
            {
                MessageBox.Show($"First Name [{txtFirstName.Text}] Is Too Short. Try Another One!");
                return false;
            }
            _client.FirstName = txtFirstName.Text;

            if (string.IsNullOrWhiteSpace(txtLastName.Text) || txtLastName.Text.Length < 3)
            {
                MessageBox.Show($"Last Name [{txtLastName.Text}] Is Too Short. Try Another One!");
                return false;
            }
            _client.LastName = txtLastName.Text;

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || txtEmail.Text.Length < 8)
            {
                MessageBox.Show("Email is invalid!");
                return false;
            }
            _client.Email = txtEmail.Text;

            if (string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text.Length < 8)
            {
                MessageBox.Show("Phone Number is invalid!");
                return false;
            }
            _client.PhoneNumber = txtPhone.Text;

            if (!decimal.TryParse(txtBalance.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal balance))
            {
                MessageBox.Show("Invalid Balance Value!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            _client.Balance = balance;

            if (string.IsNullOrWhiteSpace(txtPinCode.Text) || txtPinCode.Text.Length != 4)
            {
                MessageBox.Show("Invalid PinCode!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            _client.PinCode = txtPinCode.Text;

            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Client.IsClientExist(txtAccountNumber.Text))
            {
                MessageBox.Show($"This Account Number Already Exists [{txtAccountNumber.Text}]", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!FillData())
                return;

            if (MessageBox.Show("Are You Sure You Want To Add this Client", "Add", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (_client.Save())
            {
                MessageBox.Show($"Client Added Successfully [{txtAccountNumber.Text}]", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                DisableAllTextBoxes();
            }
            else
            {
                MessageBox.Show($"Failed to add client [{txtAccountNumber.Text}]", "Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisableAllTextBoxes()
        {
            txtAccountNumber.IsEnabled = false;
            txtPinCode.IsEnabled = false;
            txtBalance.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtFirstName.IsEnabled = false;
            txtLastName.IsEnabled = false;
            txtPhone.IsEnabled = false;
        }

        private void EnableAllTextBoxes()
        {
            txtAccountNumber.IsEnabled = true;
            txtPinCode.IsEnabled = true;
            txtBalance.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtFirstName.IsEnabled = true;
            txtLastName.IsEnabled = true;
            txtPhone.IsEnabled = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtAccountNumber.Clear();
            txtPinCode.Clear();
            txtBalance.Clear();
            txtEmail.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            EnableAllTextBoxes();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Clear_Click(sender, e);
        }
    }
}
