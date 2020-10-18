using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaffShop.Services
{
    public class CaffItemService : ICaffItemService
    {
        private readonly CaffShopContext _context;

        public CaffItemService(CaffShopContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCaffExists(long id)
        {
            return 0 != await _context.CaffItems.Where(c => c.Id == id).Take(1).CountAsync();
        }

        public async Task<CaffItem> GetCaffItem(long id, bool withOwner = false)
        {
            var q = _context.CaffItems.AsQueryable();

            if (withOwner)
                q = q.Include("Owner");

            return await q.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<CaffItem>> GetCaffItems(bool withOwner = false)
        {
            var q = _context.CaffItems.AsQueryable();

            if (withOwner)
                q = q.Include("Owner");
            return await q.ToListAsync();
        }

        public async Task<IEnumerable<CaffItem>> SearchCaffItems(IEnumerable<string> keywords, bool withOwner = false)
        {
            var q = _context.CaffItems.AsQueryable();

            q = keywords.Aggregate(q, (current, keyword)
                => current.Where(i => i.Name.Contains(keyword) || i.Description.Contains(keyword))
            );

            if (withOwner)
                q = q.Include("Owner");

            return await q.ToListAsync();
        }

        public async Task DeleteCaffItem(CaffItem item)
        {
            _context.CaffItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<CaffItem> SaveCaff(CaffItem item)
        {
            await _context.CaffItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}