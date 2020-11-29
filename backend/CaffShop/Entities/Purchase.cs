using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CaffShop.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Purchase
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        public DateTime PurchasedAt { get; set; }
        
        [ForeignKey(nameof(User))]
        [Required]
        public long UserId { get; set; }
        
        [ForeignKey(nameof(CaffItem))]
        [Required]
        public long CaffItemId { get; set; }
    }
}