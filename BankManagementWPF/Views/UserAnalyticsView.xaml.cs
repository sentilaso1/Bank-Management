using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using BankBusinessLayer;
using BankDataAccessLayer;

namespace BankManagementSystem.WPF.Views
{
    public partial class UserAnalyticsView : UserControl
    {
        private DataTable _usersTable;
        private DataTable _loginRegistersTable;

        public UserAnalyticsView()
        {
            InitializeComponent();
            LoadData();
            UpdateMetrics();
            SetupCharts();
        }

        private void LoadData()
        {
            _usersTable = User.GetAllUsers();
            _loginRegistersTable = BankDataAccessLayer.LoginRegistersData.GetAllLoginRegisterIncludeAllColumn();
        }

        private void UpdateMetrics()
        {
            if (_usersTable == null || _loginRegistersTable == null)
                return;

            // Tổng số người dùng
            int totalUsers = _usersTable.Rows.Count;
            TotalUsersTextBlock.Text = totalUsers.ToString();

            // Số người dùng đang hoạt động và không hoạt động
            int activeUsers = _usersTable.AsEnumerable().Count(r => r.Field<bool>("IsActive"));
            int inactiveUsers = totalUsers - activeUsers;
            ActiveUsersTextBlock.Text = activeUsers.ToString();
            InactiveUsersTextBlock.Text = inactiveUsers.ToString();

            // Số lần đăng nhập thành công và không thành công
            int successfulLogins = _loginRegistersTable.AsEnumerable().Count(r => r.Field<bool>("IsSuccessful"));
            int failedLogins = _loginRegistersTable.AsEnumerable().Count(r => !r.Field<bool>("IsSuccessful"));
            SuccessfulLoginsTextBlock.Text = successfulLogins.ToString();
            FailedLoginsTextBlock.Text = failedLogins.ToString();
        }

        private void SetupCharts()
        {
            if (_usersTable == null || _loginRegistersTable == null)
                return;

            // Biểu đồ cột: Số lượng người dùng theo vai trò
            var roles = _usersTable.AsEnumerable()
                .GroupBy(r => r.Field<string>("Role"))
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToList();

            var roleSeries = new SeriesCollection();
            foreach (var role in roles)
            {
                roleSeries.Add(new ColumnSeries
                {
                    Title = role.Role,
                    Values = new ChartValues<int> { role.Count },
                    Fill = role.Role == "Admin" ? Brushes.Purple : Brushes.Blue
                });
            }

            RoleChart.Series = roleSeries;
            RoleChart.AxisX.First().Labels = roles.Select(r => r.Role).ToArray();
            RoleChart.AxisY.First().Title = "Number of Users";

            // Biểu đồ tròn: Phân phối trạng thái hoạt động
            int activeUsers = _usersTable.AsEnumerable().Count(r => r.Field<bool>("IsActive"));
            int inactiveUsers = _usersTable.Rows.Count - activeUsers;

            ActivityChart.Series = new SeriesCollection
            {
                new PieSeries { Title = "Active", Values = new ChartValues<int> { activeUsers }, DataLabels = true, Fill = Brushes.Green },
                new PieSeries { Title = "Inactive", Values = new ChartValues<int> { inactiveUsers }, DataLabels = true, Fill = Brushes.Red }
            };
            ActivityChart.LegendLocation = LegendLocation.Right;

            // Biểu đồ đường: Số lần đăng nhập thành công theo ngày (7 ngày gần nhất)
            var loginCounts = _loginRegistersTable.AsEnumerable()
                .Where(r => r.Field<bool>("IsSuccessful"))
                .GroupBy(r => r.Field<DateTime>("LoginDateTime").Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .TakeLast(7) // Lấy 7 ngày gần nhất
                .ToList();

            LoginTrendChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Successful Logins",
                    Values = new ChartValues<int>(loginCounts.Select(g => g.Count)),
                    Stroke = Brushes.Blue,
                    Fill = Brushes.Transparent
                }
            };
            LoginTrendChart.AxisX.First().Labels = loginCounts.Select(g => g.Date.ToString("MMM dd")).ToArray();
            LoginTrendChart.AxisY.First().Title = "Login Count";
        }
    }
}