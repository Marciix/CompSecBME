using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;
using Ganss.XSS;
using Microsoft.EntityFrameworkCore;

namespace CaffShop.Services
{
    public class CommentService : ICommentService
    {
        private readonly CaffShopContext _context;
        private readonly ICaffItemService _caffItemService;

        public CommentService(CaffShopContext context, ICaffItemService caffItemService)
        {
            _context = context;
            _caffItemService = caffItemService;
        }

        public async Task<List<Comment>> GetCommentForCaffItem(long caffId, bool withAuthors = false)
        {
            if (!await _caffItemService.IsCaffExists(caffId))
                throw new CaffItemNotFoundException();

            var q = _context.Comments.Where(c => c.CaffItemId == caffId).AsQueryable();

            if (withAuthors)
                q = q.Include(nameof(Comment.Author));

            return await q.ToListAsync();
        }

        public async Task<Comment> SaveComment(string content, long caffItemId, long authorId)
        {
            if (!await _caffItemService.IsCaffExists(caffItemId))
                throw new CaffItemNotFoundException();
            
            // Use HtmlSanitizer https://github.com/mganss/HtmlSanitizer#tags-allowed-by-default
            var sanitizer = new HtmlSanitizer();
            var comment = new Comment
            {
                Content = sanitizer.Sanitize(content),
                CreatedAt = DateTime.Now,
                CaffItemId = caffItemId,
                AuthorId = authorId
            };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}