using BankBusinessLayer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace BankManagementSystem.WPF.Views
{
    public partial class RevenueView : UserControl
    {
        public SeriesCollection DailyRevenueSeries { get; set; }
        public SeriesCollection MonthlyRevenueSeries { get; set; }
        public SeriesCollection SummarySeries { get; set; }
        public string[] DateLabels { get; set; }
        public string[] PeriodLabels { get; set; }
        public Func<double, string> CurrencyFormatter { get; set; }

        private List<RevenueChartData> currentDailyData;
        private List<MonthlyRevenueData> currentMonthlyData;
        private DataTable currentTopAccountsData;
        private string aiAnalysisResult;
        private bool isAnalyzing;

        public RevenueView()
        {
            InitializeComponent();
            InitializeChart();
            InitializeDateFilters();
            LoadRevenueData();
            DataContext = this;
            aiAnalysisResult = string.Empty;
            isAnalyzing = false;
        }

        private void InitializeChart()
        {
            CurrencyFormatter = value => value.ToString("C", CultureInfo.CurrentCulture);

            DailyRevenueSeries = new SeriesCollection();
            MonthlyRevenueSeries = new SeriesCollection();
            SummarySeries = new SeriesCollection();

            dailyRevenueChart.Series = DailyRevenueSeries;
            monthlyRevenueChart.Series = MonthlyRevenueSeries;
            summaryChart.Series = SummarySeries;
        }

        private void InitializeDateFilters()
        {
            // Initialize date pickers
            dpEndDate.SelectedDate = DateTime.Now;
            dpStartDate.SelectedDate = DateTime.Now.AddDays(-30);

            // Initialize year combo box
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = DateTime.Now.Year;
        }

        private void LoadRevenueData()
        {
            try
            {
                LoadStatistics();
                LoadDailyRevenueChart();
                LoadMonthlyRevenueChart();
                LoadTopAccountsData();
                LoadSummaryChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading revenue data: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatistics()
        {
            // Total Revenue
            decimal totalRevenue = Revenue.GetTotalRevenue();
            txtTotalRevenue.Text = totalRevenue.ToString("C", CultureInfo.CurrentCulture);

            // Today's Revenue
            DateTime today = DateTime.Today;
            DataTable todayData = Revenue.GetRevenueByDate(today, today);
            decimal todayRevenue = 0;
            if (todayData.Rows.Count > 0)
            {
                todayRevenue = Convert.ToDecimal(todayData.Rows[0]["Revenue"]);
            }
            txtTodayRevenue.Text = todayRevenue.ToString("C", CultureInfo.CurrentCulture);

            // This Month's Revenue
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            DataTable monthData = Revenue.GetRevenueByDate(startOfMonth, endOfMonth);
            decimal monthRevenue = 0;
            if (monthData.Rows.Count > 0)
            {
                monthRevenue = monthData.AsEnumerable().Sum(row => Convert.ToDecimal(row["Revenue"]));
            }
            txtMonthRevenue.Text = monthRevenue.ToString("C", CultureInfo.CurrentCulture);

            // Daily Average (last 30 days)
            DateTime last30Days = DateTime.Now.AddDays(-30);
            DataTable avgData = Revenue.GetRevenueByDate(last30Days, DateTime.Now);
            decimal avgDaily = 0;
            if (avgData.Rows.Count > 0)
            {
                decimal total30Days = avgData.AsEnumerable().Sum(row => Convert.ToDecimal(row["Revenue"]));
                avgDaily = total30Days / 30;
            }
            txtAvgDaily.Text = avgDaily.ToString("C", CultureInfo.CurrentCulture);
        }

        private void LoadDailyRevenueChart()
        {
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Now.AddDays(-30);
            DateTime endDate = dpEndDate.SelectedDate ?? DateTime.Now;

            currentDailyData = Revenue.GetRevenueChartData(startDate, endDate);

            var lineSeries = new LineSeries
            {
                Title = "Daily Revenue",
                Values = new ChartValues<decimal>(currentDailyData.Select(x => x.Revenue)),
                Stroke = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                Fill = new SolidColorBrush(Color.FromArgb(50, 52, 152, 219)),
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 8
            };

            DailyRevenueSeries.Clear();
            DailyRevenueSeries.Add(lineSeries);

            DateLabels = currentDailyData.Select(x => x.Date.ToString("MM/dd")).ToArray();
        }

        private void LoadMonthlyRevenueChart()
        {
            int selectedYear = (int)(cmbYear.SelectedItem ?? DateTime.Now.Year);
            currentMonthlyData = Revenue.GetMonthlyRevenueData(selectedYear);

            MonthlyRevenueSeries.Clear();

            if (currentMonthlyData.Any())
            {
                foreach (var data in currentMonthlyData)
                {
                    MonthlyRevenueSeries.Add(new PieSeries
                    {
                        Title = data.MonthName,
                        Values = new ChartValues<decimal> { data.Revenue },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{data.MonthName}: {chartPoint.Y:C}"
                    });
                }
            }
        }

        private void LoadTopAccountsData()
        {
            currentTopAccountsData = Revenue.GetTopRevenueAccounts(10);
            dgTopAccounts.ItemsSource = currentTopAccountsData.DefaultView;
        }

        private void LoadSummaryChart()
        {
            // Create summary data for last 6 months
            List<decimal> summaryValues = new List<decimal>();
            List<string> summaryLabels = new List<string>();

            for (int i = 5; i >= 0; i--)
            {
                DateTime monthStart = DateTime.Now.AddMonths(-i).AddDays(1 - DateTime.Now.AddMonths(-i).Day);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                DataTable monthData = Revenue.GetRevenueByDate(monthStart, monthEnd);
                decimal monthRevenue = 0;
                if (monthData.Rows.Count > 0)
                {
                    monthRevenue = monthData.AsEnumerable().Sum(row => Convert.ToDecimal(row["Revenue"]));
                }

                summaryValues.Add(monthRevenue);
                summaryLabels.Add(monthStart.ToString("MMM yyyy"));
            }

            var columnSeries = new ColumnSeries
            {
                Title = "Monthly Revenue",
                Values = new ChartValues<decimal>(summaryValues),
                Fill = new SolidColorBrush(Color.FromRgb(155, 89, 182))
            };

            SummarySeries.Clear();
            SummarySeries.Add(columnSeries);

            PeriodLabels = summaryLabels.ToArray();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                LoadDailyRevenueChart();
            }
        }

        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbYear.SelectedItem != null)
            {
                LoadMonthlyRevenueChart();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRevenueData();
            MessageBox.Show("Data refreshed successfully!", "Success",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Revenue_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToPDF(saveDialog.FileName);
                    MessageBox.Show("PDF exported successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAIAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (isAnalyzing)
                return;

            isAnalyzing = true;
            aiAnalysisContent.Text = "Analyzing... Please wait.";
            aiAnalysisPanel.Visibility = Visibility.Visible;
            btnExportAIAnalysis.IsEnabled = false;

            try
            {
                // Prepare data for AI analysis
                var analysisData = new
                {
                    TotalRevenue = txtTotalRevenue.Text,
                    TodayRevenue = txtTodayRevenue.Text,
                    MonthRevenue = txtMonthRevenue.Text,
                    DailyAverage = txtAvgDaily.Text,
                    DailyData = currentDailyData.Select(d => new { d.Date, d.Revenue }).ToList(),
                    MonthlyData = currentMonthlyData.Select(m => new { m.MonthName, m.Revenue }).ToList(),
                    TopAccounts = currentTopAccountsData.AsEnumerable().Select(row => new
                    {
                        AccountNumber = row["AccountNumber"].ToString(),
                        Revenue = Convert.ToDecimal(row["Revenue"]),
                        TransferCount = Convert.ToInt32(row["TransferCount"])
                    }).ToList()
                };

                string jsonData = JsonConvert.SerializeObject(analysisData);

                // Call Gemini API
                string apiKey = "YOUR_GEMINI_API_KEY"; // Replace with actual API key
                string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-goog-api-key", $"{apiKey}");
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
                                        text = $"Analyze this bank revenue data and provide insights:\n{jsonData}"
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
                    aiAnalysisResult = responseObject.candidates[0].content.parts[0].text.ToString();

                    aiAnalysisContent.Text = aiAnalysisResult;
                    btnExportAIAnalysis.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                aiAnalysisContent.Text = $"Error during AI analysis: {ex.Message}";
            }
            finally
            {
                isAnalyzing = false;
            }
        }

        private void btnExportAIAnalysis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"AI_Analysis_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportAIAnalysisToPDF(saveDialog.FileName);
                    MessageBox.Show("AI Analysis PDF exported successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting AI Analysis to PDF: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToPDF(string filePath)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Title
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            Paragraph title = new Paragraph("Revenue Analytics Report", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            document.Add(title);

            // Report Date
            Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
            Paragraph dateInfo = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateFont);
            dateInfo.Alignment = Element.ALIGN_RIGHT;
            dateInfo.SpacingAfter = 20f;
            document.Add(dateInfo);

            // Statistics Section
            Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            Paragraph statsTitle = new Paragraph("Revenue Statistics", sectionFont);
            statsTitle.SpacingAfter = 10f;
            document.Add(statsTitle);

            // Statistics Table
            PdfPTable statsTable = new PdfPTable(4);
            statsTable.WidthPercentage = 100;
            statsTable.SpacingAfter = 20f;

            // Table Headers
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            PdfPCell[] headerCells = {
                new PdfPCell(new Phrase("Total Revenue", headerFont)) { BackgroundColor = new BaseColor(52, 73, 94), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 },
                new PdfPCell(new Phrase("Today's Revenue", headerFont)) { BackgroundColor = new BaseColor(52, 73, 94), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 },
                new PdfPCell(new Phrase("This Month", headerFont)) { BackgroundColor = new BaseColor(52, 73, 94), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 },
                new PdfPCell(new Phrase("Daily Average", headerFont)) { BackgroundColor = new BaseColor(52, 73, 94), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 }
            };

            foreach (var cell in headerCells)
                statsTable.AddCell(cell);

            // Table Data
            Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            PdfPCell[] dataCells = {
                new PdfPCell(new Phrase(txtTotalRevenue.Text, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 },
                new PdfPCell(new Phrase(txtTodayRevenue.Text, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 },
                new PdfPCell(new Phrase(txtMonthRevenue.Text, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 },
                new PdfPCell(new Phrase(txtAvgDaily.Text, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 }
            };

            foreach (var cell in dataCells)
                statsTable.AddCell(cell);

            document.Add(statsTable);

            // Daily Revenue Data Section
            if (currentDailyData != null && currentDailyData.Any())
            {
                Paragraph dailyTitle = new Paragraph("Daily Revenue Data", sectionFont);
                dailyTitle.SpacingAfter = 10f;
                document.Add(dailyTitle);

                PdfPTable dailyTable = new PdfPTable(2);
                dailyTable.WidthPercentage = 60;
                dailyTable.SpacingAfter = 20f;

                // Headers
                dailyTable.AddCell(new PdfPCell(new Phrase("Date", headerFont))
                { BackgroundColor = new BaseColor(52, 152, 219), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });
                dailyTable.AddCell(new PdfPCell(new Phrase("Revenue", headerFont))
                { BackgroundColor = new BaseColor(52, 152, 219), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });

                // Data
                foreach (var data in currentDailyData.Take(10)) // Show only top 10 for PDF
                {
                    dailyTable.AddCell(new PdfPCell(new Phrase(data.Date.ToString("yyyy-MM-dd"), dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                    dailyTable.AddCell(new PdfPCell(new Phrase(data.Revenue.ToString("C"), dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                }

                document.Add(dailyTable);
            }

            // Top Revenue Accounts Section
            if (currentTopAccountsData != null && currentTopAccountsData.Rows.Count > 0)
            {
                Paragraph topAccountsTitle = new Paragraph("Top Revenue Generating Accounts", sectionFont);
                topAccountsTitle.SpacingAfter = 10f;
                document.Add(topAccountsTitle);

                PdfPTable topAccountsTable = new PdfPTable(3);
                topAccountsTable.WidthPercentage = 80;
                topAccountsTable.SpacingAfter = 20f;

                // Headers
                topAccountsTable.AddCell(new PdfPCell(new Phrase("Account Number", headerFont))
                { BackgroundColor = new BaseColor(39, 174, 96), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });
                topAccountsTable.AddCell(new PdfPCell(new Phrase("Revenue", headerFont))
                { BackgroundColor = new BaseColor(39, 174, 96), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });
                topAccountsTable.AddCell(new PdfPCell(new Phrase("Transfer Count", headerFont))
                { BackgroundColor = new BaseColor(39, 174, 96), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });

                // Data
                foreach (DataRow row in currentTopAccountsData.Rows)
                {
                    topAccountsTable.AddCell(new PdfPCell(new Phrase(row["AccountNumber"].ToString(), dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                    topAccountsTable.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(row["Revenue"]).ToString("C"), dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                    topAccountsTable.AddCell(new PdfPCell(new Phrase(row["TransferCount"].ToString(), dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                }

                document.Add(topAccountsTable);
            }

            // Monthly Revenue Summary
            if (currentMonthlyData != null && currentMonthlyData.Any())
            {
                Paragraph monthlyTitle = new Paragraph("Monthly Revenue Summary", sectionFont);
                monthlyTitle.SpacingAfter = 10f;
                document.Add(monthlyTitle);

                PdfPTable monthlyTable = new PdfPTable(2);
                monthlyTable.WidthPercentage = 60;
                monthlyTable.SpacingAfter = 20f;

                // Headers
                monthlyTable.AddCell(new PdfPCell(new Phrase("Month", headerFont))
                { BackgroundColor = new BaseColor(243, 156, 18), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });
                monthlyTable.AddCell(new PdfPCell(new Phrase("Revenue", headerFont))
                { BackgroundColor = new BaseColor(243, 156, 18), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });

                // Data
                foreach (var data in currentMonthlyData)
                {
                    monthlyTable.AddCell(new PdfPCell(new Phrase(data.MonthName, dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                    monthlyTable.AddCell(new PdfPCell(new Phrase(data.Revenue.ToString("C"), dataFont))
                    { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 6 });
                }

                document.Add(monthlyTable);
            }

            // Footer
            Paragraph footer = new Paragraph($"Report generated by Bank Management System - {DateTime.Now:yyyy}", dateFont);
            footer.Alignment = Element.ALIGN_CENTER;
            footer.SpacingBefore = 30f;
            document.Add(footer);

            document.Close();
        }

        private void ExportAIAnalysisToPDF(string filePath)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Title
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            Paragraph title = new Paragraph("AI Revenue Analysis Report", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            document.Add(title);

            // Report Date
            Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
            Paragraph dateInfo = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", dateFont);
            dateInfo.Alignment = Element.ALIGN_RIGHT;
            dateInfo.SpacingAfter = 20f;
            document.Add(dateInfo);

            // AI Analysis Section
            Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            Paragraph analysisTitle = new Paragraph("AI Analysis Results", sectionFont);
            analysisTitle.SpacingAfter = 10f;
            document.Add(analysisTitle);

            Font analysisFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
            Paragraph analysisContent = new Paragraph(aiAnalysisResult, analysisFont);
            analysisContent.SpacingAfter = 20f;
            document.Add(analysisContent);

            // Footer
            Paragraph footer = new Paragraph($"Report generated by Bank Management System - AI Analysis - {DateTime.Now:yyyy}", dateFont);
            footer.Alignment = Element.ALIGN_CENTER;
            footer.SpacingBefore = 30f;
            document.Add(footer);

            document.Close();
        }
    }
}