using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UserCRUD
{
    // Định nghĩa class User
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public int Permission { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Program
    {
        // Chuỗi kết nối MySQL - thay đổi cho phù hợp
        private static readonly IUserService _userService = new UserService("server=localhost;database=BankDB;user=root;password=senti123;");

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n===== MENU QUẢN LÝ NGƯỜI DÙNG =====");
                Console.WriteLine("1. Thêm người dùng");
                Console.WriteLine("2. Xem danh sách người dùng");
                Console.WriteLine("3. Sửa người dùng");
                Console.WriteLine("4. Xóa người dùng");
                Console.WriteLine("5. Thoát");
                Console.Write("Chọn chức năng (1-5): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddUser(); break;
                    case "2": ListUsers(); break;
                    case "3": UpdateUser(); break;
                    case "4": DeleteUser(); break;
                    case "5": return;
                    default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
                }
            }
        }

        // Hàm thêm người dùng mới
        static void AddUser()
        {
            User user = new User();
            Console.WriteLine("\n=== Thêm người dùng mới ===");
            user.Username = PromptValid("Username", ValidateUsername);
            user.Password = PromptValid("Password", ValidatePassword);
            user.FirstName = PromptValid("First Name", s => !string.IsNullOrWhiteSpace(s));
            user.LastName = PromptValid("Last Name", s => !string.IsNullOrWhiteSpace(s));
            user.Email = PromptValid("Email", ValidateEmail);
            user.Phone = PromptValid("Phone", ValidatePhone);
            user.Address = PromptValid("Address", s => !string.IsNullOrWhiteSpace(s));
            user.Role = PromptValid("Role", s => !string.IsNullOrWhiteSpace(s));
            user.Permission = int.Parse(PromptValid("Permission (số nguyên)", s => int.TryParse(s, out _)));
            user.IsActive = true;
            user.CreatedDate = DateTime.Now;

            ShowUser(user);
            if (!Confirm("Bạn có chắc chắn muốn lưu không? (y/n): ")) return;

            try
            {
                _userService.AddUser(user);
                Console.WriteLine("Thêm người dùng thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        // Hàm hiển thị danh sách người dùng
        static void ListUsers()
        {
            Console.WriteLine("\n=== Danh sách người dùng ===");
            try
            {
                var users = _userService.GetAllUsers();
                Console.WriteLine("ID | Username | Name | Email | Phone | Role | Active | CreatedDate");
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.UserID,2} | {user.Username,-10} | {user.FirstName} {user.LastName,-15} | {user.Email,-20} | {user.Phone,-11} | {user.Role,-8} | {(user.IsActive ? "Yes" : "No"),-6} | {user.CreatedDate}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        // Hàm cập nhật người dùng
        static void UpdateUser()
        {
            Console.Write("\nNhập UserID cần sửa: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("UserID không hợp lệ!");
                return;
            }

            User user = _userService.GetUserById(userId);
            if (user == null)
            {
                Console.WriteLine("Không tìm thấy người dùng!");
                return;
            }

            Console.WriteLine("Thông tin hiện tại:");
            ShowUser(user);

            user.Username = PromptUpdate("Username", user.Username, ValidateUsername);
            user.Password = PromptUpdate("Password", user.Password, ValidatePassword);
            user.FirstName = PromptUpdate("First Name", user.FirstName, s => !string.IsNullOrWhiteSpace(s));
            user.LastName = PromptUpdate("Last Name", user.LastName, s => !string.IsNullOrWhiteSpace(s));
            user.Email = PromptUpdate("Email", user.Email, ValidateEmail);
            user.Phone = PromptUpdate("Phone", user.Phone, ValidatePhone);
            user.Address = PromptUpdate("Address", user.Address, s => !string.IsNullOrWhiteSpace(s));
            user.Role = PromptUpdate("Role", user.Role, s => !string.IsNullOrWhiteSpace(s));
            user.Permission = int.Parse(PromptUpdate("Permission (số nguyên)", user.Permission.ToString(), s => int.TryParse(s, out _)));
            user.IsActive = Confirm("Kích hoạt người dùng? (y/n): ");

            ShowUser(user);
            if (!Confirm("Bạn có chắc chắn muốn lưu không? (y/n): ")) return;

            try
            {
                _userService.UpdateUser(user);
                Console.WriteLine("Cập nhật thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        // Hàm xóa người dùng
        static void DeleteUser()
        {
            Console.Write("\nNhập UserID cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("UserID không hợp lệ!");
                return;
            }

            User user = _userService.GetUserById(userId);
            if (user == null)
            {
                Console.WriteLine("Không tìm thấy người dùng!");
                return;
            }

            ShowUser(user);
            if (!Confirm("Bạn có chắc chắn muốn xóa không? (y/n): ")) return;

            try
            {
                _userService.DeleteUser(userId);
                Console.WriteLine("Xóa thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        // Hàm hiển thị thông tin user
        static void ShowUser(User user)
        {
            Console.WriteLine("\n--- Thông tin người dùng ---");
            Console.WriteLine($"UserID: {user.UserID}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Password: {user.Password}");
            Console.WriteLine($"Họ tên: {user.FirstName} {user.LastName}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Phone: {user.Phone}");
            Console.WriteLine($"Address: {user.Address}");
            Console.WriteLine($"Role: {user.Role}");
            Console.WriteLine($"Permission: {user.Permission}");
            Console.WriteLine($"IsActive: {(user.IsActive ? "Yes" : "No")}");
            Console.WriteLine($"CreatedDate: {user.CreatedDate}");
        }

        // Hàm xác nhận yes/no
        static bool Confirm(string message)
        {
            Console.Write(message);
            string ans = Console.ReadLine();
            return ans.Equals("y", StringComparison.OrdinalIgnoreCase);
        }

        // Hàm nhập và kiểm tra hợp lệ
        static string PromptValid(string field, Func<string, bool> validate)
        {
            string input;
            do
            {
                Console.Write($"{field}: ");
                input = Console.ReadLine();
                if (!validate(input))
                    Console.WriteLine($"Giá trị không hợp lệ cho {field}!");
            } while (!validate(input));
            return input;
        }

        // Hàm nhập lại khi update (giữ nguyên nếu bấm Enter)
        static string PromptUpdate(string field, string oldValue, Func<string, bool> validate)
        {
            Console.Write($"{field} ({oldValue}): ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return oldValue;
            while (!validate(input))
            {
                Console.WriteLine($"Giá trị không hợp lệ cho {field}!");
                Console.Write($"{field} ({oldValue}): ");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return oldValue;
            }
            return input;
        }

        // Các hàm kiểm tra hợp lệ
        static bool ValidateUsername(string s) => !string.IsNullOrWhiteSpace(s) && s.Length <= 50;
        static bool ValidatePassword(string s) => !string.IsNullOrWhiteSpace(s) && s.Length >= 4;
        static bool ValidateEmail(string s) => Regex.IsMatch(s ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        static bool ValidatePhone(string s) => Regex.IsMatch(s ?? "", @"^\d{10,11}$");
    }
}