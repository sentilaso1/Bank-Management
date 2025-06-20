using System;
using System.Windows.Forms;
using BankBusinessLayer;
using Bank_Management_System;
using Bank_Management_System.Forms.Transactions;

namespace Bank_Management_System.Forms.Register
{
    public class FrmRegister : Form
    {
        TextBox txtUsername;
        TextBox txtPassword;
        TextBox txtFirstName;
        TextBox txtLastName;
        TextBox txtEmail;
        TextBox txtPhone;
        Button btnRegister;

        public FrmRegister()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Register";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 360);

            Label lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Location = new System.Drawing.Point(20, 20);
            lblUsername.AutoSize = true;

            txtUsername = new TextBox();
            txtUsername.Location = new System.Drawing.Point(120, 16);
            txtUsername.Width = 230;

            Label lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Location = new System.Drawing.Point(20, 60);
            lblPassword.AutoSize = true;

            txtPassword = new TextBox();
            txtPassword.Location = new System.Drawing.Point(120, 56);
            txtPassword.Width = 230;
            txtPassword.PasswordChar = '*';

            Label lblFirstName = new Label();
            lblFirstName.Text = "First Name";
            lblFirstName.Location = new System.Drawing.Point(20, 100);
            lblFirstName.AutoSize = true;

            txtFirstName = new TextBox();
            txtFirstName.Location = new System.Drawing.Point(120, 96);
            txtFirstName.Width = 230;

            Label lblLastName = new Label();
            lblLastName.Text = "Last Name";
            lblLastName.Location = new System.Drawing.Point(20, 140);
            lblLastName.AutoSize = true;

            txtLastName = new TextBox();
            txtLastName.Location = new System.Drawing.Point(120, 136);
            txtLastName.Width = 230;

            Label lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Location = new System.Drawing.Point(20, 180);
            lblEmail.AutoSize = true;

            txtEmail = new TextBox();
            txtEmail.Location = new System.Drawing.Point(120, 176);
            txtEmail.Width = 230;

            Label lblPhone = new Label();
            lblPhone.Text = "Phone";
            lblPhone.Location = new System.Drawing.Point(20, 220);
            lblPhone.AutoSize = true;

            txtPhone = new TextBox();
            txtPhone.Location = new System.Drawing.Point(120, 216);
            txtPhone.Width = 230;

            btnRegister = new Button();
            btnRegister.Text = "Register";
            btnRegister.Location = new System.Drawing.Point(140, 260);
            btnRegister.Click += btnRegister_Click;

            this.Controls.AddRange(new Control[] {
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                btnRegister
            });
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            if (username.Length <= 3)
            {
                MessageBox.Show("Invalid username.");
                return;
            }
            if (User.IsUserExists(username))
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            User user = new User();
            user.Username = username;
            user.Password = txtPassword.Text;
            user.FirstName = txtFirstName.Text.Trim();
            user.LastName = txtLastName.Text.Trim();
            user.Email = txtEmail.Text.Trim();
            user.PhoneNumber = txtPhone.Text.Trim();
            user.Permission =
                (int)FrmMainMenu.enMainMenuPermissions.enHome |
                (int)FrmMainMenu.enMainMenuPermissions.enTransactions |
                (int)FrmTransactions.enTransactions.enDeposit |
                (int)FrmTransactions.enTransactions.enWithdraw;

            if (user.Save())
            {
                MessageBox.Show("Account created successfully.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to create account.");
            }
        }
    }
}
