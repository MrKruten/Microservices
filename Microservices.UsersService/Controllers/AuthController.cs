using Microservices.UsersService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.UsersService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AuthController : ControllerBase
    {
        private List<User> Users = new List<User>
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

        public AuthController(ILogger<AuthController> logger)
        {
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

            }

            return Unauthorized();
        }

        private User Authenticate(string email, string password)
        {
            return Users.SingleOrDefault(u => u.Email == email && u.Password == u.Password);
        }
    }
}