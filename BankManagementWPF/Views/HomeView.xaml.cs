using BankBusinessLayer;
using BankDataAccessLayer;
using BankManagementSystem.WPF.Security;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BankManagementSystem.WPF.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Loaded += HomeView_Loaded;
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = CurrentUserSession.CurrentUser;
            if (user == null || user.Role != "User")
            {
                var btn = this.FindName("DeleteAccountButton") as Button;
                var faceBtn = this.FindName("CaptureFaceButton") as Button;
                if (btn != null)
                    btn.Visibility = Visibility.Collapsed;
                if (faceBtn != null)
                    faceBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var user = CurrentUserSession.CurrentUser;
            if (user == null)
                return;

            if (MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.", "Delete Account", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if (User.LockUser(user.Username))
            {
                MessageBox.Show("Your account has been deleted.", "Account Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                Global.CurrentUser = null;
                CurrentUserSession.Logout();

                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    mainWindow.Close();
                }
            }
            else
            {
                MessageBox.Show("Failed to deactivate your account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CaptureFaceButton_Click(object sender, RoutedEventArgs e)
        {
            var captureWindow = new Window
            {
                Title = "Capture Face",
                Width = 640,
                Height = 480,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");
            if (!File.Exists(cascadePath))
            {
                MessageBox.Show($"Haar cascade file not found at: {cascadePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CascadeClassifier faceCascade;
            try
            {
                faceCascade = new CascadeClassifier(cascadePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load Haar cascade: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var capture = new VideoCapture(0); // Webcam
            if (!capture.IsOpened)
            {
                MessageBox.Show("Failed to open webcam.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var imageControl = new System.Windows.Controls.Image();
            var captureButton = new Button { Content = "Capture and Save", Width = 120, Height = 30, Margin = new Thickness(10) };
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(imageControl);
            stackPanel.Children.Add(captureButton);
            captureWindow.Content = stackPanel;

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
                                // Marshal UI update to the UI thread
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

            captureButton.Click += (s, args) =>
            {
                using (var frame = capture.QueryFrame())
                {
                    if (frame == null)
                    {
                        MessageBox.Show("Failed to capture frame from webcam.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                                byte[] faceBytes;
                                using (var ms = new MemoryStream())
                                {
                                    croppedFace.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    faceBytes = ms.ToArray();
                                }

                                // Save to MySQL
                                string connectionString = "Server=localhost;Database=bankdb;Uid=root;Pwd=your_password;";
                                using (var conn = new MySqlConnection(connectionString))
                                {
                                    try
                                    {
                                        conn.Open();
                                        string query = "INSERT INTO FaceData (clientId, faceImage) VALUES (@clientId, @faceImage)";
                                        using (var cmd = new MySqlCommand(query, conn))
                                        {
                                            int clientId = GetClientIdByAccountNumber(CurrentUserSession.CurrentUser.accountNumber);
                                            cmd.Parameters.AddWithValue("@clientId", clientId);
                                            cmd.Parameters.AddWithValue("@faceImage", faceBytes);
                                            cmd.ExecuteNonQuery();
                                        }
                                        MessageBox.Show("Face captured and saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                        capture.Stop();
                                        captureWindow.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"Failed to save face data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No face detected. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            };

            captureWindow.Closed += (s, args) => capture.Stop();
            captureWindow.ShowDialog();
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
                bitmapImage.Freeze(); // Freeze to make it cross-thread accessible
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