using System.Collections.Generic;
using System.Threading.Tasks;
using CaffShop.Entities;

namespace CaffShop.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserById(long id);
        Task<User> GetUserByUserNameOrEmail(string userNameOrEmail);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> CreateUser(User user, string password);
        Task<User> UpdateUser(User user);
    }
}