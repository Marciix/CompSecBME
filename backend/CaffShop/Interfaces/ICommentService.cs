using System.Collections.Generic;
using System.Threading.Tasks;
using CaffShop.Entities;

namespace CaffShop.Interfaces
{
    public interface ICommentService
    {
        public Task<List<Comment>> GetCommentForCaffItem(long caffId, bool withAuthors = false);
        public Task<Comment> SaveComment(string content, long caffItemId, long authorId);
    }
}