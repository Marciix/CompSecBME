using System.ComponentModel.DataAnnotations;

namespace CaffShop.Models
{
    public class UserAuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}