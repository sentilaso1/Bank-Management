using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace BankDataAccessLayer
{
    public class RevenueData
    {
        public static decimal GetTotalRevenue()
        {
            decimal totalRevenue = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT SUM(Amount * 0.02) as TotalRevenue 
                            FROM TransferLogs 
                            WHERE FromAccountNumber IS NOT NULL 
                            AND ToAccountNumber IS NOT NULL";

            MySqlCommand command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    totalRevenue = Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return totalRevenue;
        }

        public static DataTable GetRevenueByDate(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT DATE(TransferDate) as Date, SUM(Amount * 0.02) as Revenue
                            FROM TransferLogs 
                            WHERE FromAccountNumber IS NOT NULL 
                            AND ToAccountNumber IS NOT NULL
                            AND TransferDate BETWEEN @StartDate AND @EndDate
                            GROUP BY DATE(TransferDate)
                            ORDER BY DATE(TransferDate)";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static DataTable GetRevenueByMonth(int year)
        {
            DataTable dt = new DataTable();

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT MONTH(TransferDate) as Month, 
                                   MONTHNAME(TransferDate) as MonthName,
                                   SUM(Amount * 0.02) as Revenue
                            FROM TransferLogs 
                            WHERE FromAccountNumber IS NOT NULL 
                            AND ToAccountNumber IS NOT NULL
                            AND YEAR(TransferDate) = @Year
                            GROUP BY MONTH(TransferDate), MONTHNAME(TransferDate)
                            ORDER BY MONTH(TransferDate)";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Year", year);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static DataTable GetTopRevenueAccounts(int topCount = 10)
        {
            DataTable dt = new DataTable();

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT FromAccountNumber as AccountNumber, 
                                   SUM(Amount * 0.02) as Revenue,
                                   COUNT(*) as TransferCount
                            FROM TransferLogs 
                            WHERE FromAccountNumber IS NOT NULL 
                            AND ToAccountNumber IS NOT NULL
                            GROUP BY FromAccountNumber
                            ORDER BY Revenue DESC
                            LIMIT @TopCount";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@TopCount", topCount);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }
    }
}