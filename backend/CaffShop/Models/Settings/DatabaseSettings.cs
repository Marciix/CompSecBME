using CaffShop.Helpers;

namespace CaffShop.Models.Settings
{
    public class DatabaseSettings
    {
        public string Host { get; private set; }
        public string Port { get; private set; }
        public string Name { get; private set; }
        public string User { get; private set; }
        public string Pass { get; private set; }

        public string GetConnectionString()
        {
            return $"server={Host};port={Port};database={Name};user={User};password={Pass}";
        }

        public static DatabaseSettings GetFromEnvironment()
        {
            return new DatabaseSettings
            {
                Host = HelperFunctions.GetEnvironmentValueOrException("DB_HOST"),
                Port = HelperFunctions.GetEnvironmentValueOrException("DB_PORT"),
                Name = HelperFunctions.GetEnvironmentValueOrException("DB_NAME"),
                User = HelperFunctions.GetEnvironmentValueOrException("DB_USER"),
                Pass = HelperFunctions.GetEnvironmentValueOrException("DB_PASS")
            };
        }
    }
}