using System;

namespace CaffShop.Models
{
    public class CommentPublic
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public long AuthorId { get; set; }
        public long CaffItemId { get; set; }
        public UserPublic Author { get; set; }
    }
}