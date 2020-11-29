using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CaffShop.Models.Options
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class JwtOptions
    {
        public const string OptionsName = "JwtOptions";
        private string _secretString;

        [Required]
        // ReSharper disable once UnusedMember.Global
        public string Secret
        {
            get => _secretString;
            set
            {
                _secretString = value;
                SecretBytes = Encoding.ASCII.GetBytes(value);
            }
        }

        [Required]
        public string Issuer { get; set; }

        [Required]
        public long LifeTimeMinutes { get; set; }

        public string[] Audiences { get; set; } = new string[0];

        public bool RequireHttpsMetadata { get; set; } = true;

        public byte[] SecretBytes { get; private set; } = new byte[0];
    }
}