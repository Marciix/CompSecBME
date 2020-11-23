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
using CaffShop.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaffShop.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        
        public const string TokenAudience = "CaffShopBackend";
        public const string TokenIssuer = "CaffShopBackendIssuer";
        
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserLoginResponse>> Authenticate([FromBody] UserAuthenticateModel model)
        {
            var user = await _authenticationService.Authenticate(model.Username, model.Password);

            if (null == user)
                return BadRequest(new {message = "Username or password is incorrect"});
            
            // Create a new JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Put user's data into claims: userId and userName
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                
                // Set 2 hours as JWT lifetime
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSecret),
                    SecurityAlgorithms.HmacSha256Signature),
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                IssuedAt = DateTime.Now,
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new UserLoginResponse
            {
                JwtToken = tokenString,
                Role = user.IsAdmin ? UserHelper.RoleAdmin : UserHelper.RoleUser
            });
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdResult>> Register([FromBody] UserRegistrationModel model)
        {
            // map DTO to model
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                user = await _userService.CreateUser(user, model.Password);
                return Ok(new IdResult{ Id = user.Id});
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(new {message = ex.Message});
            }
            catch (PasswordRequiredException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}