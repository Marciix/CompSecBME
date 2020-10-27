using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models;
using CaffShop.Models.CaffItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http.Headers;
using CaffShop.Entities;
using CaffShop.Models.Exceptions;
using CaffShop.Models.Settings;
using ImageMagick;

namespace CaffShop.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CaffItemsController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ICaffItemService _caffItemService;
        private readonly ICommentService _commentService;
        private readonly IPurchaseService _purchaseService;
        private readonly ICaffParserWrapper _caffParserWrapper;
        private readonly UploadSettings _us;

        public CaffItemsController(IMapper mapper, ICaffItemService caffItemService, ICommentService commentService,
            IPurchaseService purchaseService, ICaffParserWrapper caffParserWrapper, UploadSettings us)
        {
            _mapper = mapper;
            _caffItemService = caffItemService;
            _commentService = commentService;
            _purchaseService = purchaseService;
            _caffParserWrapper = caffParserWrapper;
            _us = us;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaffItemPublic>>> ListCaffItems(bool withOwner = false)
        {
            var items = await _caffItemService.GetCaffItems(withOwner);
            var mappedItems = _mapper.Map<CaffItemPublic[]>(items);
            return Ok(mappedItems);
        }

        [HttpPost("upload"), RequestSizeLimit(UploadSettings.UploadSizeLimit)]
        public async Task<ActionResult<IdResult>> UploadCaffFile([FromForm] CaffItemCreation itemMeta)
        {
            if (Request.Form.Files.Count != 1)
                return BadRequest("Submitted form data must contain exactly one file");
            var file = Request.Form.Files[0];

            var originalName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            
            if (Path.GetExtension(originalName).ToLower() != ".caff")
                return BadRequest("The file must be a caff file");

            var rndName = RandomFileNameWithTimestamp();

            var tempFilePath = Path.Combine(_us.TempDirPath, rndName + ".caff");
            var caffFilePath = Path.Combine(_us.CaffDirPath, rndName + ".caff");
            var prevFilePath = Path.Combine(_us.PrevDirPath, rndName + ".ppm");

            await using var stream = new FileStream(tempFilePath, FileMode.Create);
            await file.CopyToAsync(stream);

            try
            {
                _caffParserWrapper.ValidateAndParseCaff(tempFilePath, prevFilePath);
                System.IO.File.Move(tempFilePath, caffFilePath);
            }
            catch (InvalidCaffFileException ex)
            {
                System.IO.File.Delete(tempFilePath);
                return BadRequest(ex.Message);
            }
            
            var finalPrevFilePath = Path.Combine(_us.PrevDirPath, rndName + UploadSettings.PreviewExtension);
            ConvertPpmToJpg(prevFilePath, finalPrevFilePath);

            var item = new CaffItem
            {
                Name = itemMeta.Name,
                Description = itemMeta.Description,
                OwnerId = UserHelper.GetAuthenticatedUserId(User),
                UploadedAt = DateTime.Now,
                CaffPath = caffFilePath,
                PreviewPath = finalPrevFilePath,
                InnerName = rndName,
                OriginalName = originalName
            };

            try
            {
                item = await _caffItemService.SaveCaff(item);
                return Ok(new IdResult{ Id = item.Id});
            }
            catch
            {
                // TODO Log error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static void ConvertPpmToJpg(string ppmPath, string targetPath)
        {
            if (false == System.IO.File.Exists(ppmPath))
                throw new FileNotFoundException();

            using var img = new MagickImage(ppmPath);
            img.Write(targetPath);
            System.IO.File.Delete(ppmPath);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaffItemPublic>> GetCaffItem(long id, bool withOwner = false)
        {
            var item = await _caffItemService.GetCaffItem(id, withOwner);

            if (item == null)
                return NotFound("CAFF Item not found");

            return Ok(_mapper.Map<CaffItemPublic>(item));
        }

        [HttpPost("{id}/buy")]
        public async Task<ActionResult> BuyCaffItem(long id)
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);

            if (false == await _caffItemService.IsCaffExists(id))
                return NotFound("CAFF Item not found");

            if (await _purchaseService.IsUserPurchasedItem(id, userId))
                return Conflict("User already purchased this item.");

            try
            {
                await _purchaseService.PurchaseItem(id, userId);
                return Ok();
            }
            catch
            {
                // TODO Log error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/download")]
        public async Task<ActionResult> DownloadCaffFile(long id)
        {
            var item = await _caffItemService.GetCaffItem(id);

            if (null == item)
                return NotFound("CAFF Item not found");

            var userId = UserHelper.GetAuthenticatedUserId(User);
            if (false == await _purchaseService.IsUserPurchasedItem(id, userId))
                return StatusCode(StatusCodes.Status403Forbidden, "You should buy this item before download!");

            try
            {
                return DownloadFileStreamResult(item.CaffPath, item.OriginalName);
            }
            catch
            {
                // TODO Log error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/preview.jpg")]
        public async Task<ActionResult> PreviewCaffFile(long id)
        {
            var item = await _caffItemService.GetCaffItem(id);

            if (null == item)
                return NotFound("CAFF Item not found");

            try
            {
                return GetFileStreamResult(item.PreviewPath);
            }
            catch
            {
                // TODO Log error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("search/{keyword}")]
        public async Task<ActionResult<IEnumerable<CaffItemPublic>>> SearchCaffItems(string keyword,
            bool withOwner = false)
        {
            var keywords = keyword.Split(' ');
            var items = await _caffItemService.SearchCaffItems(keywords, withOwner);
            var mappedItems = _mapper.Map<IEnumerable<CaffItemPublic>>(items);

            return Ok(mappedItems);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCaffItem(long id)
        {
            var item = await _caffItemService.GetCaffItem(id);

            if (null == item)
                return NotFound("CAFF Item not found");

            var userId = UserHelper.GetAuthenticatedUserId(User);
            var isAdmin = UserHelper.IsAuthenticatedUserAdmin(User);

            if (item.OwnerId != userId && false == isAdmin)
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to delete this item!");

            await _caffItemService.DeleteCaffItem(item);
            DeleteCaffItemFromDisc(item);
            return Ok();
        }

        private static void DeleteCaffItemFromDisc(CaffItem item)
        {
            if (System.IO.File.Exists(item.CaffPath))
                System.IO.File.Delete(item.CaffPath);

            if (System.IO.File.Exists(item.PreviewPath))
                System.IO.File.Delete(item.PreviewPath);
        }

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<CommentPublic>>> GetCommentsForCaffItem(long id,
            bool withAuthors = false)
        {
            if (false == await _caffItemService.IsCaffExists(id))
                return NotFound("Caff item not found");

            var comments = await _commentService.GetCommentForCaffItem(id, withAuthors);
            return Ok(_mapper.Map<IEnumerable<CommentPublic>>(comments));
        }


        [HttpPost("{id}/comments")]
        public async Task<ActionResult<IdResult>> CommentOnCaffItem(long id, [FromBody] CommentCreationModel commentModel)
        {
            if (false == await _caffItemService.IsCaffExists(id))
                return NotFound("Caff item not found");

            var userId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                var comment = await _commentService.SaveComment(commentModel.Content, id, userId);
                return Ok(new IdResult{ Id = comment.Id});
            }
            catch
            {
                // TODO Log error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        private static FileStreamResult GetFileStreamResult(string fullPath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(fullPath, out var contentType);
            var stream = System.IO.File.OpenRead(fullPath);
            return new FileStreamResult(stream, contentType ?? "application/octet-stream");
        }

        private static FileStreamResult DownloadFileStreamResult(string fullPath, string fileName)
        {
            var fileStream = GetFileStreamResult(fullPath);

            fileStream.FileDownloadName = fileName;

            return fileStream;
        }

        private static string RandomFileNameWithTimestamp()
        {
            var ts = HelperFunctions.GetUnixTimestamp();
            var rnd = HelperFunctions.GenerateRandomString(10);
            return $"{ts}_{rnd}";
        }
    }
}