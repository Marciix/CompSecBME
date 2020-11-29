using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models;
using CaffShop.Models.CaffItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CaffShop.Models.Comment;
using CaffShop.Models.Exceptions;
using CaffShop.Models.Options;
using Microsoft.Extensions.Logging;

namespace CaffShop.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CaffItemsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICaffItemService _caffItemService;
        private readonly ICommentService _commentService;
        private readonly IPurchaseService _purchaseService;
        private readonly ICaffUploadService _caffUploadService;
        private readonly ILogger<CaffItemsController> _logger;

        public CaffItemsController(
            IMapper mapper,
            IUserService userService,
            ICaffItemService caffItemService,
            ICommentService commentService,
            IPurchaseService purchaseService,
            ICaffUploadService caffUploadService,
            ILogger<CaffItemsController> logger
        )
        {
            _mapper = mapper;
            _userService = userService;
            _caffItemService = caffItemService;
            _commentService = commentService;
            _purchaseService = purchaseService;
            _caffUploadService = caffUploadService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaffItemPublic>>> ListCaffItems(bool withOwner = false)
        {
            var items = await _caffItemService.GetCaffItems(withOwner);
            var mappedItems = _mapper.Map<CaffItemPublic[]>(items);
            return Ok(mappedItems);
        }

        [HttpPost("upload"), RequestSizeLimit(UploadOptions.UploadSizeLimit)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdResult>> UploadCaffFile()
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                if (Request.Form.Files.Count != 1)
                    return BadRequest("Submitted form data must contain exactly one file");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Request does not contain form data.");
            }

            try
            {
                var file = Request.Form.Files[0];

                var caffItem = await _caffUploadService.UploadCaffFile(file, userId);
                _logger.LogInformation($"User #{userId} created item #{caffItem.Id}");
                return Ok(new IdResult {Id = caffItem.Id});
            }
            catch (NotCaffFileException)
            {
                _logger.LogWarning($"User #{userId} tried to upload a non CAFF file.");
                return BadRequest("The file must be a caff file");
            }
            catch (CaffUploadException ex)
            {
                _logger.LogWarning($"User #{userId} tried to upload an invalid CAFF file.", ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured during CAFF upload", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CaffItemPublic>> GetCaffItem(long id, bool withOwner = false)
        {
            try
            {
                var item = await _caffItemService.GetCaffItem(id, withOwner);
                return Ok(_mapper.Map<CaffItemPublic>(item));
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("CAFF Item not found");
            }
        }

        [HttpPost("{id}/buy")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> BuyCaffItem(long id)
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                await _purchaseService.PurchaseItem(id, userId);
                _logger.LogInformation($"User #{userId} bought item #{id}");
                return Ok();
            }
            catch (UserAlreadyPurchasedItemException)
            {
                return Conflict("User already purchased this item.");
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("CAFF Item not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured during buying an item", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/download")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DownloadCaffFile(long id)
        {
            try
            {
                var userId = UserHelper.GetAuthenticatedUserId(User);
                if (!await _purchaseService.IsUserPurchasedItem(id, userId))
                    throw new UserNotPurchasedCaffItemException();
                return await _caffItemService.DownloadCaffFile(id, userId);
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("CAFF Item not found");
            }
            catch (UserNotPurchasedCaffItemException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You should buy this item before download!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured during downloading item #{id}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/preview.jpg")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PreviewCaffFile(long id)
        {
            try
            {
                return await _caffItemService.GetPreviewImage(id);
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("CAFF Item not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured during printing preview for item #{id}", ex);
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCaffItem(long id)
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                var item = await _caffItemService.GetCaffItem(id);
                var isAdmin = await _userService.IsUserAdmin(userId);
                await _caffItemService.DeleteCaffItem(item, userId, isAdmin);
                _caffItemService.DeleteCaffItemFromDisc(item);
                _logger.LogInformation($"User #{userId} deleted item #{id}");
                return Ok();
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("CAFF Item not found");
            }
            catch (UserNotAllowedToDeleteCaffException)
            {
                _logger.LogWarning($"User #{userId} tried to delete item #{id}");
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to delete this item!");
            }
        }

        [HttpGet("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CommentPublic>>> GetCommentsForCaffItem(long id,
            bool withAuthors = false)
        {
            try
            {
                var comments = await _commentService.GetCommentForCaffItem(id, withAuthors);
                return Ok(_mapper.Map<IEnumerable<CommentPublic>>(comments));
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("Caff item not found");
            }
        }


        [HttpPost("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdResult>> CommentOnCaffItem(long id,
            [FromBody] CommentCreationModel commentModel)
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                var comment = await _commentService.SaveComment(commentModel.Content, id, userId);
                _logger.LogInformation($"User #{userId} commented on item #{id}");
                return Ok(new IdResult {Id = comment.Id});
            }
            catch (CaffItemNotFoundException)
            {
                return NotFound("Caff item not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured during commenting", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}