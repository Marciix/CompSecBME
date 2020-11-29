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
        public Task DeleteCaffItem(CaffItem item, long userId, bool isAdmin);
        public Task<CaffItem> SaveCaff(CaffItem item);
        public void DeleteCaffItemFromDisc(CaffItem item);
        public  Task<FileStreamResult> GetPreviewImage(long caffItemId);
        public  Task<FileStreamResult> DownloadCaffFile(long caffItemId, long userId);
    }
}