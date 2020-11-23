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
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IAuthenticationService authenticationService, IMapper mapper, ILogger<AuthController> logger)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _jwtSecret = Encoding.ASCII.GetBytes(HelperFunctions.GetEnvironmentValueOrException("JWT_SECRET"));
            _logger = logger;
        }


        [HttpPost("login")]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserLoginResponse>> Authenticate([FromBody] UserAuthenticateModel model)
        {
            try
            {
                var user = await _authenticationService.Authenticate(model.Username, model.Password);
                // Create a new JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = CreateTokenDescriptor(user);
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation($"Successful login for user #{user.Id}");

                // return basic user info and authentication token
                return Ok(new UserLoginResponse
                {
                    JwtToken = tokenString,
                    Role = user.IsAdmin ? UserHelper.RoleAdmin : UserHelper.RoleUser
                });

            }
            catch (LoginFailedException)
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured during authentication", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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
                _logger.LogInformation($"User #{user.Id} created");
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
            catch (Exception ex)
            {
                _logger.LogError("An error occured during registration", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        private SecurityTokenDescriptor CreateTokenDescriptor(User user)
        {
            return new SecurityTokenDescriptor
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
        }

    }
}