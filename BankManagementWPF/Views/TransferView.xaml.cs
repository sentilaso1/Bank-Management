using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BankBusinessLayer;
using BankManagementSystem.WPF.Security;
using BankDataAccessLayer;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows.Media.Imaging;
using Emgu.CV;

namespace BankManagementSystem.WPF.Views
{
    public partial class TransferView : UserControl
    {
        private Client _fromClient;
        private Client _toClient;
        private decimal _feePercent = 0.02m; // 2% fee

        private readonly Dictionary<string, List<string>> _transferSuggestions = new Dictionary<string, List<string>>
        {
            { "Personal", new List<string> {
                "Gift for family member",
                "Personal expense reimbursement",
                "Loan repayment to friend"
            }},
            { "Business", new List<string> {
                "Payment for services",
                "Invoice settlement",
                "Business expense reimbursement"
            }},
            { "Charity", new List<string> {
                "Donation to non-profit",
                "Charitable contribution",
                "Support for community event"
            }},
            { "Other", new List<string> {
                "Miscellaneous payment",
                "General transfer",
                "Other purpose"
            }}
        };

        public TransferView()
        {
            InitializeComponent();

            TransferPurposeComboBox.ItemsSource = _transferSuggestions.Keys;
            TransferPurposeComboBox.SelectedIndex = 0;

            if (CurrentUserSession.CurrentUser?.Role == "User")
            {
                var username = CurrentUserSession.CurrentUser.accountNumber;
                if (Client.IsClientExist(username))
                {
                    FromAccountTextBlock.Text = username;
                    LoadFromAccount(username);
                }
                else
                {
                    MessageBox.Show("No client account found for this user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    TransferDetailsGroup.IsEnabled = false;
                    ProcessTransferButton.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("User role not supported for transfer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TransferDetailsGroup.IsEnabled = false;
                ProcessTransferButton.IsEnabled = false;
            }
        }

        private void LoadFromAccount(string accountNumber)
        {
            FromAccountInfoBorder.Visibility = Visibility.Collapsed;
            TransferDetailsGroup.IsEnabled = false;
            ProcessTransferButton.IsEnabled = false;

            if (!Client.IsClientExist(accountNumber))
            {
                MessageBox.Show($"Source account not found [{accountNumber}].", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _fromClient = Client.Find(accountNumber);
            if (_fromClient == null)
            {
                MessageBox.Show("Failed to load source account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FromClientNameTextBlock.Text = $"{_fromClient.FirstName} {_fromClient.LastName}";
            FromBalanceTextBlock.Text = _fromClient.Balance.ToString("C");
            FromAccountInfoBorder.Visibility = Visibility.Visible;
            EnableTransferDetails();
        }

        private void FindToAccount_Click(object sender, RoutedEventArgs e)
        {
            ToAccountInfoBorder.Visibility = Visibility.Collapsed;
            TransferDetailsGroup.IsEnabled = false;
            ProcessTransferButton.IsEnabled = false;

            if (!Client.IsClientExist(ToAccountTextBox.Text))
            {
                MessageBox.Show("Destination account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _toClient = Client.Find(ToAccountTextBox.Text);
            if (_toClient == null)
            {
                MessageBox.Show("Failed to load destination account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_toClient.AccountNumber == _fromClient.AccountNumber)
            {
                MessageBox.Show("Cannot transfer to the same account.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ToClientNameTextBlock.Text = $"{_toClient.FirstName} {_toClient.LastName}";
            ToAccountInfoBorder.Visibility = Visibility.Visible;
            EnableTransferDetails();
        }

        private void EnableTransferDetails()
        {
            if (_fromClient != null && _toClient != null)
            {
                TransferDetailsGroup.IsEnabled = true;
                ProcessTransferButton.IsEnabled = true;
                TransferAmountTextBox.Focus();
                UpdatePreview();
            }
        }

        private void TransferAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (_fromClient == null || _toClient == null)
                return;

            if (!decimal.TryParse(TransferAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                TransferFeeTextBlock.Text = string.Empty;
                TotalDeductedTextBlock.Text = string.Empty;
                ProcessTransferButton.IsEnabled = false;
                return;
            }

            decimal fee = amount * _feePercent;
            decimal totalDeducted = amount + fee;
            TransferFeeTextBlock.Text = fee.ToString("C");
            TotalDeductedTextBlock.Text = totalDeducted.ToString("C");
            ProcessTransferButton.IsEnabled = amount > 0 && totalDeducted <= _fromClient.Balance;
        }

        private bool VerifyPinCode(string enteredPin)
        {
            string firstName = string.Empty, lastName = string.Empty, email = string.Empty, phoneNumber = string.Empty, pinCode = string.Empty;
            decimal balance = 0m;
            int clientId = -1;

            bool isFound = ClientsData.GetClientInfoByAccountNumber(_fromClient.AccountNumber, ref firstName, ref lastName, ref email, ref phoneNumber, ref pinCode, ref balance, ref clientId);

            return isFound && pinCode == enteredPin;
        }

        private bool VerifyFace()
        {
            string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");
            if (!File.Exists(cascadePath))
            {
                MessageBox.Show($"Haar cascade file not found at: {cascadePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            CascadeClassifier faceCascade;
            try
            {
                faceCascade = new CascadeClassifier(cascadePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load Haar cascade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var capture = new VideoCapture(0);
            if (!capture.IsOpened)
            {
                MessageBox.Show("Failed to open webcam.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var verifyWindow = new Window
            {
                Title = "Verify Face",
                Width = 640,
                Height = 480,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            var imageControl = new System.Windows.Controls.Image();
            var verifyButton = new Button { Content = "Verify Face", Width = 120, Height = 30, Margin = new Thickness(10) };
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(imageControl);
            stackPanel.Children.Add(verifyButton);
            verifyWindow.Content = stackPanel;

            bool isVerified = false;
            capture.ImageGrabbed += (s, args) =>
            {
                using (var frame = capture.QueryFrame())
                {
                    if (frame == null)
                        return;

                    using (var grayFrame = new Mat())
                    {
                        CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);
                        var faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 3, System.Drawing.Size.Empty);
                        if (faces.Length > 0)
                        {
                            var face = faces[0];
                            using (var faceImage = frame.ToBitmap())
                            {
                                var croppedFace = new Bitmap(face.Width, face.Height);
                                using (var graphics = Graphics.FromImage(croppedFace))
                                {
                                    graphics.DrawImage(faceImage, 0, 0, new Rectangle(face.X, face.Y, face.Width, face.Height), GraphicsUnit.Pixel);
                                }
                                Dispatcher.Invoke(() =>
                                {
                                    imageControl.Source = BitmapToImageSource(croppedFace);
                                });
                            }
                        }
                    }
                }
            };
            capture.Start();

            verifyButton.Click += (s, args) =>
            {
                using (var frame = capture.QueryFrame())
                {
                    if (frame == null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Failed to capture frame from webcam.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            capture.Stop();
                            verifyWindow.Close();
                        });
                        return;
                    }

                    using (var grayFrame = new Mat())
                    {
                        CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);
                        var faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 3, System.Drawing.Size.Empty);
                        if (faces.Length > 0)
                        {
                            var face = faces[0];
                            using (var faceImage = frame.ToBitmap())
                            {
                                var croppedFace = new Bitmap(face.Width, face.Height);
                                using (var graphics = Graphics.FromImage(croppedFace))
                                {
                                    graphics.DrawImage(faceImage, 0, 0, new Rectangle(face.X, face.Y, face.Width, face.Height), GraphicsUnit.Pixel);
                                }
                                byte[] capturedFaceBytes;
                                using (var ms = new MemoryStream())
                                {
                                    croppedFace.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    capturedFaceBytes = ms.ToArray();
                                }

                                // Retrieve stored face
                                byte[] storedFaceBytes = null;
                                string connectionString = "Server=localhost;Database=bankdb;Uid=root;Pwd=senti123;";
                                using (var conn = new MySqlConnection(connectionString))
                                {
                                    try
                                    {
                                        conn.Open();
                                        string query = "SELECT faceImage FROM FaceData WHERE clientId = @clientId ORDER BY createdAt DESC LIMIT 1";
                                        using (var cmd = new MySqlCommand(query, conn))
                                        {
                                            int clientId = GetClientIdByAccountNumber(_fromClient.AccountNumber);
                                            cmd.Parameters.AddWithValue("@clientId", clientId);
                                            using (var reader = cmd.ExecuteReader())
                                            {
                                                if (reader.Read())
                                                {
                                                    storedFaceBytes = (byte[])reader["faceImage"];
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            MessageBox.Show($"Failed to retrieve face data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                            capture.Stop();
                                            verifyWindow.Close();
                                        });
                                        return;
                                    }
                                }

                                if (storedFaceBytes == null)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        MessageBox.Show("No face data found for this user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        capture.Stop();
                                        verifyWindow.Close();
                                    });
                                    return;
                                }

                                // Convert byte arrays to Mat
                                using (var capturedBitmap = new Bitmap(new MemoryStream(capturedFaceBytes)))
                                using (var storedBitmap = new Bitmap(new MemoryStream(storedFaceBytes)))
                                using (var capturedMat = capturedBitmap.ToMat())
                                using (var storedMat = storedBitmap.ToMat())
                                {
                                    using (var capturedGray = new Mat())
                                    using (var storedGray = new Mat())
                                    {
                                        CvInvoke.CvtColor(capturedMat, capturedGray, ColorConversion.Bgr2Gray);
                                        CvInvoke.CvtColor(storedMat, storedGray, ColorConversion.Bgr2Gray);

                                        if (capturedGray.Size != storedGray.Size)
                                        {
                                            CvInvoke.Resize(storedGray, storedGray, capturedGray.Size);
                                        }

                                        using (var result = new Mat())
                                        {
                                            CvInvoke.MatchTemplate(capturedGray, storedGray, result, TemplateMatchingType.CcoeffNormed);
                                            double[] minValues, maxValues;
                                            System.Drawing.Point[] minLocations, maxLocations;
                                            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                                            if (maxValues[0] > 0.3) // Threshold for similarity
                                            {
                                                isVerified = true;
                                                Dispatcher.Invoke(() =>
                                                {
                                                    MessageBox.Show("Face verified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    capture.Stop();
                                                    verifyWindow.Close();
                                                });
                                            }
                                            else
                                            {
                                                Dispatcher.Invoke(() =>
                                                {
                                                    MessageBox.Show("Face verification failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                    capture.Stop();
                                                    verifyWindow.Close();
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("No face detected. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            });
                        }
                    }
                }
            };

            verifyWindow.Closed += (s, args) => capture.Stop();
            verifyWindow.ShowDialog();
            return isVerified;
        }

        private void ProcessTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (_fromClient == null || _toClient == null)
            {
                MessageBox.Show("Load both accounts first.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TransferAmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Invalid amount. Please enter a positive number.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PinCodeTextBox.Password))
            {
                MessageBox.Show("Please enter your PIN code.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!VerifyPinCode(PinCodeTextBox.Password))
            {
                MessageBox.Show("Invalid PIN code.", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal fee = amount * _feePercent;
            decimal totalDeducted = amount + fee;
            if (totalDeducted <= 0)
            {
                MessageBox.Show("Total amount to be transferred must be greater than zero.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (totalDeducted > _fromClient.Balance)
            {
                MessageBox.Show("Insufficient balance in source account.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (amount > 30000)
            {
                if (!VerifyFace())
                {
                    MessageBox.Show("Face verification required for transfers over 30,000.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (MessageBox.Show($"Transfer {amount:C} (+{fee:C} fee) from {_fromClient.AccountNumber} to {_toClient.AccountNumber}?",
                "Confirm Transfer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            if (!_fromClient.Transfer(_fromClient.AccountNumber, _toClient.AccountNumber, amount))
            {
                MessageBox.Show("Transfer failed.", "Transfer", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var log = new TransferLogs
            {
                Date = DateTime.Now,
                AccountForom = _fromClient.AccountNumber,
                AccountTo = _toClient.AccountNumber,
                Amount = amount,
                PerformedBy = CurrentUserSession.CurrentUser?.accountNumber
            };
            log.Tin = TransferDescriptionTextBox.Text.Trim();
            log.AddTrnasferLog();

            _fromClient = Client.Find(_fromClient.AccountNumber);
            _toClient = Client.Find(_toClient.AccountNumber);
            FromBalanceTextBlock.Text = _fromClient.Balance.ToString("C");
            MessageBox.Show("Transfer completed successfully", "Transfer", MessageBoxButton.OK, MessageBoxImage.Information);
            TransferAmountTextBox.Clear();
            PinCodeTextBox.Clear();
            TransferDescriptionTextBox.Clear();
            TransferPurposeComboBox.SelectedIndex = 0;
            UpdatePreview();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ToAccountTextBox.Clear();
            TransferAmountTextBox.Clear();
            PinCodeTextBox.Clear();
            TransferDescriptionTextBox.Clear();
            TransferPurposeComboBox.SelectedIndex = 0;
            TransferFeeTextBlock.Text = string.Empty;
            TotalDeductedTextBlock.Text = string.Empty;
            ToAccountInfoBorder.Visibility = Visibility.Collapsed;
            TransferDetailsGroup.IsEnabled = false;
            ProcessTransferButton.IsEnabled = false;
            _toClient = null;
        }

        private void SuggestDescription_Click(object sender, RoutedEventArgs e)
        {
            string selectedPurpose = TransferPurposeComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPurpose) || !_transferSuggestions.ContainsKey(selectedPurpose))
            {
                MessageBox.Show("Please select a valid transfer purpose.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var suggestions = _transferSuggestions[selectedPurpose];
            var suggestionWindow = new Window
            {
                Title = "Transfer Description Suggestions",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            var listBox = new ListBox { ItemsSource = suggestions, Margin = new Thickness(0, 0, 0, 10) };
            listBox.SelectionChanged += (s, args) =>
            {
                if (listBox.SelectedItem != null)
                {
                    TransferDescriptionTextBox.Text = listBox.SelectedItem.ToString();
                    suggestionWindow.Close();
                }
            };

            var closeButton = new Button
            {
                Content = "Close",
                Width = 80,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            closeButton.Click += (s, args) => suggestionWindow.Close();

            stackPanel.Children.Add(new TextBlock { Text = "Select a suggestion:", Margin = new Thickness(0, 0, 0, 10) });
            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(closeButton);
            suggestionWindow.Content = stackPanel;
            suggestionWindow.ShowDialog();
        }

        private System.Windows.Media.Imaging.BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private int GetClientIdByAccountNumber(string accountNumber)
        {
            string firstName = string.Empty, lastName = string.Empty, email = string.Empty, phoneNumber = string.Empty, pinCode = string.Empty;
            decimal balance = 0m;
            int clientId = -1;
            ClientsData.GetClientInfoByAccountNumber(accountNumber, ref firstName, ref lastName, ref email, ref phoneNumber, ref pinCode, ref balance, ref clientId);
            return clientId;
        }
    }
}