namespace CaffShop.Models.Options
{
    public class MySqlServerOptions
    {
        public static readonly string OptionsName = "MySqlServerOptions";
        public string Host { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        public bool DoMigration { get; set; }

        public string ConnectionString => $"server={Host};port={Port};database={Name};user={User};password={Pass}";
    }

}