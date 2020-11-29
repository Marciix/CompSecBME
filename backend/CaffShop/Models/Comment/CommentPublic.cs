using System;
using CaffShop.Models.User;

namespace CaffShop.Models.Comment
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