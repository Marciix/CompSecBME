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
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(
                    "Value cannot be empty or whitespace only string.", "password"
                );

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        
        public static long GetAuthenticatedUserId(ClaimsPrincipal user)
        {
            return Convert.ToInt32(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        
        public static bool IsAuthenticatedUserAdmin(ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role) == RoleAdmin;
        }
    }
}