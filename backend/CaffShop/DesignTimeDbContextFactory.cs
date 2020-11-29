using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaffShop
{
    // ReSharper disable once UnusedType.Global
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CaffShopContext>
    {
        public CaffShopContext CreateDbContext(string[] args)
        {
            Console.Write("Enter your connection string for migration: ");
            var connectionString = Console.ReadLine();

            if (string.IsNullOrEmpty(connectionString))
                connectionString = "server=none";

            var builder = new DbContextOptionsBuilder<CaffShopContext>();

            builder.UseMySql(connectionString);
            return new CaffShopContext(builder.Options);
        }
    }
}