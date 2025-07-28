using System.Windows;
using BankBusinessLayer;
using BankManagementSystem.WPF.Views;
using BankManagementSystem.WPF.Security;

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
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public async void Login()
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                lblAttempts.Text = "Please enter both username and password.";
                lblAttempts.Visibility = Visibility.Visible;
                return;
            }
            txtPassword.Password = txtPassword.Password.Trim();
            txtUsername.Text = txtUsername.Text.Trim();
            var user = User.FindUserByUsernameAndPassword(txtUsername.Text, txtPassword.Password);
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = RoleMapping.GetRoleName(user.RoleId);
                }

                Global.CurrentUser = user;
                User.AddNewLoginRegisters(user.Username, System.DateTime.Now);

                var permissionService = new PermissionService(user.Role);
                CurrentUserSession.SetUser(user);
                CurrentUserSession.SetPermissionService(permissionService);

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
                    lblAttempts.Text = $"Login Failed Or Your Account has been locked. Try again. You have {Remaining} trial(s) left.";
                    lblAttempts.Visibility = Visibility.Visible;
                }
                else
                {
                    lblAttempts.Text = "Maximum login attempts exceeded";
                    lblAttempts.Visibility = Visibility.Visible;
                    MessageBox.Show(lblAttempts.Text, "Locked", MessageBoxButton.OK, MessageBoxImage.Error);
                    await System.Threading.Tasks.Task.Delay(1500);
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
