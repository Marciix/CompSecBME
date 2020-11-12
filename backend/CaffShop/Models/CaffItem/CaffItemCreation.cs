using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaffShop.Models.CaffItem
{
    public class CaffItemCreation
    {
        [Required]
        public string Title { get; set; }
        public List<string> Tags { get; set; }
    }
}