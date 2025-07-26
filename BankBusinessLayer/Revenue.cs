using System;
using System.Collections.Generic;
using System.Data;
using BankDataAccessLayer;

namespace BankBusinessLayer
{
    public class Revenue
    {
        public static decimal GetTotalRevenue()
        {
            return RevenueData.GetTotalRevenue();
        }

        public static DataTable GetRevenueByDate(DateTime startDate, DateTime endDate)
        {
            return RevenueData.GetRevenueByDate(startDate, endDate);
        }

        public static DataTable GetRevenueByMonth(int year)
        {
            return RevenueData.GetRevenueByMonth(year);
        }

        public static DataTable GetTopRevenueAccounts(int topCount = 10)
        {
            return RevenueData.GetTopRevenueAccounts(topCount);
        }

        public static List<RevenueChartData> GetRevenueChartData(DateTime startDate, DateTime endDate)
        {
            List<RevenueChartData> chartData = new List<RevenueChartData>();
            DataTable dt = GetRevenueByDate(startDate, endDate);

            foreach (DataRow row in dt.Rows)
            {
                chartData.Add(new RevenueChartData
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    Revenue = Convert.ToDecimal(row["Revenue"])
                });
            }

            return chartData;
        }

        public static List<MonthlyRevenueData> GetMonthlyRevenueData(int year)
        {
            List<MonthlyRevenueData> monthlyData = new List<MonthlyRevenueData>();
            DataTable dt = GetRevenueByMonth(year);

            foreach (DataRow row in dt.Rows)
            {
                monthlyData.Add(new MonthlyRevenueData
                {
                    Month = Convert.ToInt32(row["Month"]),
                    MonthName = row["MonthName"].ToString(),
                    Revenue = Convert.ToDecimal(row["Revenue"])
                });
            }

            return monthlyData;
        }
    }

    public class RevenueChartData
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MonthlyRevenueData
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
    }
}