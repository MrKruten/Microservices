using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microservices.Core;
using Microservices.UsersService.Models;
using Microsoft.AspNetCore.Mvc;
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
        private List<User> Users = new()
        {
        new User
        {
            Id = 1,
            Email = "user1@email.com",
            Password = "user1"
        },
        new User
        {
            Id = 2,
            Email = "user2@email.com",
            Password = "user2"
        },
        new User
        {
            Id = 3,
            Email = "user3@email.com",
            Password = "user3"
        }
    };

        private readonly ILogger<AuthController> _logger;

        public AuthController(IOptions<AuthOptions> authOptions, ILogger<AuthController> logger)
        {
            _authOptions = authOptions;
            _logger = logger;
        }

        [Route("")]
        [HttpGet, ActionName("GetUsers")]
        public IActionResult Get()
        {
            return Ok(Users);
        }

        [Route("login")]
        [HttpPost, ActionName("Login")]
        public IActionResult Login([FromBody] Login request)
        {
            var user = Authenticate(request.Email, request.Password);

            if (user != null)
            {
                var token = GenerateJWT(user);

                return Ok(new { access_token = token });
            }

            return Unauthorized();
        }

        private User Authenticate(string email, string password)
        {
            return Users.SingleOrDefault(u => u.Email == email && u.Password == password);
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