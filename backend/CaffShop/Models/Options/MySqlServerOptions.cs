using System.Diagnostics.CodeAnalysis;

namespace CaffShop.Models.Options
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class MySqlServerOptions
    {
        public const string OptionsName = "MySqlServerOptions";
        public string Host { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        public bool DoMigration { get; set; }

        public string ConnectionString => $"server={Host};port={Port};database={Name};user={User};password={Pass}";
    }

}