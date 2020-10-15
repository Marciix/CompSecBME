using System;

namespace CaffShop.Helpers
{
    public static class HelperFunctions
    {
        public static string GetEnvironmentValue(string key, string defaultValue = "")
        {
            return Environment.GetEnvironmentVariable(key) ?? defaultValue;
        }

        public static string GetEnvironmentValueOrException(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);

            if(value == null)
                throw new ArgumentException("The environment value not found", key);

            return value;
        }
    }
}