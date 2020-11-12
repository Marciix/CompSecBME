using System.ComponentModel.DataAnnotations;

namespace CaffShop.Models.CaffItem
{
    public class CaffItemCreation
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}