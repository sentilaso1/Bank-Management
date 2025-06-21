using System.Windows;
using BankBusinessLayer;
using BankManagementSystem.WPF.Views;

namespace BankManagementSystem.WPF.Views
{
    public partial class LoginWindow : Window
    {
        int Attempts = 0;
        const int MaxAttempts = 3;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddNewUserWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        public async void Login()
        {
            Global.CurrentUser = User.FindUserByUsernameAndPassword(txtUsername.Text, txtPassword.Password);
            if (Global.CurrentUser != null)
            {
                User.AddNewLoginRegisters(Global.CurrentUser.Username, System.DateTime.Now);
                var main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                Attempts++;
                int Remaining = MaxAttempts - Attempts;
                if (Remaining > 0)
                {
                    lblAttempts.Text = $"Login Failed. Try again. You have {Remaining} trial(s) left.";
                    lblAttempts.Visibility = Visibility.Visible;
                }
                else
                {
                    lblAttempts.Text = "Maximum login attempts exceeded.";
                    MessageBox.Show(lblAttempts.Text, "Locked", MessageBoxButton.OK, MessageBoxImage.Error);
                    await System.Threading.Tasks.Task.Delay(1500);
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
