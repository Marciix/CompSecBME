using System;

namespace CaffShop.Models.CaffItem
{
    public class CaffItemPublic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long OwnerId { get; set; }
        public DateTime UploadedAt { get; set; }
        public UserPublic Owner { get; set; }
    }
}