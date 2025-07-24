using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCRUD
{
    public interface IUserService
    {
        void AddUser(User user);
        List<User> GetAllUsers();
        User GetUserById(int userId);
        void UpdateUser(User user);
        void DeleteUser(int userId);
    }
}
