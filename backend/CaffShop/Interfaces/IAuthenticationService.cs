using System.Threading.Tasks;
using CaffShop.Entities;

namespace CaffShop.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<User> Authenticate(string username, string password);
        public Task<bool> IsUserAbleToLogin(string username);
    }
}