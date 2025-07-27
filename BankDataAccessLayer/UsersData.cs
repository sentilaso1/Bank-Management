using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BankDataAccessLayer
{
    public class UsersData
    {

        public static bool GetUserByID(int UserID, ref string firstName, ref string lastName, ref string email, ref string phoneNumber, ref string Username, ref string Password, ref string Role, ref int Permission, ref string AccountNumber)
        {
            bool isFound = false;
            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Select FirstName, LastName, Email, Phone, Username, Password, Role, Permission, AccountNumber
                            from Users
                            where UserID = @UserID";

            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    firstName = (string)reader["FirstName"];
                    lastName = (string)reader["LastName"];
                    email = (string)reader["Email"];
                    phoneNumber = (string)reader["Phone"];
                    Password = (string)reader["Password"];
                    Role = (string)reader["Role"];
                    Permission = (int)reader["Permission"];
                    AccountNumber = (string)reader["AccountNumber"];
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

            return isFound;
        }

        public static bool GetUserByUsername(string Username, ref string firstName, ref string lastName, ref string email, ref string phoneNumber, ref string Password, ref string Role, ref int Permission, ref int UserID, ref bool IsActive)
        {
            bool isFound = false;
            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Select FirstName, LastName, Email, Phone, Username, Password, Role, Permission, UserID, IsActive
                            from Users
                            where Username = @Username";

            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", Username);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;


                    firstName = (string)reader["FirstName"];
                    lastName = (string)reader["LastName"];
                    email = (string)reader["Email"];
                    phoneNumber = (string)reader["Phone"];
                    Password = (string)reader["Password"];
                    Role = (string)reader["Role"];
                    Permission = (int)reader["Permission"];
                    UserID = (int)reader["UserID"];
                    IsActive = Convert.ToBoolean(reader["IsActive"]);

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

            return isFound;
        }

        public static bool GetUserByUsernameAndPassword(string Username, string Password, ref string Role, ref int Permission, ref string AccountNumber)
        {
            bool isFound = false;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Select * from Users where Username = @Username AND Password = @Password AND IsActive = 1";

            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@Password", Password);

            try
            {
                connection.Open();

                MySqlDataReader Reader = command.ExecuteReader();

                if (Reader.Read())
                {
                    isFound = true;
                    Role = Reader["Role"].ToString();
                    Permission = (int)Reader["Permission"];
                    AccountNumber = (string)Reader["AccountNumber"];
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

            return isFound;
        }

        public static bool DeleteUser(string Username)
        {
            int RowsAffected = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"delete from Users
                            where Username = @Username";

            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", Username);

            try
            {

                connection.Open();

                RowsAffected = command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.ToString());
            }

            finally
            {

                connection.Close();
            }

            return (RowsAffected > 0);
        }

        public static bool UpdateUser(string Username, string firstName, string lastName, string email, string phoneNumber, string Password, string Role, int Permission)
        {
            int RowsAffected = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update Users
                            Set
                                FirstName = @FirstName,
                                 LastName = @LastName,
                                 Email = @Email,
                                 Phone = @Phone,
                                 Role = @Role,
                                 Permission = @Permission,
                                 Password = @Password
                                    where (Username = @Username)
                            ";

            MySqlCommand Command = new MySqlCommand(query, connection);


            Command.Parameters.AddWithValue("@FirstName", firstName);
            Command.Parameters.AddWithValue("@LastName", lastName);
            Command.Parameters.AddWithValue("@Email", email);
            Command.Parameters.AddWithValue("@Phone", phoneNumber);
            Command.Parameters.AddWithValue("@Username", Username);
            Command.Parameters.AddWithValue("@Role", Role);
            Command.Parameters.AddWithValue("@Permission", Permission);
            Command.Parameters.AddWithValue("@Password", Password);

            try
            {
                connection.Open();

                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return RowsAffected > 0;
        }

        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select FirstName, LastName, Email, Phone , Username, Password, Role, Permission, IsActive from Users";

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
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static bool isUserExist(string Username)
        {

            bool isFound = false;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Select 1 as Found from Users
                                Where (Username = @Username)";

            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", Username);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static int AddNewUser(string Username, string FirstName, string LastName, string Email, string Phone, string Password, string Role, int Permission, string AccountNumber)
        {
            int UserID = -1;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"
                            Insert into Users(FirstName, LastName, Email, Phone, Username, Password, Role, Permission, AccountNumber)
                            Values (@FirstName, @LastName, @Email, @Phone,@Username,@Password,@Role,@Permission, @AccountNumber);
                            Select LAST_INSERT_ID()";

            MySqlCommand Command = new MySqlCommand(query, connection);

            Command.Parameters.AddWithValue("@FirstName", FirstName);
            Command.Parameters.AddWithValue("@LastName", LastName);
            Command.Parameters.AddWithValue("@Email", Email);
            Command.Parameters.AddWithValue("@Phone", Phone);
            Command.Parameters.AddWithValue("@Username", Username);
            Command.Parameters.AddWithValue("@Password", Password);
            Command.Parameters.AddWithValue("@Role", Role);
            Command.Parameters.AddWithValue("@Permission", Permission);
            Command.Parameters.AddWithValue("@AccountNumber", AccountNumber);
            try
            {
                connection.Open();

                object result = Command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {

                    UserID = insertedID;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            finally
            {
                connection.Close();
            }

            return UserID;
        }

        public static int GetTotalUsers()
        {
            int totalUsers = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Select Count(Users.UserID) from Users";

            MySqlCommand Command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null && int.TryParse(Result.ToString(), out int InsertedTotalUsers))
                {
                    totalUsers = InsertedTotalUsers;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            finally
            {
                connection.Close();
            }

            return totalUsers;
        }

        public static bool SetUserActive(string Username, bool isActive)
        {
            int rowsAffected = 0;

            MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE Users SET IsActive = @IsActive WHERE Username = @Username";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IsActive", isActive);
            command.Parameters.AddWithValue("@Username", Username);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return rowsAffected > 0;
        }
    }
}
