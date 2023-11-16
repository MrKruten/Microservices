using Microservices.UsersService.Models;

namespace Microservices.UsersService.Services
{
    public interface IUsersService
    {
        Task<User> AddUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUser(string email, string password);
        Task<IReadOnlyList<User>> GetAllUsers();
    }
}
