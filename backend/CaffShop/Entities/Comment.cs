using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CaffShop.Entities
{
    [Table("comments")]
    public class Comment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("content")]
        [Required]
        public string Content { get; set; }
        
        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }
        
        [Column("author_id")]
        [ForeignKey("Author")]
        [Required]
        public long AuthorId { get; set; }
        
        [Column("caff_item_id")]
        [ForeignKey("CaffItem")]
        [Required]
        public long CaffItemId { get; set; }
        
        public User Author { get; set; }
    }
}