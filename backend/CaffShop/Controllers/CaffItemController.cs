using System;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Entities;
using CaffShop.Interfaces;
using CaffShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaffShop.Controllers
{
    // [Authorize] TODO!!!
    [Route("[controller]")]
    public class CaffItemController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPurchaseService _purchaseService;

        public CaffItemController(IMapper mapper, IPurchaseService purchaseService)
        {
            _mapper = mapper;
            _purchaseService = purchaseService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadCaffFile()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> ViewCaffItem()
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}/buy")]
        public async Task<ActionResult> BuyCaffItem(int id)
        {
            const int userId = 1; // TODO

            if (await _purchaseService.IsUserPurchasedItem(id, userId))
            {
                return Conflict("User already purchased this item.");
            }

            try
            {
                var purchase = await _purchaseService.PurchaseItem(id, userId);
                return Ok(purchase.Id);
            }
            catch
            {
                // TODO Log error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/download")]
        public async Task<ActionResult> DownloadCaffFile(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("search/{keywords}")]
        public async Task<ActionResult> SearchCaffItems(string keywords)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCaffItem(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}/comment")]
        public async Task<ActionResult> CommentOnCaffItem(int id, [FromBody] CommentCreationModel commentCreationModel)
        {
            // TODO check if CAFF exists
            var comment = _mapper.Map<Comment>(commentCreationModel);
            comment.AuthorId = 1;
            comment.CaffItemId = id;
            comment.CreatedAt = DateTime.Now;

            return await Task.FromResult(Ok(comment));
        }
    }
}