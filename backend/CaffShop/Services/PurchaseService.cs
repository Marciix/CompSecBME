using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaffShop.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CaffShopContext _context;

        public PurchaseService(CaffShopContext context)
        {
            _context = context;
        }

        public async Task<Purchase> PurchaseItem(int itemId, int userId)
        {
            var purchase = new Purchase
            {
                CaffItemId = itemId,
                UserId = userId,
                PurchasedAt = DateTime.Now
            };

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<List<Purchase>> GetItemPurchases(int itemId)
        {
            return await _context.Purchases.Where(p => p.Id == itemId).ToListAsync();
        }

        public async Task<int> CountItemPurchases(int itemId)
        {
            return await _context.Purchases.Where(p => p.Id == itemId).CountAsync();
        }

        public async Task<bool> IsUserPurchasedItem(int itemId, int userId)
        {
            var nr = await _context.Purchases
                .Where(p => p.CaffItemId == itemId && p.UserId == userId)
                .Take(1)
                .CountAsync();
            
            return nr != 0;
        }
    }
}