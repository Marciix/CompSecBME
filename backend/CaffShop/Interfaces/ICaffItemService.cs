using System.Collections.Generic;
using System.Threading.Tasks;
using CaffShop.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CaffShop.Interfaces
{
    public interface ICaffItemService
    {
        public Task<bool> IsCaffExists(long id);
        public Task<CaffItem> GetCaffItem(long id, bool withOwner = false);
        public Task<IEnumerable<CaffItem>> GetCaffItems(bool withOwner = false);
        public Task<IEnumerable<CaffItem>> SearchCaffItems(IEnumerable<string> keywords, bool withOwner = false);
        public Task DeleteCaffItem(CaffItem item);
        public Task<CaffItem> SaveCaff(CaffItem item);
    }
}