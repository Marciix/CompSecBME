using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CaffShop.Entities;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models;
using CaffShop.Models.Exceptions;
using CaffShop.Models.Options;
using CaffShop.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CaffShop.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtOptions _jwtOptions;

        public AuthController(
            IUserService userService,
            IAuthenticationService authenticationService,
            IMapper mapper,
            ILogger<AuthController> logger,
            IOptions<JwtOptions> jwtOptions)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
            _logger = logger;
            _jwtOptions = jwtOptions.Value;
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            catch (LoginInvalidPasswordException)
            {
                return Unauthorized(new {message = "Username or password is incorrect"});
            }
            catch (LoginUserDoesNotExistException)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IdResult>> Register([FromBody] UserRegistrationModel model)
        {
            // map DTO to model
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                user = await _userService.CreateUser(user, model.Password);
                _logger.LogInformation($"User #{user.Id} created");
                return Ok(new IdResult {Id = user.Id});
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(new {message = ex.Message});
            }
            catch (LoginInvalidPasswordException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (InvalidUserDataException ex)
            {
                return BadRequest(new {message = ex.Message});
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

                // Set JWT lifetime
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtOptions.SecretBytes),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtOptions.Issuer,
                IssuedAt = DateTime.Now
            };
        }


    }
}