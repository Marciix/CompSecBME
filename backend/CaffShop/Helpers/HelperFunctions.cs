using System;
using System.Security.Cryptography;

namespace CaffShop.Helpers
{
    public static class HelperFunctions
    {
        public static string GenerateRandomString(int length, string allowableChars = null)
        {
            if (string.IsNullOrEmpty(allowableChars))
                allowableChars = @"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Generate random data
            var rnd = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(rnd);

            // Generate the output string
            var allowable = allowableChars.ToCharArray();
            var l = allowable.Length;
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = allowable[rnd[i] % l];

            return new string(chars);
        }

        public static long GetUnixTimestamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            return Convert.ToInt64(ts.TotalSeconds);
        }

    }
}