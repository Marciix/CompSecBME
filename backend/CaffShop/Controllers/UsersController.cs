using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models;
using CaffShop.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaffShop.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserPublic>>> ListUsers()
        {
            // Only admin can access user list
            if (false == await _userService.IsAuthenticatedUserAdmin(User))
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to view users");

            var users = await _userService.GetAllUsers();

            return Ok(_mapper.Map<UserPublic[]>(users));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteUser(long id)
        {
            // Check if user is able to delete a user (must be admin) or delete itself
            if (await IsUserNotAllowedToModifyUser(id))
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to delete this user!");

            var user = await _userService.GetUserById(id);

            if (null == user)
                return NotFound("User not found");

            await _userService.DeleteUser(user);
            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> ModifyUserData(long id, [FromBody] UserModifyModel model)
        {
            // Check if user is able to modify a user (must be admin) or delete itself
            if (await IsUserNotAllowedToModifyUser(id))
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to delete this user!");

            var user = await _userService.GetUserById(id);

            if (null == user)
                return NotFound("User not found");

            if (model.FirstName.Length != 0)
                user.FirstName = model.FirstName;

            if (model.LastName.Length != 0)
                user.LastName = model.LastName;

            await _userService.UpdateUser(user);

            return Ok();
        }

        // Function that negates CheckIfUserAllowedToModifyUser to avoid ! in if conditions
        private async Task<bool> IsUserNotAllowedToModifyUser(long userIdToModify)
        {
            return false == await CheckIfUserAllowedToModifyUser(userIdToModify);
        }

        // Returns true if the authenticated user is admin or user wants modify itself
        private async Task<bool> CheckIfUserAllowedToModifyUser(long userIdToModify)
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);
            var isAdmin = await _userService.IsAuthenticatedUserAdmin(User);
            return isAdmin || userIdToModify == userId;
        }
    }
}