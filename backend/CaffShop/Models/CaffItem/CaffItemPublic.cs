using System;
using System.Collections.Generic;
using CaffShop.Models.User;

namespace CaffShop.Models.CaffItem
{
    public class CaffItemPublic
    {
        public long Id { get; set; }
        public string Title { get; set; }
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<string> Tags { get; set; }
        public long OwnerId { get; set; }
        public DateTime UploadedAt { get; set; }
        public UserPublic Owner { get; set; }
    }
}