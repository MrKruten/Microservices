using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microservices.Core;
using Microservices.UsersService.Context;
using Microservices.UsersService.Models;
using Microservices.UsersService.Services.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Microservices.UsersService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> _authOptions;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly LabContext _db;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IOptions<AuthOptions> authOptions, ILogger<AuthController> logger, LabContext db, IRabbitMqService rabbitMqService)
        {
            _authOptions = authOptions;
            _logger = logger;
            _db = db;
            _rabbitMqService = rabbitMqService;
        }

        [Route("")]
        [HttpGet, ActionName("GetUsers")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"AuthController - get users {DateTime.Now}");
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }

        [Route("login")]
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var user = await GetUser(request.Email, request.Password);

            if (user != null)
            {
                var token = GenerateJWT(user);
                _logger.LogInformation($"AuthController - success login {request.Email} {DateTime.Now}");
                _rabbitMqService.SendMessage($"user {user.Email} login");
                return Ok(new { access_token = token });
            }

            _logger.LogInformation($"AuthController - unauthorized login {request.Email} {DateTime.Now}");
            return Unauthorized();
        }

        [Route("register")]
        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> Register([FromBody] Login request)
        {
            var user = await GetUserByEmail(request.Email);

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
            newUser = await AddUser(newUser);
            _logger.LogInformation($"AuthController - success register {request.Email} {DateTime.Now}");
            return Ok(newUser);
        }

        private async Task<User> AddUser(User user)
        {
            var result = await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        private async Task<User?> GetUser(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            return user;
        }

        private async Task<User?> GetUserByEmail(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        private string GenerateJWT(User user)
        {
            var authParams = _authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();
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