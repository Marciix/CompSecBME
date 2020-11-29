using System;
using System.Security.Claims;

namespace CaffShop.Helpers
{
    public static class UserHelper
    {
        public const string RoleUser = "user";
        public const string RoleAdmin = "admin";

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public static long GetAuthenticatedUserId(ClaimsPrincipal user)
        {
            return Convert.ToInt64(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}