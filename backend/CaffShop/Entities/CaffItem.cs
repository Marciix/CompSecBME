using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffShop.Entities
{
    [Table("caff_items")]
    public class CaffItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; }

        public string PreviewPath { get; set; }


        public User Owner { get; set; }
    }
}