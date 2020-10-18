using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Entities;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaffShop.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {

        
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly byte[] _jwtSecret;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, IAuthenticationService authenticationService, IMapper mapper)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _jwtSecret = Encoding.ASCII.GetBytes(HelperFunctions.GetEnvironmentValueOrException("JWT_SECRET"));
        }

        
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Authenticate([FromBody] UserAuthenticateModel model)
        {
            var user = await _authenticationService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new {message = "Username or password is incorrect"});

            var role = user.IsAdmin ? UserHelper.RoleAdmin : UserHelper.RoleUser;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSecret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new UserLoginResponse
            {
                JwtToken = tokenString,
                Role = role
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<long>> Register([FromBody] UserRegistrationModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                user = await _userService.CreateUser(user, model.Password);
                return Ok(user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

    }
}