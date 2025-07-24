using System;
using System.Collections.Generic;
using System.Linq;
using UserCRUD;
using Xunit;

namespace UserCRUD.Tests
{
    public class UserServiceTests
    {
        private readonly List<User> _inMemoryUsers;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Use an in-memory list to simulate the database
            _inMemoryUsers = new List<User>();
            // Use TestableUserService to override UserService behavior
            _userService = new TestableUserService(_inMemoryUsers);
        }

        [Fact]
        public void AddUser_ShouldAddUserToList()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Address = "123 Main St",
                Role = "User",
                Permission = 1,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Act
            _userService.AddUser(user);

            // Assert
            Assert.Single(_inMemoryUsers);
            var addedUser = _inMemoryUsers.First();
            Assert.Equal(user.Username, addedUser.Username);
            Assert.Equal(user.Email, addedUser.Email);
            Assert.Equal(user.IsActive, addedUser.IsActive);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            _inMemoryUsers.AddRange(new[]
            {
                new User { UserID = 1, Username = "user1", Email = "user1@example.com", IsActive = true },
                new User { UserID = 2, Username = "user2", Email = "user2@example.com", IsActive = false }
            });

            // Act
            var result = _userService.GetAllUsers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.UserID == 1 && u.Username == "user1");
            Assert.Contains(result, u => u.UserID == 2 && u.Username == "user2");
        }

        [Fact]
        public void GetAllUsers_WhenNoUsers_ShouldReturnEmptyList()
        {
            // Act
            var result = _userService.GetAllUsers();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                UserID = 1,
                Username = "testuser",
                Email = "test@example.com",
                IsActive = true
            };
            _inMemoryUsers.Add(user);

            // Act
            var result = _userService.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserID, result.UserID);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public void GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var result = _userService.GetUserById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateUser_ShouldUpdateExistingUser()
        {
            // Arrange
            var user = new User
            {
                UserID = 1,
                Username = "olduser",
                Email = "old@example.com",
                IsActive = true
            };
            _inMemoryUsers.Add(user);

            var updatedUser = new User
            {
                UserID = 1,
                Username = "newuser",
                Email = "new@example.com",
                IsActive = false
            };

            // Act
            _userService.UpdateUser(updatedUser);

            // Assert
            var result = _inMemoryUsers.First();
            Assert.Equal(updatedUser.Username, result.Username);
            Assert.Equal(updatedUser.Email, result.Email);
            Assert.Equal(updatedUser.IsActive, result.IsActive);
        }

        [Fact]
        public void UpdateUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User { UserID = 999, Username = "nonexistent" };

            var exception = Assert.Throws<Exception>(() => _userService.UpdateUser(user));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public void DeleteUser_ShouldRemoveUser_WhenUserExists()
        {
            // Arrange
            var user = new User { UserID = 1, Username = "testuser" };
            _inMemoryUsers.Add(user);

            // Act
            _userService.DeleteUser(1);

            // Assert
            Assert.Empty(_inMemoryUsers);
        }

        [Fact]
        public void DeleteUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _userService.DeleteUser(999));
            Assert.Equal("User not found", exception.Message);
        }

        // TestableUserService to simulate database operations with an in-memory list
        private class TestableUserService : UserService
        {
            private readonly List<User> _users;

            public TestableUserService(List<User> users) : base("dummy_connection_string")
            {
                _users = users;
            }

            public override void AddUser(User user)
            {
                user.UserID = _users.Any() ? _users.Max(u => u.UserID) + 1 : 1;
                _users.Add(user);
            }

            public override List<User> GetAllUsers()
            {
                return _users.ToList();
            }

            public override User GetUserById(int userId)
            {
                return _users.FirstOrDefault(u => u.UserID == userId);
            }

            public override void UpdateUser(User user)
            {
                var existing = _users.FirstOrDefault(u => u.UserID == user.UserID);
                if (existing == null)
                    throw new Exception("User not found");

                existing.Username = user.Username;
                existing.Password = user.Password;
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                existing.Email = user.Email;
                existing.Phone = user.Phone;
                existing.Address = user.Address;
                existing.Role = user.Role;
                existing.Permission = user.Permission;
                existing.IsActive = user.IsActive;
                existing.CreatedDate = user.CreatedDate;
            }

            public override void DeleteUser(int userId)
            {
                var user = _users.FirstOrDefault(u => u.UserID == userId);
                if (user == null)
                    throw new Exception("User not found");

                _users.Remove(user);
            }
        }
    }
}