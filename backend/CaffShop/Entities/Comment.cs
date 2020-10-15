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
        public int Id { get; set; }
        
        [Column("content")]
        [Required]
        public string Content { get; set; }
        
        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }
        
        [Column("author_id")]
        [ForeignKey("Author")]
        [Required]
        public int AuthorId { get; set; }
        
        [Column("caff_item_id")]
        [ForeignKey("CaffItem")]
        [Required]
        public int CaffItemId { get; set; }
        
        public User Author { get; set; }
        public CaffItem CaffItem { get; set; }
    }
}