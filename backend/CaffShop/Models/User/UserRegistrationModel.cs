using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CaffShop.Models.User
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class UserRegistrationModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}