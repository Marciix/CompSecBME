using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffShop.Entities
{
    [Table("caff_items")]
    public class CaffItem
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [Required]
        [ForeignKey("Owner")]
        public long OwnerId { get; set; }
        
        [Required]
        public DateTime UploadedAt { get; set; }
        
        // Technical field
        public string PreviewPath { get; set; }
        
        // Technical field
        public string CaffPath { get; set; }
        
        // Technical field
        public string OriginalName { get; set; }
        
        // Technical field
        public string InnerName { get; set; }
        

        // Technical field
        public long CaffFileSize { get; set; }

        public User Owner { get; set; }
    }
}