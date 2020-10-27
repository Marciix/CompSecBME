using System.Threading.Tasks;
using CaffShop.Helpers;
using CaffShop.Interfaces;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(long id)
        {
            if (CheckIfUserAllowedToModifyUser(id))
                return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to delete this user!");

            var user = await _userService.GetUserById(id);

            if (null == user)
                return NotFound("User not found");

            await _userService.DeleteUser(user);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ModifyUserData(long id, [FromBody] UserModifyModel model)
        {
            if (CheckIfUserAllowedToModifyUser(id))
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

        private bool CheckIfUserAllowedToModifyUser(long userIdToModify)
        {
            var userId = UserHelper.GetAuthenticatedUserId(User);
            var isAdmin = UserHelper.IsAuthenticatedUserAdmin(User);
            return isAdmin || userIdToModify == userId;
        }
    }
}