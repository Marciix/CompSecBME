using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffShop.Entities
{
    [Table("purchases")]
    public class Purchase
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("purchased_at")]
        [Required]
        public DateTime PurchasedAt { get; set; }
        
        [Column("user_id")]
        [ForeignKey("User")]
        [Required]
        public long UserId { get; set; }
        
        [Column("caff_item_id")]
        [ForeignKey("CaffItem")]
        [Required]
        public long CaffItemId { get; set; }
        
        public User User { get; set; }
        public CaffItem CaffItem { get; set; }
    }
}