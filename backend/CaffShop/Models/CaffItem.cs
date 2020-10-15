using System.Collections.Generic;
using CaffShop.Entities;

namespace CaffShop.Models
{
    public class CaffItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Owner { get; set; }
        public List<Comment> Comments;
    }
}