using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaffShop.Services
{
    public class CommentService : ICommentService
    {
        private readonly CaffShopContext _context;

        public CommentService(CaffShopContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentForCaffItem(long caffId, bool withAuthors = false)
        {
            var q = _context.Comments.Where(c => c.CaffItemId == caffId).AsQueryable();

            if (withAuthors)
                q = q.Include("Author");

            return await q.ToListAsync();
        }

        public async Task<Comment> SaveComment(string content, long caffItemId, long authorId)
        {
            var comment = new Comment
            {
                Content = content,
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