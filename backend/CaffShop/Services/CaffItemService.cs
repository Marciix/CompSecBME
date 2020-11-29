using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
                q = q.Include(nameof(CaffItem.Owner));

            try
            {
                return await q.FirstAsync(i => i.Id == id);
            }
            catch (InvalidOperationException)
            {
                throw new CaffItemNotFoundException();
            }
        }

        public async Task<IEnumerable<CaffItem>> GetCaffItems(bool withOwner = false)
        {
            var q = _context.CaffItems.AsQueryable();

            if (withOwner)
                q = q.Include(nameof(CaffItem.Owner));
            return await q.ToListAsync();
        }

        public async Task<IEnumerable<CaffItem>> SearchCaffItems(IEnumerable<string> keywords, bool withOwner = false)
        {
            var q = _context.CaffItems.AsQueryable();

            q = keywords.Aggregate(q, (current, keyword)
                => current.Where(i => i.Title.Contains(keyword) || i.TagsText.Contains(keyword))
            );

            if (withOwner)
                q = q.Include(nameof(CaffItem.Owner));

            return await q.ToListAsync();
        }

        public async Task DeleteCaffItem(CaffItem item, long userId, bool isAdmin)
        {
            // Admins can delete all items. Users are allowed to delete only their own items.
            if (item.OwnerId != userId && !isAdmin)
                throw new UserNotAllowedToDeleteCaffException();

            _context.CaffItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<CaffItem> SaveCaff(CaffItem item)
        {
            await _context.CaffItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public void DeleteCaffItemFromDisc(CaffItem item)
        {
            if (System.IO.File.Exists(item.CaffPath))
                System.IO.File.Delete(item.CaffPath);

            if (System.IO.File.Exists(item.PreviewPath))
                System.IO.File.Delete(item.PreviewPath);
        }

        // Returns a filestream
        public async Task<FileStreamResult> GetPreviewImage(long caffItemId)
        {
            var item = await GetCaffItem(caffItemId);
            return GetFileStream(item.PreviewPath);
        }

        // Returns a filestream result with Download Name which tells the browser: download the file 
        public async Task<FileStreamResult> DownloadCaffFile(long caffItemId, long userId)
        {
            var item = await GetCaffItem(caffItemId);

            var fileStream = GetFileStream(item.CaffPath);
            fileStream.FileDownloadName = item.OriginalName;
            return fileStream;
        }

        private static FileStreamResult GetFileStream(string path)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(path, out var contentType);
            var stream = File.OpenRead(path);
            return new FileStreamResult(stream, contentType ?? "application/octet-stream");
        }
    }
}