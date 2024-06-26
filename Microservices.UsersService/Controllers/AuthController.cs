using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microservices.Core;
using Microservices.UsersService.Models;
using Microservices.UsersService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microservices.UsersService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> _authOptions;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IUsersService _usersService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IOptions<AuthOptions> authOptions, ILogger<AuthController> logger, IUsersService usersService, IRabbitMqService rabbitMqService)
        {
            _authOptions = authOptions;
            _logger = logger;
            _rabbitMqService = rabbitMqService;
            _usersService = usersService;
        }

        [Route("")]
        [HttpGet, ActionName("GetUsers")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"AuthController - get users {DateTime.Now}");
            var users = await _usersService.GetAllUsers();
            return Ok(users);
        }

        [Route("login")]
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var user = await _usersService.GetUser(request.Email, request.Password);

            if (user != null)
            {
                var token = GenerateJWT(user);
                _logger.LogInformation($"AuthController - success login {request.Email} {DateTime.Now}");
                try
                {
                    _rabbitMqService.SendMessage($"user {user.Email} login");
                }
                catch (Exception e)
                {
                    // ignore
                }
                return Ok(new { access_token = token });
            }

            _logger.LogInformation($"AuthController - unauthorized login {request.Email} {DateTime.Now}");
            return Unauthorized();
        }

        [Route("register")]
        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> Register([FromBody] Login request)
        {
            var user = await _usersService.GetUserByEmail(request.Email);

            if (user != null)
            {
                _logger.LogInformation($"AuthController - bad request register {request.Email} {DateTime.Now}");
                return BadRequest("A user with such an email already exists");
            }

            var newUser = new User()
            {
                Email = request.Email,
                Password = request.Password,
            };
            newUser = await _usersService.AddUser(newUser);
            _logger.LogInformation($"AuthController - success register {request.Email} {DateTime.Now}");
            return Ok(newUser);
        }

        private string GenerateJWT(User user)
        {
            var authParams = _authOptions.Value;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authParams.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(authParams.Issuer, authParams.Audience,
                claims, expires: DateTime.Now.AddSeconds(authParams.TokenLifetime), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}