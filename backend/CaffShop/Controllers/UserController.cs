﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaffShop.Controllers
{
    // [Authorize] TODO !!!
    [Route("[controller]")]
    public class UserController : Controller
    {
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(long id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ModifyUserData()
        {
            throw new NotImplementedException();
        }
    }
}