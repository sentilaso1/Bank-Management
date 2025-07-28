using BankBusinessLayer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BankManagementSystem.WPF.Security;

namespace BankManagementSystem.WPF.Views
{
    public partial class GoalSettingControl : UserControl
    {
        private string generatedPlan = string.Empty;
        private bool isGenerating = false;

        public GoalSettingControl()
        {
            InitializeComponent();
        }

        private async void GeneratePlanButton_Click(object sender, RoutedEventArgs e)
        {
            string goal = GoalInputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(goal))
            {
                MessageBox.Show("Vui lòng nhập mục tiêu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isGenerating)
                return;

            isGenerating = true;
            GeneratePlanButton.IsEnabled = false;
            PlanOutputTextBlock.Text = "Đang tạo kế hoạch, vui lòng đợi...";
            DownloadPdfButton.IsEnabled = false;

            try
            {
                generatedPlan = await GeneratePlanWithGemini(goal);
                PlanOutputTextBlock.Text = generatedPlan;
                DownloadPdfButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                PlanOutputTextBlock.Text = $"Lỗi khi tạo kế hoạch: {ex.Message}";
                DownloadPdfButton.IsEnabled = false;
            }
            finally
            {
                isGenerating = false;
                GeneratePlanButton.IsEnabled = true;
            }
        }

        private async Task<string> GeneratePlanWithGemini(string goal)
        {
            string apiKey = "YOUR_GEMINI_API_KEY"; // Thay bằng API key thực tế
            string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
            string accountNumber = CurrentUserSession.CurrentUser.accountNumber;
            Client temp = Client.Find(accountNumber);
            decimal balance = temp.Balance;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-goog-api-key", apiKey);

                // Tạo prompt chi tiết
                string prompt = $@"Bạn là một chuyên gia lập kế hoạch tài chính. Người dùng muốn đạt được mục tiêu: '{goal}'. 
                Dữ liệu tài chính của người dùng:
                - Số dư hiện tại: {balance} USD
                Hãy tạo một kế hoạch chi tiết để đạt được mục tiêu này, bao gồm:
                - Các bước cụ thể (ví dụ: tiết kiệm, đầu tư, tăng thu nhập).
                - Mốc thời gian cho từng bước.
                - Các lưu ý hoặc rủi ro cần tránh.
                - Định dạng kế hoạch dưới dạng văn bản rõ ràng, có gạch đầu dòng và tiêu đề phụ.
                Lưu ý: Đưa ra các giả định hợp lý về thu nhập hoặc chi phí nếu cần, và giải thích rõ ràng.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = prompt
                                }
                            }
                        }
                    }
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                return responseObject.candidates[0].content.parts[0].text.ToString();
            }
        }

        private void DownloadPdfButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(generatedPlan))
            {
                MessageBox.Show("Không có kế hoạch để tải!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"KeHoach_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToPDF(saveDialog.FileName);
                    MessageBox.Show($"PDF đã được lưu tại: {saveDialog.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToPDF(string filePath)
        {
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Tiêu đề
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            Paragraph title = new Paragraph("Kế Hoạch Đạt Mục Tiêu", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            document.Add(title);

            // Ngày tạo
            Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
            Paragraph dateInfo = new Paragraph($"Ngày tạo: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateFont);
            dateInfo.Alignment = Element.ALIGN_RIGHT;
            dateInfo.SpacingAfter = 20f;
            document.Add(dateInfo);

            // Mục tiêu
            Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            Paragraph goalTitle = new Paragraph("Mục Tiêu", sectionFont);
            goalTitle.SpacingAfter = 10f;
            document.Add(goalTitle);

            Font contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
            Paragraph goalContent = new Paragraph(GoalInputTextBox.Text, contentFont);
            goalContent.SpacingAfter = 20f;
            document.Add(goalContent);

            // Kế hoạch chi tiết
            Paragraph planTitle = new Paragraph("Kế Hoạch Chi Tiết", sectionFont);
            planTitle.SpacingAfter = 10f;
            document.Add(planTitle);

            Paragraph planContent = new Paragraph(generatedPlan, contentFont);
            planContent.SpacingAfter = 20f;
            document.Add(planContent);

            // Chân trang
            Paragraph footer = new Paragraph($"Báo cáo được tạo bởi Bank Management System - {DateTime.Now:yyyy}", dateFont);
            footer.Alignment = Element.ALIGN_CENTER;
            footer.SpacingBefore = 30f;
            document.Add(footer);

            document.Close();
        }
    }
}