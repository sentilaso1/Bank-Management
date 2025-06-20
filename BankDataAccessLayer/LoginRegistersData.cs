using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BankDataAccessLayer
{
    public class LoginRegistersData
    {

        public static DataTable GetAllLoginRegisters()
        {
            DataTable dt = new DataTable();

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Select LoginDateTime, Username, Password, Permissions 
                            from LoginRegisters
                            Join Users on LoginRegisters.UserID = Users.UserID";

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

        public static int AddNewLoginRegister(string Username, DateTime LoginDate)
        {
            int LoginRegisterID = -1;

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Bước 1: Lấy UserID từ Username
                    string getUserIdQuery = "SELECT UserID FROM Users WHERE Username = @Username";
                    using (MySqlCommand getUserIdCmd = new MySqlCommand(getUserIdQuery, connection))
                    {
                        getUserIdCmd.Parameters.AddWithValue("@Username", Username);
                        object userIdObj = getUserIdCmd.ExecuteScalar();

                        if (userIdObj == null)
                            return -1;

                        int userId = Convert.ToInt32(userIdObj);

                        // Bước 2: Insert LoginRegister
                        string insertQuery = @"INSERT INTO LoginRegisters (LoginDateTime, UserID, Username)
                                       VALUES(@LoginDate, @UserID, @Username);
                                       SELECT LAST_INSERT_ID();";

                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@LoginDate", LoginDate);
                            insertCmd.Parameters.AddWithValue("@UserID", userId);
                            insertCmd.Parameters.AddWithValue("@Username", Username);

                            object result = insertCmd.ExecuteScalar();

                            if (result != null && int.TryParse(result.ToString(), out int insertedID))
                            {
                                LoginRegisterID = insertedID;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi thêm login register: " + ex.Message);
                }
            }

            return LoginRegisterID;
        }


        public static int GetTotalLoginRegisters()
        {
            int totalLoginRegisters = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT COUNT(LoginRegisters.LoginID) FROM LoginRegisters";

            MySqlCommand Command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null && int.TryParse(Result.ToString(), out int InsertedTotalLoginRegisters))
                {
                    totalLoginRegisters = InsertedTotalLoginRegisters;
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

            return totalLoginRegisters;
        }
    }
}
