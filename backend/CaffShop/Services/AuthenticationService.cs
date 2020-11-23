using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;
using Microsoft.Extensions.Logging;
using IAuthenticationService = CaffShop.Interfaces.IAuthenticationService;

namespace CaffShop.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IUserService userService, ILogger<AuthenticationService> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _userService.GetUserByUserNameOrEmail(username);

            // check if username exists
            if (user == null)
            {
                _logger.LogWarning($"Failed login attempt for unknown user: {username}");
                throw new LoginFailedException("User does not exists.");
            }

            // Validate password
            if (false == VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning($"Failed login attempt for user: {user.UserName}");
                throw new LoginFailedException("Incorrect password.");
            }

            return user;
        }

        public async Task<bool> IsUserAbleToLogin(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return await _userService.IsUserExistsByUserNameOrMail(username);
        }

        // private helper methods
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));
            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).",
                    nameof(storedHash));

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            // compute entered password's hash to be compared with stored hash
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            // return false if any byte differs
            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        }
    }
}