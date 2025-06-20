using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace BankDataAccessLayer
{
    public class TransferLogsData
    {

        public static DataTable GetAllTransferLogs()
        {
            DataTable dt = new DataTable();

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT TransferDate, FromAccountNumber, ToAccountNumber, Amount, PerformedBy 
                            FROM TransferLogs";

            MySqlCommand command = new MySqlCommand(query, connection);

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

        public static int AddNewTransferLog(DateTime TransferDate, string FromAccountNumber, string ToAccountNumber, decimal Amount, string PerformedBy)
        {
            int TransferLogID = -1;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"INSERT INTO TransferLogs (TransferDate, FromAccountNumber, ToAccountNumber, Amount, PerformedBy)
                            VALUES(@TransferDate, @FromAccountNumber, @ToAccountNumber, @Amount, @PerformedBy);
                            SELECT LAST_INSERT_ID();";

            MySqlCommand Command = new MySqlCommand(query, connection);

            Command.Parameters.AddWithValue("@TransferDate", TransferDate);
            Command.Parameters.AddWithValue("@FromAccountNumber", FromAccountNumber);
            Command.Parameters.AddWithValue("@ToAccountNumber", ToAccountNumber);
            Command.Parameters.AddWithValue("@Amount", Amount);
            Command.Parameters.AddWithValue("@PerformedBy", PerformedBy);

            try
            {
                connection.Open();

                object result = Command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    TransferLogID = insertedID;
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

            return TransferLogID;
        }

        public static int GetTotalTransferLogs()
        {
            int totalTransferLogs = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT COUNT(TransferLogs.TransferID) FROM TransferLogs";

            MySqlCommand Command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null && int.TryParse(Result.ToString(), out int InsertedTotalTransferLogs))
                {
                    totalTransferLogs = InsertedTotalTransferLogs;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            finally
            {
                connection.Close();
            }

            return totalTransferLogs;
        }
    }
}
