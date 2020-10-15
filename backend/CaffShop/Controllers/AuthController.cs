using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CaffShop.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("registration")]
        public async Task<ActionResult> Registration()
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("login")]
        public async Task<ActionResult> Login()
        {
            throw new NotImplementedException();
        }
    }
}