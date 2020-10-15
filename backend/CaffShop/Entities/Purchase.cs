using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffShop.Entities
{
    public class Purchase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("purchased_at")]
        [Required]
        public DateTime PurchasedAt { get; set; }
        
        [Column("user_id")]
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        
        [Column("caff_item_id")]
        [ForeignKey("CaffItem")]
        [Required]
        public int CaffItemId { get; set; }
        
        public User User { get; set; }
        public CaffItem CaffItem { get; set; }
    }
}