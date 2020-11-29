using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CaffShop.Models.User
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class UserAuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}