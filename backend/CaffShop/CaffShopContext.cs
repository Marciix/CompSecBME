﻿using System.Diagnostics.CodeAnalysis;
using CaffShop.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaffShop
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CaffShopContext : DbContext
    {
        
        public CaffShopContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<CaffItem> CaffItems { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<User> Users  { get; set; }

    }
}