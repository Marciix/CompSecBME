using CaffShop.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaffShop
{
    public class CaffShopContext : DbContext
    {
        
        public CaffShopContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<CaffItem> CaffItems;
        public DbSet<Comment> Comments;
        public DbSet<Purchase> Purchases;
        public DbSet<User> Users;

    }
}