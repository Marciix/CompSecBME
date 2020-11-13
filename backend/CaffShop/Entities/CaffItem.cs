using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CaffShop.Entities
{
    public class CaffItem
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        [JsonIgnore]
        public string TagsText { get; set; }
        
        [Required]
        [ForeignKey(nameof(Owner))]
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
        
        
        public User Owner { get; set; }
        
        [NotMapped]
        public List<string> Tags
        {
            get => JsonConvert.DeserializeObject<List<string>>(TagsText);
            set => TagsText = JsonConvert.SerializeObject(value);
        }
        
    }
}