using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffShop.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        public long Id { get; set; }
        
        public string UserName { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
        
        public bool IsAdmin { get; set; }

    }
}