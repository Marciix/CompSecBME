using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffShop.Entities
{
    public class Comment
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey(nameof(Author))]
        [Required]
        public long AuthorId { get; set; }
        
        [ForeignKey(nameof(CaffItem))]
        [Required]
        public long CaffItemId { get; set; }
        
        public User Author { get; set; }
    }
}