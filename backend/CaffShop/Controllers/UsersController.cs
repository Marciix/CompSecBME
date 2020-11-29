using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;
using CaffShop.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CaffShop.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IMapper mapper, ILogger<UsersController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserPublic>>> ListUsers()
        {
            var authUserId = UserHelper.GetAuthenticatedUserId(User);
            // Only admin can access user list
            if (!await _userService.IsUserAdmin(authUserId))
            {
                _logger.LogWarning($"User #{authUserId} tried to fetch user list");
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to view users");
            }

            var users = await _userService.GetAllUsers();

            return Ok(_mapper.Map<UserPublic[]>(users));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteUser(long id)
        {
            var authUserId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                await _userService.DeleteUser(id, authUserId);
                _logger.LogInformation($"User #{authUserId} deleted user #{id}");
                return Ok();
            }
            catch (UserNotExistsException)
            {
                return NotFound("User not found");
            }
            catch (UserNotAllowedToDeleteUserException)
            {
                _logger.LogWarning($"User #{authUserId} tried to delete user #{id}");
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to delete this user!");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> ModifyUserData(long id, [FromBody] UserModifyModel model)
        {
            var authUserId = UserHelper.GetAuthenticatedUserId(User);

            try
            {
                await _userService.UpdateUser(id, model.FirstName, model.LastName, authUserId);
                _logger.LogInformation($"User #{authUserId} modified user #{id}");
                return Ok();
            }
            catch (UserNotAllowedToModifyUserException)
            {
                _logger.LogWarning($"User #{authUserId} tried to edit user #{id}");
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to edit this user!");
            }
            catch (UserNotExistsException)
            {
                return NotFound("User not found.");
            }
        }
    }
}