using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CaffShop.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CaffShopContext _context;
        private readonly ICaffItemService _caffItemService;
        
        public PurchaseService(CaffShopContext context, ICaffItemService caffItemService)
        {
            _context = context;
            _caffItemService = caffItemService;
        }

        public async Task<Purchase> PurchaseItem(long itemId, long userId)
        {
            if (!await _caffItemService.IsCaffExists(itemId))
                throw new CaffItemNotFoundException();

            if (await IsUserPurchasedItem(itemId, userId))
                throw new UserAlreadyPurchasedItemException();

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

        public async Task<List<Purchase>> GetItemPurchases(long itemId)
        {
            return await _context.Purchases.Where(p => p.Id == itemId).ToListAsync();
        }

        public async Task<long> CountItemPurchases(long itemId)
        {
            return await _context.Purchases.Where(p => p.Id == itemId).CountAsync();
        }

        public async Task<bool> IsUserPurchasedItem(long itemId, long userId)
        {
            var nr = await _context.Purchases
                .Where(p => p.CaffItemId == itemId && p.UserId == userId)
                .Take(1)
                .CountAsync();

            return nr != 0;
        }
    }
}