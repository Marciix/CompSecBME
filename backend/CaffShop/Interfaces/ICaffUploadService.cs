using System.Threading.Tasks;
using CaffShop.Entities;
using Microsoft.AspNetCore.Http;

namespace CaffShop.Interfaces
{
    public interface ICaffUploadService
    {
        public Task<CaffItem> UploadCaffFile(IFormFile file, long userId);
    }
}