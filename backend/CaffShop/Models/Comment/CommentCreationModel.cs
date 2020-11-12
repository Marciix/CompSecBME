using System.ComponentModel.DataAnnotations;

namespace CaffShop.Models
{
    public class CommentCreationModel
    {
        [Required]
        public string Content { get; set; }
    }
}