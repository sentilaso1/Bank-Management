using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCRUD
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public virtual void AddUser(User user)
        {
            using (var conn = GetConnection())
            {
                string sql = @"INSERT INTO Users (Username, Password, FirstName, LastName, Email, Phone, Address, Role, Permission, IsActive, CreatedDate)
                           VALUES (@Username, @Password, @FirstName, @LastName, @Email, @Phone, @Address, @Role, @Permission, @IsActive, @CreatedDate)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);
                cmd.Parameters.AddWithValue("@Address", user.Address);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Permission", user.Permission);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = GetConnection())
            {
                string sql = "SELECT * FROM Users";
                var cmd = new MySqlCommand(sql, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Address = reader["Address"].ToString(),
                            Role = reader["Role"].ToString(),
                            Permission = Convert.ToInt32(reader["Permission"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                        });
                    }
                }
            }
            return users;
        }

        public virtual User GetUserById(int userId)
        {
            using (var conn = GetConnection())
            {
                string sql = "SELECT * FROM Users WHERE UserID=@UserID";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Address = reader["Address"].ToString(),
                            Role = reader["Role"].ToString(),
                            Permission = Convert.ToInt32(reader["Permission"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                        };
                    }
                }
            }
            return null;
        }

        public virtual void UpdateUser(User user)
        {
            using (var conn = GetConnection())
            {
                string sql = @"UPDATE Users SET Username=@Username, Password=@Password, FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone, Address=@Address, Role=@Role, Permission=@Permission, IsActive=@IsActive
                           WHERE UserID=@UserID";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);
                cmd.Parameters.AddWithValue("@Address", user.Address);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Permission", user.Permission);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@UserID", user.UserID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void DeleteUser(int userId)
        {
            using (var conn = GetConnection())
            {
                string sql = "DELETE FROM Users WHERE UserID=@UserID";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}