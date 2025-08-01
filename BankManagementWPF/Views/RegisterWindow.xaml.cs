﻿using System;
using System.Windows;
using System.Windows.Controls;
using BankDataAccessLayer;
using System.Text;

namespace BankManagementSystem.WPF.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private string GenerateUniqueAccountNumber()
        {
            Random random = new Random();
            string accountNumber;
            do
            {
                accountNumber = "ACC" + random.Next(0, 999).ToString(); 
            } while (ClientsData.isClientExist(accountNumber));
            return accountNumber;
        }

        private void Register()
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password) ||
                string.IsNullOrWhiteSpace(txtPinCode.Password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if username already exists
            if (UsersData.isUserExist(txtUsername.Text))
            {
                MessageBox.Show("Username already exists. Please choose a different username.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(txtEmail.Text.Length < 5 || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(txtPhone.Text.Length != 10 || !long.TryParse(txtPhone.Text, out _))
            {
                MessageBox.Show("Phone number must be exactly 10 digits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPinCode.Password.Length != 4 || !int.TryParse(txtPinCode.Password, out _))
            {
                MessageBox.Show("PIN code must be exactly 4 digits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string accountNumber = GenerateUniqueAccountNumber();

            int userId = UsersData.AddNewUser(
                txtUsername.Text,
                txtFirstName.Text,
                txtLastName.Text,
                txtEmail.Text,
                txtPhone.Text,
                txtPassword.Password,
                "User", 
                1,
                accountNumber
            );

            if (userId == -1)
            {
                MessageBox.Show("Failed to register user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Add new client to Clients table
            int clientId = ClientsData.AddNewClient(
                txtFirstName.Text,
                txtLastName.Text,
                txtEmail.Text,
                txtPhone.Text,
                accountNumber,
                txtPinCode.Password,
                0m 
            );

            if (clientId != -1)
            {
                MessageBox.Show($"Registration successful! Your account number is {accountNumber}. You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                // Rollback user creation if client creation fails
                UsersData.DeleteUser(txtUsername.Text);
                MessageBox.Show("Failed to register client. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}